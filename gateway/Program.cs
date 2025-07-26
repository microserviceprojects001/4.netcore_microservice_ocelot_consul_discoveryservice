using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using System.Net;
using Ocelot.Middleware;



// ServicePointManager.ServerCertificateValidationCallback +=
//     (sender, certificate, chain, sslPolicyErrors) => true;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 添加 Ocelot 配置
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 添加 Ocelot 服务
builder.Services.AddOcelot()
    .AddConsul()
    .AddDelegatingHandler<BypassCertificateValidationHandler>(true);

builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.UseOcelot();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
