namespace ZuperBackend.Application.DTOs.Auth;

/// <summary>
/// DTO para response de login.
/// Devuelto por el backend con los tokens.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// JWT Access Token (corta duración)
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh Token (larga duración, para renovar AccessToken)
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Tiempo de expiración del AccessToken en segundos
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Datos básicos del usuario autenticado
    /// </summary>
    public UserInfoDto User { get; set; } = null!;
}

/// <summary>
/// Información básica del usuario autenticado
/// </summary>
public class UserInfoDto
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public Guid TenantId { get; set; }
}
