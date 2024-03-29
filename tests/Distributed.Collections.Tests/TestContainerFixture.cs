using DotNet.Testcontainers.Containers;

namespace Distributed.Collections.Tests;

public abstract class TestContainerFixture : IAsyncLifetime
{
    private readonly Lazy<IContainer> _container;

    protected IContainer Container => _container.Value;

    protected TestContainerFixture()
    {
        _container = new(CreateContainer);
    }

    public Task InitializeAsync() => _container.Value.StartAsync();

    public Task DisposeAsync() => _container.TryDisposeAsync().AsTask();

    protected abstract IContainer CreateContainer();
}