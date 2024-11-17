using System.Text;
using System.Text.Json;
using backend.Dtos;
using backend.Models;
using backend.Repositories.Interfaces;
using MQTTnet;

namespace backend.MQTT;

public class MqttMessageHandler(IGenericMongoDbRepository<IotData> iotDataRepository)
{
    public Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var jsonPayload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        Console.WriteLine($"Received JSON payload: {jsonPayload}");
        
        try
        {
            var iotDataDto = JsonSerializer.Deserialize<IotDataDto>(jsonPayload) 
                             ?? throw new JsonException("Invalid JSON payload.");
            if (!iotDataDto.Validate())
            {
                Console.WriteLine("Invalid IoT data received.");
                return Task.CompletedTask;
            };
            
            iotDataRepository.AddAsync(new IotData()
            {
                DeviceId = iotDataDto.DeviceId,
                Temperature = iotDataDto.Temperature,
                TimeStamp = DateTime.Now
            });
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        
        return Task.CompletedTask;
    }
}