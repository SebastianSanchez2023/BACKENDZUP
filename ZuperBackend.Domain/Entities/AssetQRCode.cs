namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa la asociación entre un Activo y sus códigos QR.
/// Soporta rotación y control de reimpresión de QRs.
/// </summary>
public class AssetQRCode : BaseEntity
{
    /// <summary>
    /// Identificador del activo.
    /// </summary>
    public required Guid AssetId { get; set; }

    /// <summary>
    /// Valor único del código QR (puede ser un UUID, hash o ID personalizado).
    /// </summary>
    public required string QRCode { get; set; }

    /// <summary>
    /// Sitio web o endpoint al que resuelve el QR (puede contener parámetros).
    /// </summary>
    public string? QRCodeUrl { get; set; }

    /// <summary>
    /// Firma o checksum del QR para validar integridad (si aplica).
    /// </summary>
    public string? QRCodeSignature { get; set; }

    /// <summary>
    /// Indica si este QR está activo para escaneo.
    /// Permite rotación de QRs sin perder historial.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de activación de este QR.
    /// </summary>
    public DateTime? ActivatedAt { get; set; }

    /// <summary>
    /// Fecha de desactivación de este QR (cuando se rota a uno nuevo).
    /// </summary>
    public DateTime? DeactivatedAt { get; set; }

    /// <summary>
    /// Versión del QR (para control de cambios).
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Número de veces que este QR ha sido escaneado.
    /// </summary>
    public int ScanCount { get; set; } = 0;

    /// <summary>
    /// Última fecha de escaneo.
    /// </summary>
    public DateTime? LastScannedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Referencia al activo.
    /// </summary>
    public Asset? Asset { get; set; }
}
