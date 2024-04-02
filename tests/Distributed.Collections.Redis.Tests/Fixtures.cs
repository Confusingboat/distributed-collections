using Distributed.Collections.Tests;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Ephemerally.Redis.Xunit;

namespace Distributed.Collections.Redis.Tests;

public class RedisCollectionFixture : PooledEphemeralRedisMultiplexerFixture<RedisTestContainerFixture>;

public class RedisTestContainerFixture : TestContainerFixture, IRedisInstanceFixture
{
    public ushort PublicPort => Container.GetMappedPublicPort(6379);

    public string ConnectionString => $"localhost:{PublicPort},allowAdmin=true";

    protected override IContainer CreateContainer() =>
        new ContainerBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
}