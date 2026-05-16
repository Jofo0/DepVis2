# DepVis v2 — Frontend

**DepVis** is a dependency visualization and software composition analysis (SCA) platform. This is the frontend application that provides an interactive UI for managing projects, analyzing dependencies, tracking vulnerabilities, and visualizing dependency graphs.

## Features

- **Project Management** — Create, edit, and delete software projects with metadata (name, type, tags, ecosystem)
- **Dependency Graph** — Interactive force-directed graph visualization with severity-based filtering and DOT export
- **Vulnerability Tracking** — View, filter, and sort security vulnerabilities by severity (low / medium / high / critical) with details, CWEs, and recommendations
- **Package Analysis** — Explore dependency trees with ecosystem and depth-based hierarchy analysis
- **Branch Comparison** — Compare branches to see added/removed packages, ecosystems, and vulnerabilities
- **Branch History** — Track dependency changes across commits over time
- **Processing Pipeline** — Monitor SBOM creation and ingestion workflows
- **Data Export** — Export analysis data as CSV or DOT files

## Tech Stack

| Category      | Technologies                                           |
| ------------- | ------------------------------------------------------ |
| Framework     | React 19, TypeScript, Vite 7                           |
| State         | Redux Toolkit, RTK Query                               |
| Routing       | React Router 7                                         |
| Styling       | TailwindCSS 4, shadcn/ui, Radix UI                     |
| Visualization | React Force Graph 2D, Cytoscape.js, Recharts, d3-force |
| Tables        | TanStack Table                                         |
| Forms         | React Hook Form, Zod                                   |
| i18n          | react-intl                                             |

## Prerequisites

- [Node.js](https://nodejs.org/) 20+
- npm

## Getting Started

### Install dependencies

```bash
npm install
```

### Configure the API

Set the backend API URL via the `VITE_API_BASE_URL` environment variable. It defaults to `https://localhost:7273` if not set.

```bash
# .env (or .env.local)
VITE_API_BASE_URL=https://localhost:7273
```

### Development

```bash
npm run dev
```

### Production build

```bash
npm run build
npm run preview   # preview the production build locally
```

### Lint

```bash
npm run lint
```

## Project Structure

```
src/
├── components/
│   ├── ui/              # shadcn/ui primitives (button, dialog, table, …)
│   ├── cards/           # ProjectCard, ProcessingCard
│   ├── chart/           # PieCustomChart, XYChart
│   ├── graph/           # Dependency graph, node info, legend, selectors
│   └── table/           # DataTable, SearchFilter, SortButton
├── pages/
│   ├── DashboardPage    # Project grid overview
│   ├── ProjectPage/     # Project details & processing status
│   ├── Graph            # Interactive dependency graph
│   ├── Vulnerabilities  # Vulnerability table & charts
│   ├── Branches         # Branch management
│   ├── Packages/        # Package analysis
│   ├── BranchHistory/   # Commit-level tracking
│   └── ComparePage/     # Branch comparison
├── store/api/           # RTK Query API slices (projects, branches, git)
├── types/               # TypeScript type definitions
├── utils/               # Helpers, hooks, OData builders, column definitions
├── theme/               # Colors, ThemeProvider
├── intl/                # i18n translations
└── lib/                 # Shared utilities
```
