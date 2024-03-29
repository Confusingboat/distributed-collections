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

public abstract class DistributedHashSetTests<THashSet, T> :
    IAsyncLifetime
    where THashSet : IDistributedHashSet<T>
{
    private bool _disposed;

    private readonly Lazy<Task<THashSet>> _hashSet;

    protected DistributedHashSetTests() => _hashSet = new(CreateHashSet);

    protected THashSet HashSet => _hashSet.Value.Result;


    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_should_add_item_when_it_does_not_exist()
    {
        // Arrange

        // Act
        var result = await HashSet.AddAsync(TestValues[0]);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task AddAsync_should_not_add_item_when_it_already_exists()
    {
        // Arrange
        await HashSet.AddAsync(TestValues[0]);

        // Act
        var result = await HashSet.AddAsync(TestValues[0]);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region RemoveAsync Tests

    [Fact]
    public async Task RemoveAsync_should_remove_item_when_it_exists()
    {
        // Arrange
        await HashSet.AddAsync(TestValues[0]);

        // Act
        var result = await HashSet.RemoveAsync(TestValues[0]);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task RemoveAsync_should_not_remove_item_when_it_does_not_exist()
    {
        // Arrange

        // Act
        var result = await HashSet.RemoveAsync(TestValues[0]);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region ContainsAsync Tests

    [Fact]
    public async Task ContainsAsync_should_return_true_when_item_exists()
    {
        // Arrange
        await HashSet.AddAsync(TestValues[0]);

        // Act
        var result = await HashSet.ContainsAsync(TestValues[0]);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ContainsAsync_should_return_false_when_item_does_not_exist()
    {
        // Arrange

        // Act
        var result = await HashSet.ContainsAsync(TestValues[0]);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region CountAsync Tests

    [Fact]
    public async Task CountAsync_should_return_0_when_hash_set_is_empty()
    {
        // Arrange

        // Act
        var result = await HashSet.CountAsync();

        // Assert
        result.ShouldBe(0);
    }

    [Fact]
    public async Task CountAsync_should_return_1_when_hash_set_has_one_item()
    {
        // Arrange
        await HashSet.AddAsync(TestValues[0]);

        // Act
        var result = await HashSet.CountAsync();

        // Assert
        result.ShouldBe(1);
    }

    #endregion

    #region LongCountAsync Tests

    [Fact]
    public async Task LongCountAsync_should_return_0_when_hash_set_is_empty()
    {
        // Arrange

        // Act
        var result = await HashSet.LongCountAsync();

        // Assert
        result.ShouldBe(0);
    }

    [Fact]
    public async Task LongCountAsync_should_return_1_when_hash_set_has_one_item()
    {
        // Arrange
        await HashSet.AddAsync(TestValues[0]);

        // Act
        var result = await HashSet.LongCountAsync();

        // Assert
        result.ShouldBe(1);
    }

    #endregion

    #region ClearAsync Tests

    [Fact]
    public async Task ClearAsync_should_remove_all_items()
    {
        // Arrange
        await HashSet.AddAsync(TestValues[0]);

        // Act
        await HashSet.ClearAsync();

        // Assert
        var count = await HashSet.CountAsync();
        count.ShouldBe(0);
    }

    #endregion

    protected abstract Task<THashSet> CreateHashSet();

    protected abstract T[] TestValues { get; }

    public async Task InitializeAsync() => await _hashSet.Value;

    public async Task DisposeAsync()
    {
        if (_disposed || !_hashSet.IsValueCreated) return;

        _disposed = true;

        await HashSet.TryDisposeAsync();
    }
}