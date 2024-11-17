using backend.Models;
using backend.MQTT;
using backend.MQTT.Interfaces;
using backend.Repositories;
using backend.Repositories.Interfaces;
using backend.settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseSettings<IotData>>(builder.Configuration.GetSection("IotDataBaseSettings"));
builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));

builder.Services.AddSingleton<MqttMessageHandler>();
builder.Services.AddSingleton<IGenericMongoDbRepository<IotData>, GenericMongoDbRepository<IotData>>();
builder.Services.AddSingleton<IMqttService, MqttService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.StartMqttClient();

app.MapGet("/",
    async (IGenericMongoDbRepository<IotData> iotDataRepository) =>
    {
        return Results.Ok(await iotDataRepository.GetAllAsync());
    });

app.Run();