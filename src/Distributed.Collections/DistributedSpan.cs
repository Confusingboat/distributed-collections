namespace Distributed.Collections;

public readonly struct DistributedSpan<T>
{
    private readonly IDistributedArray<T> _array;
    private readonly int _start;
    private readonly int _length;

    public DistributedSpan(IDistributedArray<T> array, int start, int length)
    {
        _array = array;
        _start = start;
        _length = length;

        if (start < 0 || start >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(start));

        if (length < 0 || start + length > array.Length)
            throw new ArgumentOutOfRangeException(nameof(length));
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken()) =>
        _array
            .Skip(_start)
            .Take(_length)
            .GetAsyncEnumerator(cancellationToken);

    public async Task<T> GetValueAsync(int index) =>
        await _array.GetValueAsync(_start + index);

    public async Task SetValueAsync(int index, T value) =>
        await _array.SetValueAsync(_start + index, value);

    public int Length => _length;
}

public static class DistributedSpanExtensions
{
    public static DistributedSpan<T> ToDistributedSpan<T>(this IDistributedArray<T> array) =>
        new DistributedSpan<T>(array, 0, array.Length);

    public static DistributedSpan<T> Slice<T>(this IDistributedArray<T> array, int start, int length) =>
        new DistributedSpan<T>(array, start, length);
}