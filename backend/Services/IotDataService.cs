using backend.Dtos;
using backend.Models;
using backend.Repositories.Interfaces;
using backend.Services.Interfaces;
using MongoDB.Driver;

namespace backend.Services;

public class IotDataService(IGenericMongoDbRepository<IotData> repository) : IIotDataService
{
    public async Task<IotData?> GetLastDataAsync(string deviceId)
    {
        var filter = Builders<IotData>.Filter.Eq(x => x.DeviceId, deviceId);
        var sort = Builders<IotData>.Sort.Descending(x => x.TimeStamp);

        var data = await repository.FindAsync(filter,sort);

        return data.Count <= 0 ? null : data.First();
    }
    public async Task<List<IotDataAverageDto>> GetMinuteAveragesAsync(string deviceId, int hours, int minutes)
    {
        var endDate = DateTime.Now - TimeSpan.FromHours(hours);
        var interval = TimeSpan.FromMinutes(minutes);
        var filter = Builders<IotData>.Filter.And(
            Builders<IotData>.Filter.Eq(x => x.DeviceId, deviceId),
            Builders<IotData>.Filter.Gt(x => x.TimeStamp, endDate)
        );

        var query = await repository.FindAsync(filter);

        var data = query.GroupBy(x => x.TimeStamp.Ticks / interval.Ticks)
            .Select(g => new IotDataAverageDto(
                Math.Round(g.Average(x => x.Temperature), 2),
                new DateTime(g.Key * interval.Ticks)))
            .ToList();

        return data;
    }

    public async Task AddAsync(IotDataDto iotDataDto)
    {
         await repository.AddAsync(new IotData()
        {
            DeviceId = iotDataDto.DeviceId,
            Temperature = iotDataDto.Temperature,
            TimeStamp = iotDataDto.TimeStamp
        }).ConfigureAwait(false);
    }
}