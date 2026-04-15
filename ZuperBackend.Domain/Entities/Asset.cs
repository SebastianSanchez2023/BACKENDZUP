namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa un activo (equipo o componente) en el sistema.
/// </summary>
public class Asset : BaseEntity
{
    /// <summary>
    /// Identificador del tenant propietario.
    /// </summary>
    public required Guid TenantId { get; set; }

    /// <summary>
    /// Nombre/descripción del activo.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Tipo de activo (máquina, componente, etc.).
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// Número de serie o identificador interno.
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Descripción adicional del activo.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ubicación del activo.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Marca del activo.
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Modelo del activo.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Fecha de adquisición.
    /// </summary>
    public DateTime? AcquisitionDate { get; set; }

    /// <summary>
    /// Metadatos adicionales (JSON).
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Indica si el activo está en servicio.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    /// <summary>
    /// Referencia al tenant.
    /// </summary>
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// Códigos QR asociados a este activo.
    /// </summary>
    public ICollection<AssetQRCode> QRCodes { get; set; } = new List<AssetQRCode>();

    /// <summary>
    /// Incidencias asociadas a este activo.
    /// </summary>
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
}
