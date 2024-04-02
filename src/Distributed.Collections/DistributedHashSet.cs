using System.Collections.Concurrent;

namespace Distributed.Collections;

public interface IHashSetFactory
{
    IDistributedHashSet<T> Create<T>(string name);
}

public interface IDistributedHashSet { }

public interface IDistributedHashSet<T> : IDistributedHashSet, IAsyncEnumerable<T>
{
    Task<bool> AddAsync(T item);
    Task<bool> RemoveAsync(T item);
    Task<bool> ContainsAsync(T item);
    Task<int> CountAsync();
    Task<long> LongCountAsync();
    Task ClearAsync();
}

public class InMemoryDistributedHashSet<T> : IDistributedHashSet<T>
{
    private record Item(T Value);
    private class ItemComparer : IEqualityComparer<Item>
    {
        private readonly IEqualityComparer<T> _comparer;

        public ItemComparer(IEqualityComparer<T> comparer) => _comparer = comparer;

        public bool Equals(Item x, Item y) => _comparer.Equals(x.Value, y.Value);

        public int GetHashCode(Item obj) => _comparer.GetHashCode(obj.Value);
    }

    private readonly ConcurrentDictionary<Item, byte> _dictionary = new(new ItemComparer(EqualityComparer<T>.Default));

    #region IDistributedHashSet<T> Members

    public Task<bool> AddAsync(T item) =>
        Task.FromResult(_dictionary.TryAdd(new Item(item), byte.MinValue));

    public Task<bool> RemoveAsync(T item) =>
        Task.FromResult(_dictionary.TryRemove(new Item(item), out _));

    public Task<bool> ContainsAsync(T item) =>
        Task.FromResult(_dictionary.ContainsKey(new Item(item)));

    public Task<int> CountAsync() =>
        Task.FromResult(_dictionary.Count);

    public Task<long> LongCountAsync() =>
        Task.FromResult((long)_dictionary.Count);

    public Task ClearAsync()
    {
        _dictionary.Clear();
        return Task.CompletedTask;
    }

    #endregion

    #region IAsyncEnumerable<T> Members

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        _dictionary.Keys.Select(x => x.Value).ToAsyncEnumerable().GetAsyncEnumerator(cancellationToken);

    #endregion
}