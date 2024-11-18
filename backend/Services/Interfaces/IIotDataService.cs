using backend.Dtos;
using backend.Models;

namespace backend.Services.Interfaces;

public interface IIotDataService
{
    Task<IotData?> GetLastDataAsync(string deviceId);
    Task<List<IotDataAverageDto>> GetMinuteAveragesAsync(string deviceId, int hours, int minutes);
    Task AddAsync(IotDataDto dto);
}