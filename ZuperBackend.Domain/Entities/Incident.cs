namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa una incidencia reportada por un usuario sobre un activo.
/// </summary>
public class Incident : BaseEntity
{
    /// <summary>
    /// Identificador del tenant.
    /// </summary>
    public required Guid TenantId { get; set; }

    /// <summary>
    /// Identificador del activo afectado.
    /// </summary>
    public required Guid AssetId { get; set; }

    /// <summary>
    /// Identificador del usuario que reportó la incidencia.
    /// </summary>
    public Guid? ReportedBy { get; set; }

    /// <summary>
    /// Categoría de la incidencia (hardware, software, conectividad, etc.).
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Descripción del problema reportado.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Prioridad de la incidencia (Baja, Media, Alta, Crítica).
    /// </summary>
    public string Priority { get; set; } = "Media";

    /// <summary>
    /// Estado interno de la incidencia.
    /// Estados: Abierta, EnProgreso, Resuelta, Cerrada, Cancelada
    /// </summary>
    public string Status { get; set; } = "Abierta";

    /// <summary>
    /// Referencia de ticket en Zendesk.
    /// </summary>
    public string? ZendeskTicketId { get; set; }

    /// <summary>
    /// Referencia de Orden de Trabajo en Zuper.
    /// </summary>
    public string? ZuperWorkOrderId { get; set; }

    /// <summary>
    /// Solución o resolución de la incidencia.
    /// </summary>
    public string? ResolutionNotes { get; set; }

    /// <summary>
    /// Fecha y hora de cierre de la incidencia.
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Tiempo estimado de resolución (en horas).
    /// </summary>
    public int? EstimatedResolutionHours { get; set; }

    /// <summary>
    /// Ubicación GPS (latitud, longitud - JSON).
    /// </summary>
    public string? LocationCoordinates { get; set; }

    // Navigation properties
    /// <summary>
    /// Referencia al tenant.
    /// </summary>
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// Referencia al activo afectado.
    /// </summary>
    public Asset? Asset { get; set; }

    /// <summary>
    /// Referencia al usuario que reportó.
    /// </summary>
    public User? ReportedByUser { get; set; }

    /// <summary>
    /// Adjuntos (evidencias, fotos) de la incidencia.
    /// </summary>
    public ICollection<IncidentAttachment> Attachments { get; set; } = new List<IncidentAttachment>();

    /// <summary>
    /// Vínculos externos con Zendesk y Zuper.
    /// </summary>
    public ICollection<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();
}
