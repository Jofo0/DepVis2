using DepVis.Shared.Messages;
using LibGit2Sharp;
using MassTransit;
using System.Diagnostics;

namespace DepVis.Processing.Consumers;

public class ProcessingMessageConsumer : IConsumer<ProcessingMessage>
{

    public async Task Consume(ConsumeContext<ProcessingMessage> context)
    {
        var githubLink = context.Message.GitHubLink;
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var filename = $"{Guid.NewGuid()}.cdx.sbom.json";
        var outputFile = Path.Combine(tempDir, filename);

        try
        {
            Repository.Clone(githubLink, tempDir);

            var syft = new ProcessStartInfo
            {
                FileName = "syft",
                Arguments = $". -o cyclonedx-json={outputFile}",
                WorkingDirectory = tempDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            await RunProcessAsync(syft);

            Console.WriteLine($"SBOM generated: {outputFile}");

            var minio = new MinioStorageService();
            await minio.UploadAsync(outputFile, filename);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    private async Task RunProcessAsync(ProcessStartInfo psi)
    {
        using var process = new Process { StartInfo = psi };
        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        Console.WriteLine(stdout);
        if (!string.IsNullOrWhiteSpace(stderr))
            Console.Error.WriteLine(stderr);

        if (process.ExitCode != 0)
            throw new Exception($"Process '{psi.FileName}' failed with exit code {process.ExitCode}");
    }

}
