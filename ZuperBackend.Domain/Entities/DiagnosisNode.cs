namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa un nodo (pregunta) en el árbol de diagnóstico.
/// Cada nodo tiene múltiples opciones de respuesta que pueden llevar a otros nodos o a una acción final.
/// </summary>
public class DiagnosisNode : BaseEntity
{
    /// <summary>
    /// Código único del nodo para identificación.
    /// Ej: "power_check", "internet_check", "router_restart"
    /// </summary>
    public required string NodeCode { get; set; }

    /// <summary>
    /// La pregunta que se presenta al usuario.
    /// Ej: "¿La computadora enciende?"
    /// </summary>
    public required string Question { get; set; }

    /// <summary>
    /// Texto de ayuda adicional si el usuario necesita aclaraciones.
    /// Ej: "Revisa si el LED indicador está encendido"
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Orden de presentación. Permite controlar en qué secuencia se muestran los nodos.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Indicador si este nodo está activo o deshabilitado.
    /// Útil para mantener histórico de diagnósticos viejos sin eliminar datos.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Colección de opciones de respuesta disponibles para este nodo.
    /// Una pregunta puede tener 2 o más opciones (Sí/No, Rojo/Verde/Azul, etc).
    /// </summary>
    public List<DiagnosisOption> Options { get; set; } = new();
}
