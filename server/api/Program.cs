using dataaccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(cfg =>
{
    cfg.Title = "FullstackIot API";
});

//read connection string from appsettings(.Development).json
var connStr = builder.Configuration.GetConnectionString("Db");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); // serves /swagger/v1/swagger.json

    app.UseSwaggerUi(cfg =>
    {
        cfg.Path = "/swagger";
        cfg.DocumentPath = "/swagger/v1/swagger.json";
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();