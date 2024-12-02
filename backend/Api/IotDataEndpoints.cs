using backend.Services.Interfaces;
using MongoDB.Driver;

namespace backend.Api;

public static class IotDataEndpoints
{
    public static void ConfigureIotDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/IotData");
        
        group.MapGet("/Current/{deviceId}", async (
            string deviceId, 
            IIotDataService service) =>
        {
            try
            {
                var data = await service.GetLastDataAsync(deviceId);

                return data == null ? Results.NotFound($"No data found for device: {deviceId}") : Results.Ok(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Results.Problem("Something went wrong");
            }
        });
        
        group.MapGet("/average/{deviceId}/{hours}/{minutes}", async (
            string deviceId,
            int hours, 
            int minutes, 
            IIotDataService service) =>
        {
            try
            {
                var averages = await service.GetMinuteAveragesAsync(deviceId, hours, minutes);

                return averages.Count == 0 ? Results.NotFound($"No data found for device: {deviceId} int the last: {hours}h") : Results.Ok(averages);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Results.Problem("Something went wrong");
            }
        });
    }
}