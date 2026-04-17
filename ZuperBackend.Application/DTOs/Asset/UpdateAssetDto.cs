namespace ZuperBackend.Application.DTOs.Asset;

/// <summary>
/// DTO para actualizar un activo existente.
/// Todos los campos son opcionales - solo se actualizan los que se proporcionen.
/// </summary>
public class UpdateAssetDto
{
    /// <summary>
    /// Nuevo nombre del activo (opcional)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Nuevo número de serie (opcional)
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Nuevo tipo de activo (opcional)
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// Nuevo marca (opcional)
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Nuevo modelo (opcional)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Nueva ubicación (opcional)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Nueva descripción (opcional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Nueva fecha de adquisición (opcional)
    /// </summary>
    public DateTime? AcquisitionDate { get; set; }

    /// <summary>
    /// Nuevos metadatos en formato JSON (opcional)
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// ¿Marcar el activo como activo o inactivo?
    /// Útil cuando un equipo es retirado del servicio.
    /// </summary>
    public bool? IsActive { get; set; }
}
