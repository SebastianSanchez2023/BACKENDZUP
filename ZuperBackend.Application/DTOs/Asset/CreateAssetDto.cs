namespace ZuperBackend.Application.DTOs.Asset;

/// <summary>
/// DTO para crear un nuevo activo/equipo.
/// Contiene solo los datos que el cliente puede proporcionar.
/// El sistema genera automáticamente: Id, CreatedAt, CreatedBy, etc.
/// </summary>
public class CreateAssetDto
{
    /// <summary>
    /// Nombre descriptivo del activo (ej: "Computadora Despacho 1")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Número de serie del fabricante (ej: "ABC123XYZ")
    /// Debe ser único dentro del tenant.
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Tipo de activo (ej: "Computadora", "Servidor", "Impresora")
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// Marca/Fabricante (ej: "Dell", "HP", "Lenovo")
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Modelo específico (ej: "ThinkPad X1 Carbon")
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Ubicación física (ej: "Piso 2, Despacho 201")
    /// Útil para reportes de incidencias en terreno.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Descripción adicional (ej: "Monitor Dell 27 pulgadas conectado a computadora principal")
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Fecha de adquisición del equipo (ej: "2023-06-15").
    /// Útil para seguimiento de garantía y depreciación.
    /// </summary>
    public DateTime? AcquisitionDate { get; set; }

    /// <summary>
    /// JSON con metadatos adicionales específicos del activo.
    /// Ej: { "warranty_months": 24, "custom_field": "valor" }
    /// Ofrece flexibilidad para diferentes tipos de activos.
    /// </summary>
    public string? Metadata { get; set; }
}
