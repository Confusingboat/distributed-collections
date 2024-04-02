using Distributed.Collections.Tests;
using Ephemerally;
using Ephemerally.Redis;
using Ephemerally.Redis.Xunit;

namespace Distributed.Collections.Redis.Tests;

[Collection(RedisTestCollection.Name)]
public class RedisStringDistributedHashSetTests(RedisCollectionFixture multiplexerFixture) :
    StringDistributedHashSetTests<RedisDistributedHashSet<string>>
{
    protected override Task<RedisDistributedHashSet<string>> CreateHashSet() => multiplexerFixture.HashSet<string>();
}

[Collection(RedisTestCollection.Name)]
public class RedisIntDistributedHashSetTests(RedisCollectionFixture multiplexerFixture) :
    IntDistributedHashSetTests<RedisDistributedHashSet<int>>
{
    protected override Task<RedisDistributedHashSet<int>> CreateHashSet() => multiplexerFixture.HashSet<int>();
}

internal class EphemeralRedisDistributedHashSet<T> : RedisDistributedHashSet<T>, IAsyncDisposable
{
    private readonly IEphemeralRedisDatabase _database;

    public EphemeralRedisDistributedHashSet(IEphemeralRedisDatabase database, string setKey) : base(database, setKey)
    {
        _database = database;
    }

    public EphemeralRedisDistributedHashSet(IEphemeralRedisDatabase database, IRedisSerializer serializer, string setKey) : base(database, serializer, setKey) { }

    public async ValueTask DisposeAsync() => await _database.DisposeAsync();
}

file static class Extensions
{
    public static Task<RedisDistributedHashSet<T>> HashSet<T>(this RedisMultiplexerFixture fixture) =>
        new EphemeralRedisDistributedHashSet<T>(
            fixture.Multiplexer.GetEphemeralDatabase(),
            Guid.NewGuid().ToString()
        ).ToTask<RedisDistributedHashSet<T>>();
}