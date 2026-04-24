using QRCoder;
using Microsoft.EntityFrameworkCore;
using ZuperBackend.Application.Services;
using ZuperBackend.Domain.Entities;
using ZuperBackend.Infrastructure.Persistence;

namespace ZuperBackend.Infrastructure.Services;

public class QRCodeService : IQRCodeService
{
    private readonly ZuperBackendDbContext _context;

    public QRCodeService(ZuperBackendDbContext context)
    {
        _context = context;
    }

    public async Task<AssetQRCode> GenerateQRCodeAsync(Guid tenantId, Guid assetId, Guid userId)
    {
        // 1. Verificar que el activo existe y pertenece al tenant
        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == assetId && a.TenantId == tenantId && !a.IsDeleted);

        if (asset == null)
            throw new KeyNotFoundException("Activo no encontrado");

        // 2. Desactivar QRs anteriores para este activo (si los hay)
        var existingQRs = await _context.AssetQRCodes
            .Where(q => q.AssetId == assetId && q.IsActive)
            .ToListAsync();

        foreach (var qr in existingQRs)
        {
            qr.IsActive = false;
            qr.DeactivatedAt = DateTime.UtcNow;
        }

        // 3. Crear el nuevo QR
        // El valor del QR será la URL de nuestra propia API
        string qrValue = $"http://localhost:5246/api/QRCodes/scan/{assetId}";

        var newQRCode = new AssetQRCode
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            QRCode = Guid.NewGuid().ToString("N"), // Valor interno único
            QRCodeUrl = qrValue,
            IsActive = true,
            ActivatedAt = DateTime.UtcNow,
            Version = (existingQRs.Count > 0) ? existingQRs.Max(q => q.Version) + 1 : 1,
            TenantId = tenantId,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.AssetQRCodes.Add(newQRCode);
        await _context.SaveChangesAsync();

        return newQRCode;
    }

    public async Task<AssetQRCode?> GetActiveQRCodeAsync(Guid tenantId, Guid assetId)
    {
        if (tenantId == Guid.Empty)
        {
            // Búsqueda pública (para el Scan)
            return await _context.AssetQRCodes
                .FirstOrDefaultAsync(q => q.AssetId == assetId && q.IsActive);
        }

        return await _context.AssetQRCodes
            .FirstOrDefaultAsync(q => q.AssetId == assetId && q.IsActive && q.TenantId == tenantId);
    }

    public string GenerateQRCodeImage(string content)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            return Convert.ToBase64String(qrCodeAsPngByteArr);
        }
    }
}
