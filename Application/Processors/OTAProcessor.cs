using MQTTnet;

namespace Application.Processors;

public class OTAProcessor(MqttClientOptions clientOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttClientFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        Console.WriteLine("We got there");
        await mqttClient.ConnectAsync(clientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("ota.check").Build();
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            Console.WriteLine($"Received application message. {e.ApplicationMessage.ConvertPayloadToString()}");
            await Task.CompletedTask;
            var payload = e.ApplicationMessage.ConvertPayloadToString();
            var mess = new MqttApplicationMessageBuilder();
            Console.WriteLine($"sending to ota.{payload}.ok");
            mess.WithPayload("Good");
            mess.WithTopic($"ota.{payload}.ok");
            var res = await mqttClient.PublishAsync(mess.Build());
        };
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

}
