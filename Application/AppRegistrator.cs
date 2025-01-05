using Application.Processors;
using Device.GrpcClient;
using DomainService;
using Infrastructure.Rabbit;
using Infrastructure.Redis;
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.Rabbit;
using MelbergFramework.Infrastructure.Redis;

namespace Application;

public class AppRegistrator : Registrator
{
    public override void RegisterServices(IServiceCollection services)
    {
        RabbitModule.RegisterMicroConsumer<UsageProcessor,UsageMessage>(services, false);
        RabbitModule.RegisterPublisher<HeartbeatMessage>(services);
        services.AddTransient<IHeartbeatPublisher,HeartbeatPublisher>();
        services.AddTransient<ITranslatorDomainService,TranslatorDomainService>();
        DeviceGrpcDependencyModule.RegisterClient(services);
        RedisDependencyModule.LoadRedisRepository<INodeCacheRepository,NodeCacheRepository, NodeCacheContext>(services);
    }
}
