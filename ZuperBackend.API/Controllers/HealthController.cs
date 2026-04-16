using Microsoft.AspNetCore.Mvc;

namespace ZuperBackend.API.Controllers;

/// <summary>
/// Controlador para pruebas y health check
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Verifica el estado de la API
    /// </summary>
    /// <returns>Estado de la API</returns>
    [HttpGet("check")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "API is running",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }
}
