using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Publishers;

namespace Infrastructure.Rabbit;

public interface IPlaneFramePublisher
{
    Task SendMessage(string serialNumber, string frame);
}

public class PlaneFramePublisher(IStandardPublisher<PlaneFrameMessage> publisher) : IPlaneFramePublisher
{
    public Task SendMessage(string serialNumber, string frame) 
    {
        publisher.Send(new PlaneFrameMessage(){SerialNumber = serialNumber, Frame = frame});
        return Task.CompletedTask;
    }
}

public class PlaneFrameMessage : StandardMessage
{
    public string SerialNumber {get; set;} = "";
    public string Frame {get; set;} = "";

    public override string GetRoutingKey() => $"node.frame";
}
