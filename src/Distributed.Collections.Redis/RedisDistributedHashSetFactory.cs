using StackExchange.Redis;

namespace Distributed.Collections.Redis;

public class RedisDistributedHashSetFactory : IHashSetFactory
{
    private readonly IDatabase _database;

    public RedisDistributedHashSetFactory(IDatabase database) => _database = database;

    public IDistributedHashSet<T> Create<T>(string name) => new RedisDistributedHashSet<T>(_database, name);
}