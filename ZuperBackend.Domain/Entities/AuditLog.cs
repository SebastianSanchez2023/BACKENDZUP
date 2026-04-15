namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Registra auditoría de acciones importantes en el sistema.
/// Proporciona trazabilidad completa para cumplimiento y análisis.
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// Identificador del tenant.
    /// </summary>
    public required Guid TenantId { get; set; }

    /// <summary>
    /// Identificador del usuario que realizó la acción.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Tipo de acción realizada (Create, Update, Delete, Sync, etc.).
    /// </summary>
    public required string ActionType { get; set; }

    /// <summary>
    /// Entidad que fue modificada (Incident, Asset, User, etc.).
    /// </summary>
    public required string EntityType { get; set; }

    /// <summary>
    /// Identificador de la entidad modificada.
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Descripción detallada de la acción.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Valores anteriores (JSON, para updates).
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// Valores nuevos (JSON, para updates).
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Dirección IP del usuario que realizó la acción.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent del navegador/cliente.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Resultado de la acción (Success, Failure, PartialSuccess).
    /// </summary>
    public string Result { get; set; } = "Success";

    /// <summary>
    /// Mensaje de error (si aplica).
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Tiempo en milisegundos que tardó la operación.
    /// </summary>
    public long? ExecutionTimeMs { get; set; }
}
