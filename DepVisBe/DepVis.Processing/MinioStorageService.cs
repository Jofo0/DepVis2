using Minio;
using Minio.DataModel.Args;

namespace DepVis.Processing;

public class MinioStorageService
{
    private readonly IMinioClient _minio;
    private readonly string _bucketName;

    public MinioStorageService()
    {
        _bucketName = "sbom-bucket";
        _minio = new MinioClient()
            .WithEndpoint("localhost:9000")
            .WithCredentials("minioadmin", "minioadmin")
            .Build();
    }

    public async Task EnsureBucketExistsAsync()
    {
        var found = await _minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName)
        );

        if (!found)
        {
            await _minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName)
            );
        }
    }

    public async Task<string> UploadAsync(string filePath, string key)
    {
        await EnsureBucketExistsAsync();

        await _minio.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(key)
                .WithFileName(filePath)
                .WithContentType("application/json")
        );

        return $"http://localhost:9000/{_bucketName}/{key}";
    }
}
