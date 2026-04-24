using ZuperBackend.Domain.Entities;

namespace ZuperBackend.Application.Services;

public interface IQRCodeService
{
    /// <summary>
    /// Genera o recupera un código QR para un activo específico.
    /// </summary>
    Task<AssetQRCode> GenerateQRCodeAsync(Guid tenantId, Guid assetId, Guid userId);

    /// <summary>
    /// Obtiene el código QR activo de un activo.
    /// </summary>
    Task<AssetQRCode?> GetActiveQRCodeAsync(Guid tenantId, Guid assetId);

    /// <summary>
    /// Genera la imagen del QR en formato Base64.
    /// </summary>
    string GenerateQRCodeImage(string content);
}
