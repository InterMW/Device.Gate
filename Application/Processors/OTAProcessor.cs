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

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("device/+").Build();
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var mess = new MqttApplicationMessageBuilder();
            Console.WriteLine($"Received application message. {e.ApplicationMessage.ConvertPayloadToString()}");
            Console.WriteLine($"  Received message on {e.ApplicationMessage.Topic}");
            Console.WriteLine($"  {DateTime.UtcNow}");
            var payload = e.ApplicationMessage.ConvertPayloadToString();
            switch(e.ApplicationMessage.Topic)
            {
                case "device/check":
                    Console.WriteLine($"sending to {payload}");
                    mess.WithPayload("ota_good");
                    mess.WithTopic("out/"+payload);
                    var res = await mqttClient.PublishAsync(mess.Build());
                    return;
                case "device/died":
                    Console.WriteLine($"{payload} disconnected");
                    return;
            }
        };
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

}
