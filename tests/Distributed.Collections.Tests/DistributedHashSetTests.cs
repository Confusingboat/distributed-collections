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
    #region AddAsync Tests

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

    [Fact]
    public async Task AddAsync_should_not_add_item_when_it_already_exists()
    {
        // Arrange
        var hashSet = await CreateHashSet();
        await hashSet.AddAsync(TestValues[0]);

        // Act
        var result = await hashSet.AddAsync(TestValues[0]);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region RemoveAsync Tests

    [Fact]
    public async Task RemoveAsync_should_remove_item_when_it_exists()
    {
        // Arrange
        var hashSet = await CreateHashSet();
        await hashSet.AddAsync(TestValues[0]);

        // Act
        var result = await hashSet.RemoveAsync(TestValues[0]);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task RemoveAsync_should_not_remove_item_when_it_does_not_exist()
    {
        // Arrange
        var hashSet = await CreateHashSet();

        // Act
        var result = await hashSet.RemoveAsync(TestValues[0]);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region ContainsAsync Tests

    [Fact]
    public async Task ContainsAsync_should_return_true_when_item_exists()
    {
        // Arrange
        var hashSet = await CreateHashSet();
        await hashSet.AddAsync(TestValues[0]);

        // Act
        var result = await hashSet.ContainsAsync(TestValues[0]);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ContainsAsync_should_return_false_when_item_does_not_exist()
    {
        // Arrange
        var hashSet = await CreateHashSet();

        // Act
        var result = await hashSet.ContainsAsync(TestValues[0]);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    protected abstract Task<THashSet> CreateHashSet();

    protected abstract T[] TestValues { get; }
}