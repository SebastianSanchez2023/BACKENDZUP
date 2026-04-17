namespace ZuperBackend.Application.DTOs.Diagnosis;

/// <summary>
/// DTO para CREAR un nuevo nodo de diagnóstico (operación administrativa).
/// 
/// Lo usa el administrador cuando quiere agregar una nueva pregunta al árbol.
/// </summary>
public class CreateDiagnosisNodeDto
{
    /// <summary>Código único del nodo (ej: "POWER_CHECK")</summary>
    public required string NodeCode { get; set; }

    /// <summary>La pregunta que se mostrará al usuario</summary>
    public required string Question { get; set; }

    /// <summary>Texto de ayuda (opcional)</summary>
    public string? HelpText { get; set; }

    /// <summary>Orden de aparición</summary>
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// DTO para ACTUALIZAR un nodo existente.
/// 
/// Todos los campos son opcionales para permitir actualizaciones parciales.
/// </summary>
public class UpdateDiagnosisNodeDto
{
    /// <summary>Nueva pregunta (opcional)</summary>
    public string? Question { get; set; }

    /// <summary>Nuevo texto de ayuda (opcional)</summary>
    public string? HelpText { get; set; }

    /// <summary>Nuevo orden (opcional)</summary>
    public int? DisplayOrder { get; set; }

    /// <summary>¿Activar/desactivar el nodo? (opcional)</summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO para CREAR una nueva opción en un nodo.
/// 
/// Lo usa el administrador cuando agrega una respuesta a una pregunta.
/// </summary>
public class CreateDiagnosisOptionDto
{
    /// <summary>ID del nodo padre (a qué pregunta pertenece esta opción)</summary>
    public Guid DiagnosisNodeId { get; set; }

    /// <summary>Texto que se mostrará como opción</summary>
    public required string Label { get; set; }

    /// <summary>ID del siguiente nodo (si no es final)</summary>
    public Guid? NextNodeId { get; set; }

    /// <summary>Código de la acción ("RESOLVED", "ESCALATE", etc)</summary>
    public required string ActionCode { get; set; }

    /// <summary>Descripción de la acción</summary>
    public required string ActionDescription { get; set; }

    /// <summary>¿Es una acción final?</summary>
    public bool IsFinalAction { get; set; }

    /// <summary>Orden de aparición</summary>
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// DTO para ACTUALIZAR una opción existente.
/// </summary>
public class UpdateDiagnosisOptionDto
{
    /// <summary>Nuevo texto (opcional)</summary>
    public string? Label { get; set; }

    /// <summary>Nuevo siguiente nodo (opcional)</summary>
    public Guid? NextNodeId { get; set; }

    /// <summary>Nuevo código de acción (opcional)</summary>
    public string? ActionCode { get; set; }

    /// <summary>Nueva descripción de acción (opcional)</summary>
    public string? ActionDescription { get; set; }

    /// <summary>¿Cambiar si es final? (opcional)</summary>
    public bool? IsFinalAction { get; set; }

    /// <summary>Nuevo orden (opcional)</summary>
    public int? DisplayOrder { get; set; }

    /// <summary>¿Activar/desactivar? (opcional)</summary>
    public bool? IsActive { get; set; }
}
