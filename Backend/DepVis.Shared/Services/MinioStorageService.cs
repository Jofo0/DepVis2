using DepVis.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace DepVis.Shared.Services;

public class MinioStorageService
{
    private readonly IMinioClient _minio;
    private readonly string _bucketName;
    readonly ILogger<MinioStorageService> _logger;

    public MinioStorageService(
        ILogger<MinioStorageService> logger,
        IOptions<ConnectionStrings> connectionStrings
    )
    {
        _logger = logger;
        _bucketName = "sbom-bucket";

        _minio = new MinioClient()
            .WithEndpoint(connectionStrings.Value.MinioEndpoint, 9000)
            .WithCredentials("minioadmin", "minioadmin")
            .Build();
    }

    public async Task EnsureBucketExistsAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Checking if bucket {bucketName} exists...", _bucketName);
        var found = await _minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName),
            cancellationToken: ct
        );

        if (!found)
        {
            _logger.LogInformation(
                "Bucket {bucketName} does not exists... Creating it...",
                _bucketName
            );
            await _minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName),
                cancellationToken: ct
            );
            _logger.LogInformation("Bucket {bucketName} created", _bucketName);
        }
    }

    public async Task<Stream> RetrieveAsync(string filename, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation(
                "Checking if file {filename} exists in bucket {bucketName}...",
                filename,
                _bucketName
            );

            await _minio.StatObjectAsync(
                new StatObjectArgs().WithBucket(_bucketName).WithObject(filename),
                cancellationToken: ct
            );
        }
        catch
        {
            _logger.LogError(
                "File {filename} not found in bucket {bucketName}",
                filename,
                _bucketName
            );
            throw new FileNotFoundException($"File {filename} not found in bucket {_bucketName}");
        }

        var resultStream = new MemoryStream();

        _logger.LogInformation(
            "Retrieving file {filename} from bucket {bucketName}...",
            filename,
            _bucketName
        );

        var file = await _minio.GetObjectAsync(
            new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(filename)
                .WithCallbackStream(stream => stream.CopyTo(resultStream)),
            cancellationToken: ct
        );

        _logger.LogInformation(
            "File {filename} retrieved successfully from bucket {bucketName}",
            filename,
            _bucketName
        );

        resultStream.Position = 0;
        return resultStream;
    }

    public async Task<string> UploadAsync(
        string filePath,
        string key,
        CancellationToken ct = default
    )
    {
        await EnsureBucketExistsAsync(ct);

        _logger.LogInformation(
            "Uploading file {filePath} to bucket {bucketName} with key {key}...",
            filePath,
            _bucketName,
            key
        );

        await _minio.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(key)
                .WithFileName(filePath)
                .WithContentType("application/json"),
            cancellationToken: ct
        );

        _logger.LogInformation(
            "File {filePath} uploaded successfully to bucket {bucketName} with key {key}",
            filePath,
            _bucketName,
            key
        );

        return $"http://localhost:9000/{_bucketName}/{key}";
    }
}
