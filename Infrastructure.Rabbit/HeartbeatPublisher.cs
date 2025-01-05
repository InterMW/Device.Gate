using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Publishers;

namespace Infrastructure.Rabbit;

public interface IHeartbeatPublisher
{
    Task SendMessage(string serialNumber);
}

public class HeartbeatPublisher(IStandardPublisher<HeartbeatMessage> publisher) : IHeartbeatPublisher
{
    public Task SendMessage(string serialNumber) 
    {
        publisher.Send(new HeartbeatMessage(){SerialNumber = serialNumber});
        
        return Task.CompletedTask;
    }
}

public class HeartbeatMessage : StandardMessage
{
    public string SerialNumber {get; set;} = "";

    public override string GetRoutingKey() => $"node.heartbeat.{SerialNumber}";
}
