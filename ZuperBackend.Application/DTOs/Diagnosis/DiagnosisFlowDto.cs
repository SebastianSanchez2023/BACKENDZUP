namespace ZuperBackend.Application.DTOs.Diagnosis;

/// <summary>
/// DTO que representa el resultado después de que el usuario selecciona una opción.
/// 
/// Se retorna cuando se procesa la selección de una opción.
/// Puede ser:
/// - Un nodo siguiente (si el árbol continúa)
/// - Una acción final (si se llegó al final)
/// </summary>
public class DiagnosisFlowResultDto
{
    /// <summary>¿Es esta una acción final? (¿Se termina el diagnóstico aquí?)</summary>
    public bool IsFinal { get; set; }

    /// <summary>
    /// El próximo nodo a presentar (null si IsFinal = true)
    /// </summary>
    public DiagnosisNodeDto? NextNode { get; set; }

    /// <summary>
    /// Información de la acción si es final (null si IsFinal = false)
    /// </summary>
    public DiagnosisActionResultDto? ActionResult { get; set; }
}

/// <summary>
/// DTO que representa una acción final en el diagnóstico.
/// 
/// Por ejemplo:
/// - "Se resolvió el problema: El equipo ahora funciona correctamente"
/// - "Se escaló a técnico: Un especialista te contactará en 2 horas"
/// </summary>
public class DiagnosisActionResultDto
{
    /// <summary>Tipo de acción: "RESOLVED", "ESCALATE", "DOCUMENTATION"</summary>
    public required string ActionCode { get; set; }

    /// <summary>Descripción de what happened / qué hacer</summary>
    public required string ActionDescription { get; set; }

    /// <summary>Mensaje para mostrar al usuario</summary>
    public required string UserMessage { get; set; }

    /// <summary>
    /// Datos adicionales (ej: referencia de ticket escalado, enlace a documentación)
    /// </summary>
    public Dictionary<string, object>? AdditionalData { get; set; }
}

/// <summary>
/// DTO para representar el árbol completo de diagnóstico.
/// 
/// Se usa cuando necesitas obtener toda la estructura para:
/// - Visualizar el árbol
/// - Auditoría
/// - Análisis
/// </summary>
public class DiagnosisTreeDto
{
    /// <summary>ID del nodo raíz</summary>
    public Guid RootNodeId { get; set; }

    /// <summary>Todos los nodos del árbol</summary>
    public IEnumerable<DiagnosisNodeDto>? AllNodes { get; set; }

    /// <summary>Todas las opciones del árbol</summary>
    public IEnumerable<DiagnosisOptionDto>? AllOptions { get; set; }

    /// <summary>Metadata del árbol</summary>
    public DiagnosisTreeMetadataDto? Metadata { get; set; }
}

/// <summary>
/// DTO con metadata del árbol (estadísticas, información general).
/// </summary>
public class DiagnosisTreeMetadataDto
{
    /// <summary>Total de nodos en el árbol</summary>
    public int TotalNodes { get; set; }

    /// <summary>Total de opciones/caminos</summary>
    public int TotalOptions { get; set; }

    /// <summary>Profundidad máxima del árbol</summary>
    public int MaxDepth { get; set; }

    /// <summary>Cuándo fue actualizado el árbol por última vez</summary>
    public DateTime LastUpdatedAt { get; set; }

    /// <summary>Quién actualizado el árbol por última vez (userId)</summary>
    public Guid LastUpdatedBy { get; set; }
}
