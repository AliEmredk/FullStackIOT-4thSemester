using dataaccess;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.Extensions;

DotNetEnv.Env.Load(); 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

app.UseHttpsRedirection();
app.MapControllers();

app.Run();