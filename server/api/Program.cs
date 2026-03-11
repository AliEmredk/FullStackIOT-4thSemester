using System.Formats.Asn1;
using System.Text;
using api.Services;
using dataaccess;
using dataaccess.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mqtt.Controllers;
using StackExchange.Redis;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.Extensions;

DotNetEnv.Env.Load(); 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<MqttConnectHostedService>();


builder.Services.AddMqttControllers();
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddScoped<IWindmillTelemetryService, WindmillTelemetryService>();
builder.Services.AddScoped<IWindmillCommandService, WindmillCommandService>();
builder.Services.AddScoped<ITelemetryRealtimeService, TelemetryRealtimeService>();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("https://fullstackiot-web.fly.dev",
                "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// NSwag
builder.Services.AddOpenApiDocument(cfg => cfg.Title = "FullstackIot API");

// ---- Redis (StateleSSE backplane)
var redisConn =
    Environment.GetEnvironmentVariable("REDIS_CONNECTION")
    ?? "127.0.0.1:6379,abortConnect=false";

builder.Services.AddRedisSseBackplane(conf =>
{
    conf.RedisConnectionString = redisConn;
});

// ---- EF Realtime (StateleSSE + EF)
builder.Services.AddEfRealtime();

// ---- Postgres (Neon)
var connStr =
    builder.Configuration.GetConnectionString("Db")
    ?? Environment.GetEnvironmentVariable("CONN_STR")
    ?? throw new Exception("Missing DB connection string (ConnectionStrings:Db or CONN_STR)");

builder.Services.AddDbContext<AppDbContext>((sp, opt) =>
{
    opt.UseNpgsql(connStr);

   
    if (!builder.Environment.IsEnvironment("Migration"))
        opt.AddEfRealtimeInterceptor(sp);
});


var secret = builder.Configuration["Secret"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
var redisConnStr =
    Environment.GetEnvironmentVariable("REDIS_CONNECTION")
    ?? throw new Exception("REDIS_CONNECTION missing");

try
{
    var mux = await ConnectionMultiplexer.ConnectAsync(redisConnStr);
    if (mux.IsConnected)
        Console.WriteLine("✅ Redis connected successfully");
    else
        Console.WriteLine("❌ Redis NOT connected");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Redis connection failed:");
    Console.WriteLine(ex.Message);
}

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(cfg =>
    {
        cfg.Path = "/swagger";
        cfg.DocumentPath = "/swagger/v1/swagger.json";
    });
}

//FOR mqtt
var mqtt = app.Services.GetRequiredService<IMqttClientService>();

var mqttHost = builder.Configuration["Mqtt:Host"] ?? "broker.hivemq.com";
var mqttPort = int.Parse(builder.Configuration["Mqtt:Port"] ?? "1883");

_ = Task.Run(async () =>
{
    try
    {
        await mqtt.ConnectAsync(mqttHost, mqttPort);
        Console.WriteLine("✅ MQTT connected");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ MQTT connection failed: " + ex.Message);
    }
});

app.UseHttpsRedirection();

app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();