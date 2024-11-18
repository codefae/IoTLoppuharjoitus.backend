namespace backend.settings;

public class MqttSettings
{
    public required int RetryWaitSeconds { get; init; }
    public required int MqttBrokerPort { get; init; }
    public required string ClientId { get; init; }
    public required string MqttBrokerAddress { get; init; }
    public required string[] Topics { get; init; }
}