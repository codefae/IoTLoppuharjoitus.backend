using backend.MQTT.Interfaces;

namespace backend.MQTT;

public static class ConfigureMqttClient
{
    public static void StartMqttClient(this WebApplication app, CancellationTokenSource cancellationTokenSource)
    {
        var cancellationToken = cancellationTokenSource.Token;
        var mqttService = app.Services.GetRequiredService<IMqttService>();
        var lifetime = app.Lifetime;
        
        lifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(() => mqttService.EnsureConnectedAsync(cancellationToken), cancellationToken);
        });
        
        lifetime.ApplicationStopping.Register(() =>
        {
            cancellationTokenSource.Cancel();
            mqttService.DisconnectFromMqttAsync().Wait();
            mqttService.Dispose();
        });
    }
}