namespace ZuperBackend.Application.DTOs.Incident;

/// <summary>
/// DTO para devolver los datos de un incidente al cliente.
/// Información completa pero sin datos sensibles internos.
/// </summary>
public class IncidentDto
{
    /// <summary>
    /// ID único del incidente
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID del activo afectado
    /// </summary>
    public Guid AssetId { get; set; }

    /// <summary>
    /// ID del tenant (empresa) que reportó
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID del usuario que reportó la incidencia
    /// </summary>
    public Guid? ReportedBy { get; set; }

    /// <summary>
    /// Categoría del problema
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del problema
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Prioridad actual
    /// </summary>
    public string Priority { get; set; } = "Media";

    /// <summary>
    /// Estado del incidente
    /// Ej: "Abierta", "En Progreso", "Resuelta", "Cerrada"
    /// </summary>
    public string Status { get; set; } = "Abierta";

    /// <summary>
    /// ID del ticket en Zendesk (sincronización)
    /// </summary>
    public string? ZendeskTicketId { get; set; }

    /// <summary>
    /// ID de la orden de trabajo en Zuper (sincronización)
    /// </summary>
    public string? ZuperWorkOrderId { get; set; }

    /// <summary>
    /// Notas de resolución (llenadas cuando se resuelve)
    /// </summary>
    public string? ResolutionNotes { get; set; }

    /// <summary>
    /// Cuándo se cerró (si aplica)
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Horas estimadas para resolución
    /// </summary>
    public int? EstimatedResolutionHours { get; set; }

    /// <summary>
    /// Ubicación GPS
    /// </summary>
    public string? LocationCoordinates { get; set; }

    /// <summary>
    /// Resultado del diagnóstico en JSON
    /// </summary>
    public string? DiagnosisPath { get; set; }

    /// <summary>
    /// Código de acción del diagnóstico
    /// </summary>
    public string? DiagnosisResultAction { get; set; }

    /// <summary>
    /// Cuándo se creó el incidente
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Cuándo se actualizó por última vez
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Cuánto costó resolver (en horas/recursos)
    /// Información para análisis posterior
    /// </summary>
    public int? ResolutionHours { get; set; }
}
