using System.Collections.Concurrent;

namespace DepVis.SbomProcessing;

public static class TrivyLockProvider
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public static SemaphoreSlim GetLock(string runnerKey) =>
        _locks.GetOrAdd(runnerKey, _ => new SemaphoreSlim(1, 1));
}
