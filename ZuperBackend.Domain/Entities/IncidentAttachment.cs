namespace ZuperBackend.Domain.Entities;

/// <summary>
/// Representa un adjunto (foto, archivo de evidencia) asociado a una incidencia.
/// </summary>
public class IncidentAttachment : BaseEntity
{
    /// <summary>
    /// Identificador de la incidencia asociada.
    /// </summary>
    public required Guid IncidentId { get; set; }

    /// <summary>
    /// Nombre del archivo.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Tipo MIME del archivo (image/jpeg, image/png, etc.).
    /// </summary>
    public required string MimeType { get; set; }

    /// <summary>
    /// Ruta o URI de almacenamiento del archivo.
    /// Puede ser ruta local o URL de blob storage.
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// Tamaño del archivo en bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Descripción del adjunto.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tipo de adjunto (Foto, Documento, Diagnóstico, etc.).
    /// </summary>
    public string? AttachmentType { get; set; }

    /// <summary>
    /// URL pública del adjunto (si aplica).
    /// </summary>
    public string? PublicUrl { get; set; }

    // Navigation properties
    /// <summary>
    /// Referencia a la incidencia.
    /// </summary>
    public Incident? Incident { get; set; }
}
