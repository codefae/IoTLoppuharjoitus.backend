namespace backend.MQTT.Interfaces;

public interface IMqttService : IDisposable
{
    Task EnsureConnectedAsync(CancellationToken cancellationToken);
    Task DisconnectFromMqttAsync();
}