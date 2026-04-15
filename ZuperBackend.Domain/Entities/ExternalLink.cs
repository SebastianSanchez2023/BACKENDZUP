namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa un vínculo externo con sistemas de terceros (Zendesk, Zuper).
/// Mantiene la trazabilidad entre incidencias internas y tickets/OT externos.
/// </summary>
public class ExternalLink : BaseEntity
{
    /// <summary>
    /// Identificador de la incidencia interna.
    /// </summary>
    public required Guid IncidentId { get; set; }

    /// <summary>
    /// Tipo de sistema externo (Zendesk, Zuper, etc.).
    /// </summary>
    public required string ExternalSystem { get; set; }

    /// <summary>
    /// Identificador único en el sistema externo.
    /// Ej: ticket_id para Zendesk, work_order_id para Zuper.
    /// </summary>
    public required string ExternalId { get; set; }

    /// <summary>
    /// Estado actual en el sistema externo.
    /// Ej: open, pending, resolved para Zendesk.
    /// </summary>
    public string? ExternalStatus { get; set; }

    /// <summary>
    /// URL de referencia al recurso externo.
    /// </summary>
    public string? ExternalUrl { get; set; }

    /// <summary>
    /// Metadatos adicionales del vínculo (JSON).
    /// Puede incluir detalles específicos del sistema externo.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Fecha y hora de la última sincronización.
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// Dirección de sincronización.
    /// Valores: OneWay (interno → externo), TwoWay (bidireccional)
    /// </summary>
    public string SyncDirection { get; set; } = "OneWay";

    /// <summary>
    /// Indica si está habilitada la sincronización automática.
    /// </summary>
    public bool IsSyncEnabled { get; set; } = true;

    // Navigation properties
    /// <summary>
    /// Referencia a la incidencia.
    /// </summary>
    public Incident? Incident { get; set; }
}
