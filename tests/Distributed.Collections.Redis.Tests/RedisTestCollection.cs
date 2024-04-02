namespace Distributed.Collections.Redis.Tests;

[CollectionDefinition(Name)]
public class RedisTestCollection : ICollectionFixture<RedisCollectionFixture>
{
    public const string Name = "redis";
}