using System.Diagnostics;
using MelbergFramework.Application;
using MQTTnet;

namespace Application;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await MelbergHost
            .CreateHost<AppRegistrator>()
            .DevelopmentPasswordReplacement("Mqtt:Password", "mqtt_pass")
            .DevelopmentPasswordReplacement("Mqtt:User", "mqtt_user")
            .DevelopmentPasswordReplacement("Mqtt:Host", "mqtt_host")
            .DevelopmentPasswordReplacement("Mqtt:Host", "mqtt_host")
            .DevelopmentPasswordReplacement("Rabbit:ClientDeclarations:Connections:0:Password", "rabbit_pass")
            .Build()
            .RunAsync();

        return;

    }

    private static async Task ConfirmOTA(string payload)
    {
        var mqttFactory = new MqttClientFactory();
        var tlsOptions = new MqttClientTlsOptionsBuilder().UseTls(false).Build();
        var mqttClientOptions = new MqttClientOptionsBuilder()
           // .WithTcpServer("10.0.0.215")
           .WithTcpServer("10.0.0.5")
            .WithTlsOptions(tlsOptions)
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
            .WithCredentials("espnode","nonprod").Build();

        var mqttClient = mqttFactory.CreateMqttClient();
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mess = new MqttApplicationMessageBuilder();
        mess.WithPayload("Good");
        mess.WithTopic($"ota.{payload}.ok");
        await mqttClient.PublishAsync(mess.Build());
    }

    private static async Task thing(string payload)
    {
        var mqttFactory = new MqttClientFactory();
        var tlsOptions = new MqttClientTlsOptionsBuilder().UseTls(false).Build();
        var mqttClientOptions = new MqttClientOptionsBuilder()
           // .WithTcpServer("10.0.0.215")
           .WithTcpServer("10.0.0.5")
            .WithTlsOptions(tlsOptions)
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
            .WithCredentials("espnode","nonprod").Build();

        var mqttClient = mqttFactory.CreateMqttClient();
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mess = new MqttApplicationMessageBuilder();
        mess.WithPayload("Good");
        mess.WithTopic($"ota.{payload}.ok");
        await mqttClient.PublishAsync(mess.Build());
    }
}
