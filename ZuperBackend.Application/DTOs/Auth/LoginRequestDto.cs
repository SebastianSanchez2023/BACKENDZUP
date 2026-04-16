namespace ZuperBackend.Application.DTOs.Auth;

/// <summary>
/// DTO para request de login.
/// Enviado por el cliente Flutter.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Email del usuario
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Contraseña en texto plano (debe enviarse por HTTPS)
    /// </summary>
    public required string Password { get; set; }
}
