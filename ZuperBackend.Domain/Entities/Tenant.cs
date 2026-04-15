namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa a una empresa (Tenant) en el sistema.
/// Soporta multi-tenant isolation.
/// </summary>
public class Tenant : BaseEntity
{
    /// <summary>
    /// Nombre de la empresa.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Identificador único de la empresa (para uso interno/external).
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Correo de contacto principal.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Teléfono de contacto.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Dirección de la empresa.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Configuración de credenciales de Zendesk (JSON serializado).
    /// </summary>
    public string? ZendeskConfig { get; set; }

    /// <summary>
    /// Configuración de credenciales de Zuper (JSON serializado).
    /// </summary>
    public string? ZuperConfig { get; set; }

    /// <summary>
    /// Indica si el tenant está activo.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Relación con usuarios de la empresa.
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Relación con activos de la empresa.
    /// </summary>
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();

    /// <summary>
    /// Relación con incidencias reportadas en la empresa.
    /// </summary>
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
}
