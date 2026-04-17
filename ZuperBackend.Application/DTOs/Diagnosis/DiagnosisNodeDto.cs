namespace ZuperBackend.Application.DTOs.Diagnosis;

/// <summary>
/// DTO para representar un nodo del árbol de diagnóstico (lectura).
/// 
/// Se usa cuando el cliente CONSULTA un nodo para ver la pregunta y sus opciones.
/// </summary>
public class DiagnosisNodeDto
{
    /// <summary>Identificador único del nodo</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código único del nodo (ej: "START", "POWER_CHECK")
    /// 
    /// Útil para identificar nodos sin usar IDs, más legible
    /// </summary>
    public required string NodeCode { get; set; }

    /// <summary>
    /// La pregunta que se mostrará al usuario
    /// Ej: "¿Enciende el equipó?"
    /// </summary>
    public required string Question { get; set; }

    /// <summary>
    /// Texto de ayuda adicional (opcional)
    /// Ej: "Pulse el botón de encendido por 3 segundos"
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Orden en el que debe aparecer entre nodos hermanos
    /// (para casos donde hay múltiples preguntas iniciales)
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>¿Está activo? Si no, no debe aparecer en el árbol</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Timestamp de creación</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp de última actualización</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Lista de opciones (respuestas) disponibles para este nodo
    /// 
    /// Se incluye para que el cliente pueda renderizar todo de una vez
    /// </summary>
    public IEnumerable<DiagnosisOptionDto>? Options { get; set; }
}
