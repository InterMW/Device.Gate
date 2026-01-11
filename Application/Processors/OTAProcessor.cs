using Infrastructure.Rabbit;
using MQTTnet;

namespace Application.Processors;

public class OTAProcessor(MqttClientOptions clientOptions, IPlaneFramePublisher publisher,ILogger<OTAProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttClientFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
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
                    logger.LogInformation($"Received application message. {e.ApplicationMessage.ConvertPayloadToString()}");
                    logger.LogInformation($"  Received message on {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"  {DateTime.UtcNow}");
                    Console.WriteLine($"sending to {payload}");
                    mess.WithPayload("ota_good");
                    mess.WithTopic("todevice/"+payload);
                    var res = await mqttClient.PublishAsync(mess.Build());
                    return;
                case "fromdevice/alive":
                    logger.LogInformation($"{payload} checked in at {DateTime.UtcNow}");
                    return;
                case "fromdevice/died":
                    Console.WriteLine($"{payload} disconnected at {DateTime.UtcNow}");
                    return;
                case "fromdevice/plane":
                    try
                    {
                        var parts = payload.Split(":");
                        await publisher.SendMessage(parts[0], parts[1]);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    return;
            }
        };
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        while(mqttClient.IsConnected)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        Environment.Exit(-1);
    }

}
