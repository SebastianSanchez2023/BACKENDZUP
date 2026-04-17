namespace ZuperBackend.Application.DTOs.Incident;

/// <summary>
/// DTO para crear un nuevo incidente/reporte de problema.
/// El usuario reporta un problema relacionado a un activo.
/// </summary>
public class CreateIncidentDto
{
    /// <summary>
    /// ID del activo afectado (ej: la computadora que falla)
    /// </summary>
    public required Guid AssetId { get; set; }

    /// <summary>
    /// Categoría del problema (ej: "Hardware", "Software", "Conectividad", "Otro")
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Descripción detallada del problema
    /// que el usuario está experimentando.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Prioridad del incidente (ej: "Baja", "Media", "Alta", "Crítica")
    /// </summary>
    public string Priority { get; set; } = "Media";

    /// <summary>
    /// Horas estimadas para resolución
    /// Ayuda a gestionar expectativas del usuario.
    /// </summary>
    public int? EstimatedResolutionHours { get; set; }

    /// <summary>
    /// Coordenadas GPS si es un problema en terreno
    /// Formato: "lat,lng" ej: "-33.8688,151.2093"
    /// </summary>
    public string? LocationCoordinates { get; set; }

    /// <summary>
    /// Resultado del flujo de diagnóstico en JSON.
    /// Contiene: [{ nodeId, selectedOption }, { nodeId, selectedOption }]
    /// </summary>
    public string? DiagnosisPath { get; set; }

    /// <summary>
    /// Código de acción resultante del diagnóstico.
    /// Ej: "RESOLVED", "ESCALATE", "DOCUMENTATION"
    /// </summary>
    public string? DiagnosisResultAction { get; set; }
}
