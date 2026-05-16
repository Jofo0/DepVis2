using DepVis.SbomProcessing.Consumers;
using DepVis.SbomProcessing.Models;
using DepVis.SbomProcessing.Options;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DepVis.Tests.Processing;

public class BranchHistoryProcessingTests
{
    #region SplitIntoChunks

    [Fact]
    public void SplitIntoChunks_DistributesEvenly()
    {
        var commits = MakeCommits(10);
        var chunks = BranchHistoryProcessingMessageConsumer.SplitIntoChunks(commits, 2);

        Assert.Equal(2, chunks.Count);
        Assert.Equal(5, chunks[0].Count);
        Assert.Equal(5, chunks[1].Count);
    }

    [Fact]
    public void SplitIntoChunks_HandlesRemainder()
    {
        var commits = MakeCommits(7);
        var chunks = BranchHistoryProcessingMessageConsumer.SplitIntoChunks(commits, 3);

        Assert.Equal(3, chunks.Count);
        // 7/3 = 2 remainder 1 → first chunk gets 3, others get 2
        Assert.Equal(3, chunks[0].Count);
        Assert.Equal(2, chunks[1].Count);
        Assert.Equal(2, chunks[2].Count);
    }

    [Fact]
    public void SplitIntoChunks_SingleWorker_ReturnsAll()
    {
        var commits = MakeCommits(5);
        var chunks = BranchHistoryProcessingMessageConsumer.SplitIntoChunks(commits, 1);

        Assert.Single(chunks);
        Assert.Equal(5, chunks[0].Count);
    }

    [Fact]
    public void SplitIntoChunks_MoreWorkersThanCommits_ProducesEmptyChunks()
    {
        var commits = MakeCommits(2);
        var chunks = BranchHistoryProcessingMessageConsumer.SplitIntoChunks(commits, 5);

        Assert.Equal(5, chunks.Count);
        Assert.Equal(2, chunks.Sum(c => c.Count));
    }

    [Fact]
    public void SplitIntoChunks_PreservesOrder()
    {
        var commits = MakeCommits(6);
        var chunks = BranchHistoryProcessingMessageConsumer.SplitIntoChunks(commits, 2);

        var flattened = chunks.SelectMany(c => c).ToList();
        for (var i = 0; i < commits.Count; i++)
            Assert.Equal(commits[i].Sha, flattened[i].Sha);
    }

    #endregion

    #region ComputeContentHash

