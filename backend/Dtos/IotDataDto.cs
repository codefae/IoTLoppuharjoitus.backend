namespace backend.Dtos;

public record struct IotDataDto(string DeviceId, double Temperature, DateTime TimeStamp)
{
    public bool Validate() => this switch
    {
        { DeviceId.Length: < 1 or > 50 } => false,
        { Temperature: < -100 or > 100 } => false,
        _ => true
    };
}