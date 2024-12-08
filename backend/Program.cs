using backend.Api;
using backend.Models;
using backend.MQTT;
using backend.MQTT.Interfaces;
using backend.Repositories;
using backend.Repositories.Interfaces;
using backend.Services;
using backend.Services.Interfaces;
using backend.settings;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o => {});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(op => op.AddPolicy("CorsPolicy", 
    cp => cp.WithOrigins("http://localhost:5173")
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.Configure<DatabaseSettings<IotData>>(builder.Configuration.GetSection("IotDataBaseSettings"));
builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));

builder.Services.AddSingleton<CancellationTokenSource>();
builder.Services.AddSingleton<IMqttMessageHandler, MqttMessageHandler>();
builder.Services.AddSingleton<IGenericMongoDbRepository<IotData>, GenericMongoDbRepository<IotData>>();
builder.Services.AddSingleton<IIotDataService, IotDataService>();
builder.Services.AddSingleton<IMqttService, MqttService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.StartMqttClient(app.Services.GetRequiredService<CancellationTokenSource>());

app.MapHub<IotDataHub>("/IotDataHub");

app.ConfigureIotDataEndpoints();

app.Run();