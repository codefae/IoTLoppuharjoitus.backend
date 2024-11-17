using backend.MQTT.Interfaces;

namespace backend.MQTT;

public static class ConfigureMqttClient
{
    public static void StartMqttClient(this WebApplication app)
    {
        var mqttService = app.Services.GetRequiredService<IMqttService>();
        var lifetime = app.Lifetime;
        
        lifetime.ApplicationStarted.Register(() => { mqttService.ConnectToMqttAsync().Wait(); });
        lifetime.ApplicationStopping.Register(() => { mqttService.DisconnectFromMqttAsync().Wait(); });
    }
}