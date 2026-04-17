namespace ZuperBackend.Application.DTOs.Asset;

/// <summary>
/// DTO para devolver los datos de un activo al cliente.
/// Incluye información del sistema (Id, CreatedAt, etc) pero solo lo que es seguro exponer.
/// NO incluye información sensible como auditoría detallada.
/// </summary>
public class AssetDto
{
    /// <summary>
    /// ID único del activo
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre del activo
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Número de serie
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Tipo de activo
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// Marca del fabricante
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Modelo del equipo
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Ubicación física
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Descripción adicional
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Fecha de adquisición
    /// </summary>
    public DateTime? AcquisitionDate { get; set; }

    /// <summary>
    /// Metadatos en JSON
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// ¿El activo está activo?
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Cuándo se creó este registro
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Cuándo se actualizó por última vez
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// ID del usuario que creó el registro
    /// </summary>
    public Guid CreatedBy { get; set; }
}
