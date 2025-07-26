using Consul;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<ConsulConfig>(builder.Configuration.GetSection("Consul"));

// 添加 Consul 客户端
builder.Services.AddSingleton<IConsulClient>(sp =>
    new ConsulClient(config =>
    {
        config.Address = new Uri("http://localhost:8500");
    }));

// 添加健康检查
builder.Services.AddHealthChecks();

// 添加 Consul 注册服务
builder.Services.AddHostedService<ConsulRegistrationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/HealthCheck");

app.Run();
