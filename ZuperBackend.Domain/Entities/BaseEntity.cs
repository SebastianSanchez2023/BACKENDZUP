namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// Proporciona propiedades de auditoría comunes.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Fecha y hora de creación.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identificador del usuario que creó la entidad.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Fecha y hora de última modificación.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Identificador del usuario que modificó la entidad.
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Indica si la entidad ha sido eliminada lógicamente.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Fecha y hora de eliminación lógica.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