    [Fact]
    public void ComputeContentHash_SameBom_ProducesSameHash()
    {
        var bom = MakeBom(["pkg:npm/a@1.0", "pkg:npm/b@2.0"], ["CVE-1"]);
        var hash1 = BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom);
        var hash2 = BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeContentHash_DifferentOrder_ProducesSameHash()
    {
        var bom1 = MakeBom(["pkg:npm/b@2.0", "pkg:npm/a@1.0"], ["CVE-2", "CVE-1"]);
        var bom2 = MakeBom(["pkg:npm/a@1.0", "pkg:npm/b@2.0"], ["CVE-1", "CVE-2"]);

        Assert.Equal(
            BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom1),
            BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom2)
        );
    }

    [Fact]
    public void ComputeContentHash_DifferentContent_ProducesDifferentHash()
    {
        var bom1 = MakeBom(["pkg:npm/a@1.0"], []);
        var bom2 = MakeBom(["pkg:npm/a@2.0"], []);

        Assert.NotEqual(
            BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom1),
            BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom2)
        );
    }

    [Fact]
    public void ComputeContentHash_EmptyBom_DoesNotThrow()
    {
        var bom = new CycloneDxBom { Components = [], Vulnerabilities = [] };
        var hash = BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void ComputeContentHash_NullCollections_DoesNotThrow()
    {
        var bom = new CycloneDxBom { Components = null, Vulnerabilities = null };
        var hash = BranchHistoryProcessingMessageConsumer.ComputeContentHash(bom);
        Assert.NotEmpty(hash);
    }

    #endregion

    #region DeduplicateAcrossWorkers

    [Fact]
    public void DeduplicateAcrossWorkers_RemovesDuplicateHashes()
    {
        var consumer = CreateConsumer();
        var results = new[]
        {
            MakeResult("sha1", new DateTime(2025, 1, 1), "hashA"),
            MakeResult("sha2", new DateTime(2025, 1, 2), "hashA"), // duplicate
            MakeResult("sha3", new DateTime(2025, 1, 3), "hashB")
        };

        var deduped = consumer.DeduplicateAcrossWorkers(results);

        Assert.Equal(2, deduped.Count);
        Assert.Equal("sha1", deduped[0].CommitSha);
        Assert.Equal("sha3", deduped[1].CommitSha);
    }

    [Fact]
    public void DeduplicateAcrossWorkers_KeepsAllWhenNoDuplicates()
    {
        var consumer = CreateConsumer();
        var results = new[]
        {
            MakeResult("sha1", new DateTime(2025, 1, 1), "hashA"),
            MakeResult("sha2", new DateTime(2025, 1, 2), "hashB"),
            MakeResult("sha3", new DateTime(2025, 1, 3), "hashC")
        };

        var deduped = consumer.DeduplicateAcrossWorkers(results);

        Assert.Equal(3, deduped.Count);
    }

    [Fact]
    public void DeduplicateAcrossWorkers_KeepsAllWhenDuplicatesButNotNextToEachOther()
    {
        var consumer = CreateConsumer();
        var results = new[]
        {
            MakeResult("sha1", new DateTime(2025, 1, 1), "hashA"),
            MakeResult("sha2", new DateTime(2025, 1, 2), "hashB"),
            MakeResult("sha3", new DateTime(2025, 1, 3), "hashC"),
            MakeResult("sha4", new DateTime(2025, 1, 4), "hashB")
        };

        var deduped = consumer.DeduplicateAcrossWorkers(results);

        Assert.Equal(4, deduped.Count);
    }

    [Fact]
    public void DeduplicateAcrossWorkers_OrdersByDateThenSha()
    {
        var consumer = CreateConsumer();
        var results = new[]
        {
            MakeResult("sha3", new DateTime(2025, 3, 1), "hashC"),
            MakeResult("sha1", new DateTime(2025, 1, 1), "hashA"),
            MakeResult("sha2", new DateTime(2025, 2, 1), "hashB")
        };

        var deduped = consumer.DeduplicateAcrossWorkers(results);

        Assert.Equal("sha1", deduped[0].CommitSha);
        Assert.Equal("sha2", deduped[1].CommitSha);
        Assert.Equal("sha3", deduped[2].CommitSha);
    }

    [Fact]
    public void DeduplicateAcrossWorkers_EmptyInput_ReturnsEmpty()
    {
        var consumer = CreateConsumer();
        var deduped = consumer.DeduplicateAcrossWorkers([]);
        Assert.Empty(deduped);
    }

    #endregion

    #region Helpers

    private static List<CommitDescriptor> MakeCommits(int count)
    {
        return Enumerable.Range(0, count)
            .Select(i => new CommitDescriptor
            {
                Sha = $"sha-{i:D4}",
                MessageShort = $"commit {i}",
                CommitDate = new DateTime(2025, 1, 1).AddDays(i)
            })
            .ToList();
    }

    private static CycloneDxBom MakeBom(string[] purls, string[] vulnIds)
    {
        return new CycloneDxBom
        {
            Components = purls.Select(p => new CycloneDxComponent("n", "v", "g", p, [], "library", p)).ToList(),
            Vulnerabilities = vulnIds.Select(id => new CycloneDxVulnerability(
                id, new Source("src", null), [], "", "", [], [], DateTime.MinValue, DateTime.MinValue, []
            )).ToList()
        };
    }

    private static ProcessedCommitResult MakeResult(string sha, DateTime date, string hash)
    {
        return new ProcessedCommitResult
        {
            CommitInfo = new CommitProcessingInfo
            {
                CommitSha = sha,
                CommitMessage = $"msg-{sha}",
                CommitDate = date,
                FileName = $"{sha}.json"
            },
            PackageCount = 1,
            VulnerabilityCount = 0,
            ContentHash = hash
        };
    }

    private static BranchHistoryProcessingMessageConsumer CreateConsumer()
    {
        return new BranchHistoryProcessingMessageConsumer(
            Substitute.For<ILogger<BranchHistoryProcessingMessageConsumer>>(),
            null!,
            Substitute.For<IPublishEndpoint>(),
            null!,
            Options.Create(new ProcessingOptions { WorkerCount = 2 })
        );
    }

    #endregion
}