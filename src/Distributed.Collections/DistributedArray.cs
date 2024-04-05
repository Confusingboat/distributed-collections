namespace Distributed.Collections;

public interface IDistributedArrayFactory
{
    IDistributedArray<T> Create<T>(string name, int length);
}

public interface IDistributedArray { }

public interface IDistributedArray<T> : IDistributedArray, IAsyncEnumerable<T>
{
    Task<T> GetValueAsync(int index);
    Task SetValueAsync(int index, T value);
    int Length { get; }
}

public class InMemoryDistributedArray<T>(int length) : IDistributedArray<T>
{
    private readonly T[] _array = new T[length];

    #region IDistributedArray<T> Members

    public Task<T> GetValueAsync(int index) => Task.FromResult(_array[index]);

    public Task SetValueAsync(int index, T value)
    {
        _array[index] = value;
        return Task.CompletedTask;
    }


    public int Length => _array.Length;

    #endregion

    #region IAsyncEnumerable<T> Members

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken()) =>
        _array
            .ToAsyncEnumerable()
            .GetAsyncEnumerator(cancellationToken);

    #endregion    
}