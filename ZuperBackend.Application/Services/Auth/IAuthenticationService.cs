using ZuperBackend.Application.DTOs.Auth;

namespace ZuperBackend.Application.Services.Auth;

/// <summary>
/// Servicio para manejar la lógica de autenticación
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Autentica un usuario con email y contraseña
    /// </summary>
    Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
}
