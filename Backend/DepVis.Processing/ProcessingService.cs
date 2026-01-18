using System.Diagnostics;

namespace DepVis.SbomProcessing
{
    public class ProcessingService(ILogger<ProcessingService> _logger)
    {
        public async Task BestEffortInstallDependencies(string repoDir, CancellationToken ct)
        {
            if (File.Exists(Path.Combine(repoDir, "pnpm-lock.yaml")))
            {
                await TryRun("pnpm", "install --ignore-scripts --force", repoDir, ct);
                return;
            }
            else if (File.Exists(Path.Combine(repoDir, "package-lock.json")))
            {
                await TryRun("npm", "ci --ignore-scripts", repoDir, ct);
                return;
            }
            else if (File.Exists(Path.Combine(repoDir, "yarn.lock")))
            {
                await TryRun("yarn", "install --frozen-lockfile --ignore-scripts", repoDir, ct);
                return;
            }

            if (Directory.EnumerateFiles(repoDir, "*.sln", SearchOption.AllDirectories).Any()
                || Directory.EnumerateFiles(repoDir, "*.csproj", SearchOption.AllDirectories).Any())
            {
                await TryRun("dotnet", "restore", repoDir, ct);
                return;
            }
        }

        public async Task RunTrivy(string directory, string output, CancellationToken ct = default)
        {
            var trivy = new ProcessStartInfo
            {
                FileName = "trivy",
                Arguments =
                    $"fs --format cyclonedx --output {output} --include-dev-deps --scanners vuln .",
                WorkingDirectory = directory,
                UseShellExecute = false,
            };

            _logger.LogDebug("Running Trivy on the cloned repository");
            await RunProcessAsync(trivy);
            _logger.LogDebug("Trivy ran succesfully and the SBOM has been created");
        }

        public async Task RunSyft(string directory, string output, CancellationToken ct = default)
        {
            var trivy = new ProcessStartInfo
            {
                FileName = "syft",
                Arguments = $". -o cyclonedx-json={output}",
                WorkingDirectory = directory,
                UseShellExecute = false,
            };

            _logger.LogDebug("Running syft on the cloned repository");
            await RunProcessAsync(trivy,ct);
            _logger.LogDebug("syft ran succesfully and the SBOM has been created");
        }

        private async Task TryRun(string fileName, string args, string workingDir, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Dependency install step: {cmd} {args}", fileName, args);
                await RunProcessAsync(new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    WorkingDirectory = workingDir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Install step failed (continuing): {cmd} {args}", fileName, args);
            }
        }

        public async Task RunProcessAsync(ProcessStartInfo psi, CancellationToken ct = default)
        {
            using var process = new Process { StartInfo = psi };
            process.Start();

            await process.WaitForExitAsync(ct);

            if (process.ExitCode != 0)
                throw new Exception(
                    $"Process '{psi.FileName}' failed with exit code {process.ExitCode}"
                );
        }
    }
}
