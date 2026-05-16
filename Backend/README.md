# DepVis — Dependency Visualizer Backend

A .NET 9 backend for analyzing, tracking, and visualizing software dependencies and vulnerabilities across Git repositories. The system clones repositories, generates SBOMs (Software Bill of Materials) using **Trivy**, ingests the results into SQL Server, and exposes a REST API for a frontend to consume.

## Architecture Overview

```
┌─────────────┐   REST/OData    ┌──────────────┐
│   Frontend   │◄──────────────►│  DepVis.Core  │
│  (Vue/React) │                │   (Web API)   │
└─────────────┘                 └──────┬───────┘
                                       │ MassTransit
                                       │ (SQL Server Transport)
                                       ▼
                                ┌──────────────────┐
                                │ DepVis.Processing │
                                │  (SBOM Worker)    │
                                └──────┬───────────┘
                                       │
                          ┌────────────┼────────────┐
                          ▼            ▼            ▼
                      ┌───────┐  ┌─────────┐  ┌──────────┐
                      │ Trivy │  │  MinIO   │  │SQL Server│
                      │ (scan)│  │ (SBOMs)  │  │  (data)  │
                      └───────┘  └─────────┘  └──────────┘
```

## Projects

| Project | Description |
|---|---|
| **DepVis.Core** | ASP.NET Core Web API — controllers, services, repositories, EF Core migrations, MassTransit consumers for ingestion |
| **DepVis.Processing** | Background worker — clones repos, runs Trivy to generate CycloneDX SBOMs, uploads to MinIO, publishes results via MassTransit |
| **DepVis.Shared** | Shared library — entity models, MassTransit message contracts, MinIO service, configuration options |
| **DepVis.ServiceDefaults** | Shared startup — MassTransit configuration (SQL Server transport), database auto-creation |
| **DepVis.Tests** | xUnit test project — unit tests for services, extensions, models, and processing logic |

## Key Features

- **SBOM Generation** — Trivy scans produce CycloneDX BOMs with dependency and vulnerability data
- **Branch History Analysis** — Processes every commit on a branch to track dependency changes over time, with parallel multi-worker processing
- **Content-Hash Deduplication** — SHA256 of sorted purls + vulnerability IDs to skip commits with unchanged dependencies
- **Dependency Graph** — DFS-based graph traversal for parent/child package relationships with severity filtering
- **Branch Comparison** — Diff packages and vulnerabilities between two branches (added, removed, upgraded, downgraded)
- **OData Support** — Server-side filtering, sorting, and paging on packages, vulnerabilities, and branches
- **CSV & DOT Export** — Export packages, vulnerabilities, branch history, and dependency graphs
- **SBOM Download** — Download the raw CycloneDX JSON for any branch
- **Progress Tracking** — Real-time processing progress via IMemoryCache with ETA estimation

## API Endpoints

### Projects — `/api/projects`

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/projects` | List all projects |
| `GET` | `/api/projects/{id}` | Get project by ID |
| `GET` | `/api/projects/{id}/info` | Get editable project info |
| `POST` | `/api/projects` | Create a project |
| `PUT` | `/api/projects/{id}` | Update a project |
| `DELETE` | `/api/projects/{id}` | Delete a project |

### Branches — `/api/projects/{projectId}/...`

| Method | Route | Description |
|---|---|---|
| `GET` | `.../branches` | List branches for a project |
| `GET` | `.../branches/detailed` | Detailed branch list (OData, CSV export) |
| `GET` | `.../stats` | Project-level statistics |
| `POST` | `.../branches/{branchId}/process` | Trigger SBOM generation for a branch |
| `GET` | `.../branches/{branchId}/compare/{comparedWith}` | Compare two branches |
| `GET` | `.../branches/{branchId}/history` | Get branch commit history (CSV export) |
| `POST` | `.../branches/{branchId}/history` | Trigger history processing |
| `POST` | `.../branches/{branchId}/history/{historyId}/ingest` | Ingest a specific history commit |
| `GET` | `.../branches/{branchId}/sbom/download` | Download latest SBOM JSON |

### Packages — `/api/projects`

| Method | Route | Description |
|---|---|---|
| `GET` | `/{branchId}/packages` | List packages (OData, CSV export) |
| `GET` | `/packages/{packageId}` | Package details |
| `GET` | `/{branchId}/packages/graph` | Dependency graph (DOT export) |
| `GET` | `/{branchId}/packages/graph/{packageId}` | Package hierarchy to root |

### Vulnerabilities — `/api/projects`

| Method | Route | Description |
|---|---|---|
| `GET` | `/{branchId}/vulnerabilities` | List vulnerabilities (OData, CSV export) |
| `GET` | `/vulnerabilities/{vulnId}` | Vulnerability details |

### Git — `/api/git`

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/git/{gitHubUrl}` | Retrieve branches and tags from a Git repo |

## Message Flow (MassTransit)

Messages are transported via **SQL Server** (no external broker required).

```
[Core]                              [Processing]
  │                                      │
  │── ProcessingMessage ───────────────►│  Clone + Trivy scan
  │◄── UpdateProcessingMessage ────────│  Status updates
  │                                      │
  │── BranchHistoryProcessingMessage ──►│  Multi-worker history scan
  │◄── BranchProcessingCountMessage ───│  Progress (cached in Core)
  │◄── UpdateBranchHistoryProcessingMsg│  Final results with commits
  │                                      │
  │── IngestProcessingMessage ────────►(Core consumer)  Parse & store SBOM
  │── IngestBranchHistoryMessage ─────►(Core consumer)  Parse history SBOMs
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or Docker: `mcr.microsoft.com/mssql/server`)
- [MinIO](https://min.io/) for SBOM file storage
- [Trivy](https://aquasecurity.github.io/trivy/) (installed in the Processing Docker image; needed locally for dev)