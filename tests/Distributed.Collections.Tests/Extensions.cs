﻿namespace Distributed.Collections.Tests;

public static class Extensions
{
    public static Task<T> ToTask<T>(this T value) => Task.FromResult(value);

    public static async ValueTask<bool> TryDisposeAsync<T>(this T self)
    {
        if (self is not IAsyncDisposable disposable)
            return false;

        await disposable.DisposeAsync().ConfigureAwait(false);
        return true;
    }

    public static bool TryDispose<T>(this T self)
    {
        if (self is not IDisposable disposable)
            return false;

        disposable.Dispose();
        return true;
    }
}