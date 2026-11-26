using System.Collections.Concurrent;

// Shared in-memory cache. One instance is used by every request at the same time
class Cache<TKey, TValue> where TKey : notnull
{
    private sealed record Entry(Lazy<Task<TValue>> Value, DateTime ExpiresAt);

    private readonly ConcurrentDictionary<TKey, Entry> _entries = new();

    // returns the stored value for the key, or runs fetch and stores its result
    public async Task<TValue> GetOrAdd(TKey key, Func<Task<TValue>> fetch, TimeSpan lifetime)
    {
        DateTime now = DateTime.UtcNow;

        // the first caller stores a new entry callers that arrive meanwhile get that same entry
        Entry entry = _entries.AddOrUpdate(
            key,
            _ => new Entry(new Lazy<Task<TValue>>(fetch), now + lifetime),
            (_, existing) => existing.ExpiresAt > now
                ? existing
                : new Entry(new Lazy<Task<TValue>>(fetch), now + lifetime)
        );

        try
        {
            // Lazy runs fetch only once, everyone else awaits the same task
            return await entry.Value.Value;
        }
        catch
        {
            // do not remember failures, the next call will try again
            _entries.TryRemove(new KeyValuePair<TKey, Entry>(key, entry));
            throw;
        }
    }
}
