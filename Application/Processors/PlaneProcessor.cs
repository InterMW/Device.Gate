using DomainService;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Translator;

namespace Application.Processors;

public class PlaneProcessor(IJsonToObjectTranslator<TestMessage> translator,
        ITranslatorDomainService domainService) : IStandardConsumer
{
    public async Task ConsumeMessageAsync(Message message, CancellationToken ct)
    {
        Console.WriteLine(message.Body.Length);
    }
}
public class TestMessage : StandardMessage
{
    public string HostName { get; set; } = "";
    public override string GetRoutingKey() => "";
}
