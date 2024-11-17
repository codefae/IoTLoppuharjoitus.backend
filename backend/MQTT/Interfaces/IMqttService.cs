namespace backend.MQTT.Interfaces;

public interface IMqttService
{
    Task ConnectToMqttAsync();
    Task DisconnectFromMqttAsync();
}