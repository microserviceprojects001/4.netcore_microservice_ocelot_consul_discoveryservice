using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class ConsulRegistrationService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly ConsulConfig _consulConfig;
    private readonly ILogger<ConsulRegistrationService> _logger;
    private string _serviceId;

    public ConsulRegistrationService(
        IConsulClient consulClient,
        IOptions<ConsulConfig> consulConfig,
        ILogger<ConsulRegistrationService> logger)
    {
        _consulClient = consulClient;
        _consulConfig = consulConfig.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _serviceId = $"{_consulConfig.ServiceName}-{Guid.NewGuid()}";

        var healthCheck = $"{_consulConfig.ServiceScheme}://{_consulConfig.ServiceAddress}:{_consulConfig.ServicePort}/HealthCheck";

        var serviceName = _consulConfig.ServiceName;

        Console.WriteLine($"Registering service {serviceName} with ID {_serviceId} at {healthCheck}");

        var registration = new AgentServiceRegistration
        {
            ID = _serviceId,
            Name = _consulConfig.ServiceName,
            Address = _consulConfig.ServiceAddress,
            Port = _consulConfig.ServicePort,
            Check = new AgentServiceCheck
            {
                HTTP = $"{_consulConfig.ServiceScheme}://{_consulConfig.ServiceAddress}:{_consulConfig.ServicePort}/HealthCheck",
                Interval = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5),
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
                //TLSSkipVerify = true, // 跳过证书验证
            }
        };

        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
        _logger.LogInformation($"Service {_consulConfig.ServiceName} registered with Consul");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken);
        _logger.LogInformation($"Service {_consulConfig.ServiceName} deregistered from Consul");
    }
}

public class ConsulConfig
{
    public string ServiceName { get; set; } = "service-name";
    public string ServiceAddress { get; set; } = "localhost";
    public int ServicePort { get; set; } = 5000;
    public string ServiceScheme { get; set; } = "http";
}