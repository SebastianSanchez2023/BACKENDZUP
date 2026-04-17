namespace ZuperBackend.Application.DTOs.Diagnosis;

/// <summary>
/// DTO para representar una opción (respuesta) en el árbol de diagnóstico.
/// 
/// Cada nodo tiene múltiples opciones.
/// El usuario selecciona una opción, lo que lleva a:
/// a) Otro nodo (pregunta siguiente)
/// b) Una acción final (RESOLVED, ESCALATE, etc)
/// </summary>
public class DiagnosisOptionDto
{
    /// <summary>Identificador único de la opción</summary>
    public Guid Id { get; set; }

    /// <summary>Texto que se mostrará al usuario como opción</summary>
    public required string Label { get; set; }

    /// <summary>
    /// ID del siguiente nodo si el usuario selecciona esta opción
    /// 
    /// Si es null, significa que es una ACCIÓN FINAL (no hay más preguntas)
    /// </summary>
    public Guid? NextNodeId { get; set; }

    /// <summary>
    /// Código de la acción a ejecutar si es final
    /// Valores posibles: "RESOLVED", "ESCALATE", "DOCUMENTATION", etc
    /// </summary>
    public required string ActionCode { get; set; }

    /// <summary>
    /// Descripción de la acción
    /// Ej: Para ESCALATE sería "Se escalará a técnico especializado"
    /// Ej: Para RESOLVED sería "El problema ha sido resuelto"
    /// </summary>
    public required string ActionDescription { get; set; }

    /// <summary>
    /// ¿Es esta una acción terminal? (¿termina el árbol aquí?)
    /// True si ActionCode indica un fin (RESOLVED, ESCALATE, etc)
    /// </summary>
    public bool IsFinalAction { get; set; }

    /// <summary>Orden en el que aparecerá entre las opciones de este nodo</summary>
    public int DisplayOrder { get; set; }

    /// <summary>¿Está activa? Si no, no debe mostrarse</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Timestamp de creación</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp de última actualización</summary>
    public DateTime? UpdatedAt { get; set; }
}
