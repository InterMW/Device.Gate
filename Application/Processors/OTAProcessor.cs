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

        //     var mess = new MqttApplicationMessageBuilder();
        //             mess.WithPayload("restart");
        //             mess.WithTopic("todevice/30EDA0E2549E");
        //             var res = await mqttClient.PublishAsync(mess.Build());
        // return;
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("fromdevice/+").Build();
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var mess = new MqttApplicationMessageBuilder();
            var payload = e.ApplicationMessage.ConvertPayloadToString();
            switch(e.ApplicationMessage.Topic)
            {
                case "fromdevice/check":
                    Console.WriteLine($"Received application message. {e.ApplicationMessage.ConvertPayloadToString()}");
                    Console.WriteLine($"  Received message on {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"  {DateTime.UtcNow}");
                    Console.WriteLine($"sending to {payload}");
                    mess.WithPayload("ota_good");
                    mess.WithTopic("todevice/"+payload);
                    var res = await mqttClient.PublishAsync(mess.Build());
                    return;
                case "fromdevice/alive":
                    Console.WriteLine($"{payload} checked in at {DateTime.UtcNow}");
                    return;
                case "fromdevice/died":
                    Console.WriteLine($"{payload} disconnected at {DateTime.UtcNow}");
                    return;
                case "fromdevice/plane":
                    return;
            }
        };
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

}
