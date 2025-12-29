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
            .Build()
            .RunAsync();
        var mqttFactory = new MqttClientFactory();
        var tlsOptions = new MqttClientTlsOptionsBuilder().UseTls(false).Build();
        var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
           .WithTcpServer("10.0.0.5")
            .WithTlsOptions(tlsOptions)
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
            .WithCredentials("espnode","nonprod").Build();
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                Console.WriteLine($"Received application message. {e.ApplicationMessage.ConvertPayloadToString()}");

             await    thing(e.ApplicationMessage.ConvertPayloadToString());
            };
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);


        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("ota_sucess").Build();


        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

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
