using MQTTnet;

namespace backend.MQTT.Interfaces;

public interface IMqttMessageHandler
{
    Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs message, CancellationToken cancellationToken);
}