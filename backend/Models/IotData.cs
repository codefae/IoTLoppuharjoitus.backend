using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class IotData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [property: BsonElement("deviceid")] 
    public required string DeviceId { get; set; }
    [property: BsonElement("temperature")] 
    public required double Temperature { get; set; }
    [property: BsonElement("timestamp")] 
    public required DateTime TimeStamp { get; set; }
}