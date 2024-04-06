using StackExchange.Redis;

namespace Distributed.Collections.Redis;

public class RedisDistributedArray<T> : IDistributedArray<T>
{
    private const string ArrayMetadataFieldName = "_metadata";

    private readonly IDatabase _database;
    private readonly IRedisSerializer _serializer;
    private readonly string _arrayKey;
    private readonly long _length;

    private bool _initialized;

    public RedisDistributedArray(IDatabase database, string arrayKey, long length)
        : this(database, DefaultRedisSerializer.Instance, arrayKey, length)
    {
    }

    public RedisDistributedArray(IDatabase database, IRedisSerializer serializer, string arrayKey, long length)
    {
        _database = database;
        _serializer = serializer;
        _arrayKey = arrayKey;
        _length = length;
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken()) =>
        _database.HashScanAsync(_arrayKey)
            .Select(entry => _serializer.Deserialize<T>(entry.Value))
            .GetAsyncEnumerator(cancellationToken);

    public async Task<T> GetValueAsync(int index) =>
        _serializer.Deserialize<T>(
            await _database.HashGetAsync(
                _arrayKey,
                index.ToString()));

    public async Task SetValueAsync(int index, T value) =>
        await _database.HashSetAsync(
            _arrayKey,
            index.ToString(),
            _serializer.Serialize(value));

    public int Length => (int)_length;

    public long LongLength => _length;

    private ValueTask EnsureInitializedAsync() =>
        _initialized
            ? ValueTask.CompletedTask
            : new ValueTask(TryInitializeAsync());

    private async Task TryInitializeAsync()
    {
        var metadata = await GetMetadataAsync();
        if (metadata.Initialized)
        {
            _initialized = true;
            return;
        }

        if (await _database.HashSetAsync(
                _arrayKey,
                ArrayMetadataFieldName,
                _serializer.Serialize(new ArrayMetadata(_length, true)),
                when: When.NotExists))
        {
            _initialized = true;
            return;
        }

        metadata = await GetMetadataAsync();

        if (metadata.Length != _length)
        {
            throw new InvalidOperationException("Array length mismatch.");
        }

        _initialized = true;
    }

    private async Task<ArrayMetadata> GetMetadataAsync() =>
        _serializer.Deserialize<ArrayMetadata>(
            await _database.HashGetAsync(_arrayKey, ArrayMetadataFieldName));

    private record struct ArrayMetadata(long Length, bool Initialized);

    private T Deserialize(RedisValue value) =>
        _serializer.Deserialize<T>(value);

    private RedisValue Serialize(T value) =>
        _serializer.Serialize(value);
}

public record struct RedisArrayReference(string ParentArrayKey, string ArrayKey, long Length);