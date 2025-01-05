using MelbergFramework.Infrastructure.Redis;
using Microsoft.Extensions.Options;

namespace Infrastructure.Redis;

public interface INodeCacheRepository 
{
    Task<bool> Exists(string serialNumber);

    Task MarkAsExists(string serialNumber);
}

public class NodeCacheRepository : RedisRepository<NodeCacheContext>, INodeCacheRepository
{
    public NodeCacheRepository(NodeCacheContext context) : base(context) { }

    public Task<bool> Exists(string serialNumber) =>
         DB.KeyExistsAsync(CacheKey(serialNumber));

    public async Task MarkAsExists(string serialNumber)
    {
        var key = CacheKey(serialNumber);
        await DB.StringSetAsync(key,"here");
        await DB.KeyExpireAsync(key, new TimeSpan(0,5,0));
    }

    private static string CacheKey(string serialNumber) => $"device_gate_{serialNumber}";
}

public class NodeCacheContext : RedisContext
{
    public NodeCacheContext(
            IOptions<RedisConnectionOptions<NodeCacheContext>> options,
            IConnector connector) : base(options.Value, connector) { }
}
