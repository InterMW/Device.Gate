using DomainService;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Translator;

namespace Application.Processors;

public class UsageProcessor(IJsonToObjectTranslator<UsageMessage> translator,
        ITranslatorDomainService domainService) : IStandardConsumer
{
    public async Task ConsumeMessageAsync(Message message, CancellationToken ct)
    {
        var name = translator.Translate(message).HostName;
        Console.WriteLine(name);

        await domainService.HandleHeartbeat(name);
    }
}

public class UsageMessage : StandardMessage
{
    public string HostName { get; set; } = "";
    public override string GetRoutingKey() => "";
}

