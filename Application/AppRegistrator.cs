using Application.Configuration;
using Application.Processors;
using DomainService;
using Infrastructure.Rabbit;
using Infrastructure.Redis;
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.Rabbit;
using MelbergFramework.Infrastructure.Redis;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Application;

public class AppRegistrator : Registrator
{
    public override void RegisterServices(IServiceCollection services)
    {
        // RabbitModule.RegisterMicroConsumer<PlaneProcessor,TestMessage>(services, false);
        // RabbitModule.RegisterMicroConsumer<UsageProcessor,UsageMessage>(services, false);
        RabbitModule.RegisterPublisher<PlaneFrameMessage>(services);
        services.AddTransient<IPlaneFramePublisher,PlaneFramePublisher>();
        // services.AddTransient<ITranslatorDomainService,TranslatorDomainService>();
        // DeviceGrpcDependencyModule.RegisterClient(services);
        // RedisDependencyModule.LoadRedisRepository<INodeCacheRepository,NodeCacheRepository, NodeCacheContext>(services);

        services.AddOptions<MqttOptions>()
            .BindConfiguration(MqttOptions.Section);
        services.AddSingleton<MqttClientOptions>(_ =>
                {
                    var option = _.GetService<IOptions<MqttOptions>>().Value;
                    var mqttFactory = new MqttClientFactory();
                    var tlsOptions = new MqttClientTlsOptionsBuilder().UseTls(false).Build();
                    var mqttClientOptions = new MqttClientOptionsBuilder()
                       .WithTcpServer(option.Host)
                        .WithTlsOptions(tlsOptions)
                        .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                        .WithCredentials(option.User, option.Password).Build();

                    return mqttClientOptions;
                });
        services.AddHostedService<OTAProcessor>();
    }
}
