using System.Net.Sockets;
using System.Text;
using backend.MQTT.Interfaces;
using backend.settings;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Exceptions;

namespace backend.MQTT;

public class MqttService : IMqttService
{
    private readonly int _retryWaitSeconds;
    private bool _isConnecting;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;

    public MqttService(IOptions<MqttSettings> settings, MqttMessageHandler messageHandler)
    {
        var factory = new MqttClientFactory();

        _mqttClient = factory.CreateMqttClient();
        _retryWaitSeconds = settings.Value.RetryWaitSeconds;
        _mqttOptions = new MqttClientOptionsBuilder()
            .WithClientId(settings.Value.ClientId)
            .WithTcpServer(settings.Value.MqttBrokerAddress, settings.Value.MqttBrokerPort)
            .Build();

        // Mqtt client event handlers
        _mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Connected to MQTT broker.");

            foreach (var topic in settings.Value.Topics)
            {
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build())
                    .ConfigureAwait(false);
            }
        };

        _mqttClient.DisconnectedAsync += async e =>
        {
            if (_isConnecting)
            {
                return;
            }

            Console.WriteLine("Disconnected from MQTT broker.");
            await Task.Delay(TimeSpan.FromSeconds(2));

            await ConnectToMqttAsync().ConfigureAwait(false);
        };

        _mqttClient.ApplicationMessageReceivedAsync += messageHandler.HandleMessageAsync;
    }

    public async Task ConnectToMqttAsync()
    {
        var attempt = 0;
        _isConnecting = true;

        while (_mqttClient.IsConnected == false)
        {
            try
            {
                Console.WriteLine("Connecting to MQTT broker...");
                await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
                
                await _mqttClient.ConnectAsync(_mqttOptions).ConfigureAwait(false);
            }
            catch (MqttCommunicationException e)
            {
                attempt++;
                
                Console.WriteLine(
                    $"Failed to connect to MQTT broker. " +
                    $"Retrying in {_retryWaitSeconds} seconds... ({attempt}th attempt)"
                );
                await Task.Delay(TimeSpan.FromSeconds(_retryWaitSeconds));
            }
        }

        _isConnecting = false;
    }

    public async Task DisconnectFromMqttAsync()
    {
        await _mqttClient.DisconnectAsync().ConfigureAwait(false);
    }
}