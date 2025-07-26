using Microsoft.AspNetCore.Mvc;

namespace api1.Controllers;

[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet("HealthCheck")]
    public IActionResult Get() => Ok("Healthy");
}