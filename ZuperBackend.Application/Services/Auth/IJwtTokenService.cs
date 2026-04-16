using System.Security.Claims;

namespace ZuperBackend.Application.Services.Auth;

/// <summary>
/// Servicio para generar y validar JWT tokens
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Genera un JWT token con los claims del usuario
    /// </summary>
    string GenerateAccessToken(Guid userId, string email, string role, Guid tenantId);

    /// <summary>
    /// Genera un Refresh token único
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Valida un JWT token y extrae los claims
    /// </summary>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Obtiene el tiempo de expiración en segundos del AccessToken
    /// </summary>
    int GetAccessTokenExpirationSeconds();
}
