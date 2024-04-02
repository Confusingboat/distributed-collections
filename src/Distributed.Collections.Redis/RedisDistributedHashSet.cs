using StackExchange.Redis;

namespace Distributed.Collections.Redis;

public class RedisDistributedHashSet<T> : IDistributedHashSet<T>
{
    private readonly IDatabase _database;
    private readonly IRedisSerializer _serializer;
    private readonly string _setKey;

    public RedisDistributedHashSet(IDatabase database, string setKey) : this(database, DefaultRedisSerializer.Instance, setKey) { }

    public RedisDistributedHashSet(IDatabase database, IRedisSerializer serializer, string setKey)
    {
        _database = database;
        _serializer = serializer;
        _setKey = setKey;
    }

    public async Task<bool> AddAsync(T item)
    {
        return await _database.SetAddAsync(_setKey, _serializer.Serialize(item));
    }

    public async Task<bool> RemoveAsync(T item)
    {
        return await _database.SetRemoveAsync(_setKey, _serializer.Serialize(item));
    }

    public async Task<bool> ContainsAsync(T item)
    {
        return await _database.SetContainsAsync(_setKey, _serializer.Serialize(item));
    }

    public async Task<int> CountAsync()
    {
        return (int)await _database.SetLengthAsync(_setKey);
    }

    public async Task<long> LongCountAsync()
    {
        return await _database.SetLengthAsync(_setKey);
    }

    public async Task ClearAsync()
    {
        await _database.KeyDeleteAsync(_setKey);
    }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    {
        await foreach (var item in _database.SetScanAsync(_setKey).WithCancellation(cancellationToken))
        {
            yield return _serializer.Deserialize<T>(item);
        }
    }
}