using System.Text.Json;
using StackExchange.Redis;

namespace Distributed.Collections.Redis;

public interface IRedisSerializer
{
    RedisValue Serialize<T>(T value);
    T Deserialize<T>(RedisValue redisValue);
}

internal class DefaultRedisSerializer : IRedisSerializer
{
    private static readonly Lazy<DefaultRedisSerializer> _instance = new(() => new DefaultRedisSerializer());

    public static DefaultRedisSerializer Instance => _instance.Value;

    private DefaultRedisSerializer() { }

    public RedisValue Serialize<T>(T value) => new(JsonSerializer.Serialize(value));

    public T Deserialize<T>(RedisValue redisValue) => redisValue switch
    {
        { HasValue: true, IsNull: false } => JsonSerializer.Deserialize<T>(redisValue.ToString()),
        _ => default
    };
}