namespace backend.settings;

public class MqttSettings
{
    public required int RetryWaitSeconds { get; set; }
    public required int MqttBrokerPort { get; set; }
    public required string ClientId { get; set; }
    public required string MqttBrokerAddress { get; set; }
    public required string[] Topics { get; set; }
}