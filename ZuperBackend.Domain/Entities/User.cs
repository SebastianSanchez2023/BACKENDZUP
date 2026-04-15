namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa un usuario en el sistema.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece el usuario.
    /// </summary>
    public required Guid TenantId { get; set; }

    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Correo electrónico único.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Hash de la contraseña (no se almacena en texto plano).
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Teléfono del usuario.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Rol del usuario en el sistema.
    /// Valores: Enterprise, SupportDesk, Technician, Admin
    /// </summary>
    public required string Role { get; set; }

    /// <summary>
    /// Indica si el usuario está activo.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha y hora del último acceso.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Token de refresh (para renovación de JWT).
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Fecha de expiración del refresh token.
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation properties
    /// <summary>
    /// Referencia al tenant.
    /// </summary>
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// Incidencias creadas por este usuario.
    /// </summary>
    public ICollection<Incident> CreatedIncidents { get; set; } = new List<Incident>();
}
