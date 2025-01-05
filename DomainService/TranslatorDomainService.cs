using Device.GrpcClient;
using Domain;
using Infrastructure.Rabbit;
using Infrastructure.Redis;

namespace DomainService;

public interface ITranslatorDomainService
{
    Task HandleMessage(TemperatureMeasurement message);
    Task HandleHeartbeat(string serialNumber);
    //This will eventually take in an mqtt payload and figure out what's inside
    Task HandleMessage();
}

public class TranslatorDomainService(
        IDeviceGrpcClient deviceGrpcClient, 
        INodeCacheRepository nodeCacheRepository,
        IHeartbeatPublisher heartbeatPublisher) : ITranslatorDomainService
{
    public async Task HandleMessage(TemperatureMeasurement message)
    {
        var serialNumber = message.HostName;

        if(!await ConfirmNode(serialNumber))
        {
            return;
        }
    }

    public async Task HandleHeartbeat(string serialNumber)
    {

        if(!await ConfirmNode(serialNumber))
        {
            return;
        }

        await heartbeatPublisher.SendMessage(serialNumber);
    }

    private async Task<bool> ConfirmNode(string serialNumber)
    {
        if(!await nodeCacheRepository.Exists(serialNumber))
        {
            try
            {
                var device = await deviceGrpcClient.GetDeviceAsync(serialNumber);
            }
            catch (Device.Common.DeviceNotFoundException)
            {
                await deviceGrpcClient.CreateDeviceAsync(serialNumber);
                await nodeCacheRepository.MarkAsExists(serialNumber);
            }
            catch (Device.Common.DeviceSerialNumberInvalidException)
            {
                return false; 
            }
        }



        return true;
    }

    public Task HandleMessage()
    {
        throw new NotImplementedException();
    }
}
