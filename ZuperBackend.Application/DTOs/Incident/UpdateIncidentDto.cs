namespace ZuperBackend.Application.DTOs.Incident;

/// <summary>
/// DTO para actualizar un incidente existente.
/// Típicamente para cambiar estado, notas de resolución, asignar a técnico, etc.
/// </summary>
public class UpdateIncidentDto
{
    /// <summary>
    /// Nuevo estado del incidente (opcional)
    /// Ej: "En Progreso", "Resuelta", "Cerrada"
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Nueva prioridad (opcional)
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// Notas de resolución o comentarios técnicos (opcional)
    /// </summary>
    public string? ResolutionNotes { get; set; }

    /// <summary>
    /// Categoría del problema (si necesita cambiar)
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Descripción actualizada (opcional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Horas reales que tomó resolver (opcional)
    /// </summary>
    public int? ResolutionHours { get; set; }

    /// <summary>
    /// ¿Ha sido resuelto? Si es true, registra ClosedAt automáticamente
    /// </summary>
    public bool? MarkAsResolved { get; set; }

    /// <summary>
    /// ID del usuario técnico asignado (si aplica)
    /// </summary>
    public Guid? AssignedTechnicianId { get; set; }
}
