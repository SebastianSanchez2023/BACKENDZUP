using Microsoft.AspNetCore.Mvc;
using ZuperBackend.Application.DTOs.Auth;
using ZuperBackend.Application.Services.Auth;

namespace ZuperBackend.API.Controllers;

/// <summary>
/// Controlador para manejar autenticación y autorización
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Endpoint de login - Autentica un usuario y devuelve JWT token
    /// </summary>
    /// <param name="request">Email y contraseña del usuario</param>
    /// <returns>Token de acceso, refresh token y datos del usuario</returns>
    /// <response code="200">Login exitoso, devuelve tokens</response>
    /// <response code="401">Credenciales inválidas</response>
    /// <response code="400">Validación fallida</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // Validación básica
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email y contraseña son requeridos" });
        }

        // Intentar autenticación
        var response = await _authenticationService.AuthenticateAsync(request);

        if (response == null)
        {
            return Unauthorized(new { message = "Email o contraseña inválidos" });
        }

        return Ok(response);
    }

    /// <summary>
    /// Endpoint de validación - Verifica si el token es válido
    /// Endpoint de prueba para Flutter
    /// </summary>
    /// <returns>Confirmación de validez del token</returns>
    /// <response code="200">Token válido</response>
    /// <response code="401">Token inválido o expirado</response>
    [HttpGet("validate")]
    public IActionResult ValidateToken()
    {
        // Este endpoint requiere autenticación JWT en el header
        // Si llega aquí, significa que el token es válido
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        var tenantId = User.FindFirst("TenantId")?.Value;

        return Ok(new
        {
            message = "Token válido",
            userId,
            tenantId,
            timestamp = DateTime.UtcNow
        });
    }
}
