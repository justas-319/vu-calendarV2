namespace VuCalendar.UnitTests;

public class CacheTests
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(1);

    [Fact]
    public async Task GetOrAdd_SameKeyInParallel_FetchesOnlyOnce()
    {
        // Arrange
        var cache = new Cache<string, int>();
        int fetchCount = 0;

        async Task<int> Fetch()
        {
            Interlocked.Increment(ref fetchCount);
            await Task.Delay(50); // keeps the fetch running while the other callers arrive
            return 42;
        }

        // Act: 50 callers on different threads ask for the same key at once
        Task<int>[] tasks = Enumerable
            .Range(0, 50)
            .Select(_ => Task.Run(() => cache.GetOrAdd("key", Fetch, Lifetime)))
            .ToArray();

        int[] results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(1, fetchCount);
        Assert.All(results, result => Assert.Equal(42, result));
    }

    [Fact]
    public async Task GetOrAdd_DifferentKeys_FetchesEachKey()
    {
        // Arrange
        var cache = new Cache<string, string>();
        int fetchCount = 0;

        Task<string> Fetch(string value)
        {
            Interlocked.Increment(ref fetchCount);
            return Task.FromResult(value);
        }

        // Act
        string first = await cache.GetOrAdd("a", () => Fetch("first"), Lifetime);
        string second = await cache.GetOrAdd("b", () => Fetch("second"), Lifetime);
        string firstAgain = await cache.GetOrAdd("a", () => Fetch("not used"), Lifetime);

        // Assert
        Assert.Equal("first", first);
        Assert.Equal("second", second);
        Assert.Equal("first", firstAgain);
        Assert.Equal(2, fetchCount);
    }

    [Fact]
    public async Task GetOrAdd_ExpiredEntry_FetchesAgain()
    {
        // Arrange
        var cache = new Cache<string, int>();
        int fetchCount = 0;

        Task<int> Fetch()
        {
            return Task.FromResult(Interlocked.Increment(ref fetchCount));
        }

        // Act: a lifetime of zero means the entry is already expired on the next call
        int first = await cache.GetOrAdd("key", Fetch, TimeSpan.Zero);
        int second = await cache.GetOrAdd("key", Fetch, TimeSpan.Zero);

        // Assert
        Assert.Equal(1, first);
        Assert.Equal(2, second);
    }

    [Fact]
    public async Task GetOrAdd_FetchFails_DoesNotCacheTheFailure()
    {
        // Arrange
        var cache = new Cache<string, int>();

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            cache.GetOrAdd("key", () => Task.FromException<int>(new InvalidOperationException()), Lifetime));

        int result = await cache.GetOrAdd("key", () => Task.FromResult(7), Lifetime);

        Assert.Equal(7, result);
    }
}
