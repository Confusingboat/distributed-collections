using Shouldly;
using System.Collections.Frozen;

namespace Distributed.Collections.Tests;

public abstract class StringDistributedHashSetTests<THashSet>
    : DistributedHashSetTests<THashSet, string>
    where THashSet : IDistributedHashSet<string>
{
    protected sealed override string[] TestValues => ["one"];
}

public abstract class IntDistributedHashSetTests<THashSet>
    : DistributedHashSetTests<THashSet, int>
    where THashSet : IDistributedHashSet<int>
{
    protected sealed override int[] TestValues => [1];
}

public abstract class DistributedHashSetTests<THashSet, T> where THashSet : IDistributedHashSet<T>
{
    [Fact]
    public async Task AddAsync_should_add_item_when_it_does_not_exist()
    {
        // Arrange
        var hashSet = await CreateHashSet();

        // Act
        var result = await hashSet.AddAsync(TestValues[0]);

        // Assert
        result.ShouldBeTrue();
    }

    protected abstract Task<THashSet> CreateHashSet();

    protected abstract T[] TestValues { get; }
}