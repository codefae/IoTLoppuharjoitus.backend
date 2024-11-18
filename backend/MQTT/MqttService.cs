using backend.MQTT.Interfaces;
using backend.settings;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace backend.MQTT;

public class MqttService : IMqttService
{
    private readonly IOptions<MqttSettings> _settings;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;

    public MqttService(
        IOptions<MqttSettings> settings,
        IMqttMessageHandler messageHandler,
        CancellationTokenSource cancellationTokenSource)
    {
        var cancellationToken = cancellationTokenSource.Token;
        var factory = new MqttClientFactory();

        _settings = settings;
        _mqttClient = factory.CreateMqttClient();
        _mqttOptions = new MqttClientOptionsBuilder()
            .WithClientId(settings.Value.ClientId)
            .WithTcpServer(settings.Value.MqttBrokerAddress, settings.Value.MqttBrokerPort)
            .Build();

        _mqttClient.ApplicationMessageReceivedAsync += async e
            => await messageHandler.HandleMessageAsync(e, cancellationToken).ConfigureAwait(false);
    }

    public async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (await _mqttClient.TryPingAsync(cancellationToken).ConfigureAwait(false))
                {
                    continue;
                }

                Console.WriteLine("Connecting to MQTT broker...");

                await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken).ConfigureAwait(false);
                
                Console.WriteLine("The MQTT client is connected.");
                attempt = 0;
                
                await SubscribeTopicsAsync(_settings.Value.Topics, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                attempt++;
                Console.WriteLine($"Failed to connect to MQTT broker with error: {e.Message}");
                Console.WriteLine($"Retrying in {_settings.Value.RetryWaitSeconds} seconds... ({attempt}th attempt)");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(_settings.Value.RetryWaitSeconds), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public async Task DisconnectFromMqttAsync()
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync(
                    new MqttClientDisconnectOptionsBuilder()
                        .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                        .Build())
                .ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _mqttClient.Dispose();
    }

    private async Task SubscribeTopicsAsync(string[] topics, CancellationToken cancellationToken)
    {
        foreach (var topic in topics)
        {
            var result = await _mqttClient.SubscribeAsync(
                    new MqttTopicFilterBuilder().WithTopic(topic).Build(),
                    cancellationToken)
                .ConfigureAwait(false);
            
            if (result.Items.First().ResultCode == MqttClientSubscribeResultCode.GrantedQoS2 ||
                result.Items.First().ResultCode == MqttClientSubscribeResultCode.GrantedQoS1 ||
                result.Items.First().ResultCode == MqttClientSubscribeResultCode.GrantedQoS0)
            {
                Console.WriteLine($"Subscribed to topic: {topic}");
            }
            else
            {
                Console.WriteLine($"Failed to subscribe to topic: {topic} with error: {result.Items.First().ResultCode}");
            }
        }
    }
}