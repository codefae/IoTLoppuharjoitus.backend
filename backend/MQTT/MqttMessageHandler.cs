using System.Text;
using System.Text.Json;
using backend.Dtos;
using backend.MQTT.Interfaces;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using MQTTnet;

namespace backend.MQTT;

public class MqttMessageHandler(
    IIotDataService service,
    IHubContext<IotDataHub> hubContext) : IMqttMessageHandler
{
    public async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs message, CancellationToken cancellationToken)
    {
        var jsonPayload = Encoding.UTF8.GetString(message.ApplicationMessage.Payload);
        Console.WriteLine($"Received JSON payload: {jsonPayload}");

        try
        {
            var iotDataDto = JsonSerializer.Deserialize<IotDataDto>(jsonPayload);
            if (!iotDataDto.Validate())
            {
                Console.WriteLine("Invalid IoT data received.");
                return;
            }

            iotDataDto.TimeStamp = DateTime.Now;

            Console.WriteLine($"Received IoT data: {iotDataDto.Temperature}");

            await hubContext.Clients.All.SendAsync(
                "ReceiveIotData",
                iotDataDto,
                cancellationToken
            ).ConfigureAwait(false);

           await service.AddAsync(iotDataDto).ConfigureAwait(false);
        }
        catch (JsonException e)
        {
            Console.WriteLine($"JSON deserialization error: {e.Message}");
        }
        catch(MongoException e)
        {
            Console.WriteLine($"An error occurred while adding data to the database: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}