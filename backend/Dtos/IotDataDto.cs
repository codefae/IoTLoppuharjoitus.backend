using System.Diagnostics;

namespace backend.Dtos;

public class IotDataDto
{
    public required string DeviceId { get;set; }
    public required double Temperature { get; set; }

    public bool Validate() => this switch
    {
        { DeviceId.Length: < 1 or > 50 } => false,
        { Temperature: < -100 or > 100 } => false,
        _ => true
    };
}