namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa una opción de respuesta en un nodo de diagnóstico.
/// Cada opción puede llevar a otro nodo (continuando el árbol) o terminar con una acción (RESOLVED, ESCALATE, etc).
/// </summary>
public class DiagnosisOption : BaseEntity
{
    /// <summary>
    /// Identificador del nodo padre (la pregunta a la que pertenece esta opción).
    /// Foreign Key que conecta esta opción con su pregunta.
    /// </summary>
    public required Guid DiagnosisNodeId { get; set; }

    /// <summary>
    /// Referencia de navegación al nodo padre.
    /// Permite acceder desde DiagnosisOption a DiagnosisNode.
    /// </summary>
    public DiagnosisNode DiagnosisNode { get; set; } = null!;

    /// <summary>
    /// Texto que ve el usuario para esta opción.
    /// Ej: "Sí, enciende" o "No, no enciende"
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// Si esta opción NO es final, ¿a qué siguiente nodo lleva?
    /// Si es null, significa que esta es una acción final.
    /// </summary>
    public Guid? NextNodeId { get; set; }

    /// <summary>
    /// Referencia de navegación al siguiente nodo.
    /// Permite navegar al siguiente nivel del árbol.
    /// </summary>
    public DiagnosisNode? NextNode { get; set; }

    /// <summary>
    /// Código de acción si esta opción es final.
    /// Posibles valores: "RESOLVED", "ESCALATE", "DOCUMENTATION", "CONTACT_SUPPORT"
    /// </summary>
    public string? ActionCode { get; set; }

    /// <summary>
    /// Descripción de la acción que se ejecutará si se elige esta opción.
    /// Ej: "Se generará un ticket en Zendesk para soporte técnico"
    /// </summary>
    public string? ActionDescription { get; set; }

    /// <summary>
    /// ¿Esta opción termina el flujo de diagnóstico?
    /// true = no hay más preguntas, ejecutar ActionCode
    /// false = ir al NextNode para continuar con preguntas
    /// </summary>
    public bool IsFinalAction { get; set; } = false;

    /// <summary>
    /// Orden de visualización de las opciones.
    /// Permite controlar en qué orden se muestran las opciones al usuario.
    /// Ej: Opción 1 se muestra en posición 0, Opción 2 en posición 1, etc.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Indicador si esta opción está activa.
    /// Útil para deshabilitar opciones sin borrar datos históricos.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
