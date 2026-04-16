using System.Security.Cryptography;
using System.Text;
using ZuperBackend.Domain.Entities;
using ZuperBackend.Infrastructure.Persistence;

namespace ZuperBackend.Infrastructure.Seeding;

/// <summary>
/// Servicio para sembrar datos iniciales en la BD
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Siembra datos iniciales si la BD está vacía
    /// </summary>
    public static async Task SeedAsync(ZuperBackendDbContext dbContext)
    {
        // Si ya hay tenants, no hacer nada
        if (dbContext.Tenants.Any())
            return;

        // Crear tenant de prueba
        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = "Tenant de Prueba",
            Code = "TENANT-001",
            Email = "admin@tenant.com",
            IsActive = true,
            CreatedBy = Guid.Empty,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Tenants.Add(tenant);

        // Crear usuario administrador de prueba
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            TenantId = tenantId,
            FullName = "Administrador de Prueba",
            Email = "admin@test.com",
            PasswordHash = HashPassword("Admin@123456"), // Contraseña: Admin@123456
            Role = "Admin",
            IsActive = true,
            CreatedBy = Guid.Empty,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);

        // Crear un activo de prueba
        var assetId = Guid.NewGuid();
        var asset = new Asset
        {
            Id = assetId,
            TenantId = tenantId,
            Name = "Servidor de Prueba",
            Description = "Servidor para pruebas de QR",
            SerialNumber = "SN-001-TEST",
            AssetType = "Server",
            Location = "Data Center 1",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Assets.Add(asset);

        // Crear código QR para el activo
        var qrCode = new AssetQRCode
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            QRCode = "ASSET-SN-001-TEST-00001", // En producción sería un QR real
            IsActive = true,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.AssetQRCodes.Add(qrCode);

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Genera un hash seguro de contraseña usando PBKDF2
    /// </summary>
    private static string HashPassword(string password)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            new byte[16], // Salt de 16 bytes (se genera aleatorio en producción)
            10000,
            HashAlgorithmName.SHA256))
        {
            byte[] salt = pbkdf2.Salt;
            byte[] hash = pbkdf2.GetBytes(32);

            // Formato: salt$hash (ambos en Base64)
            return $"{Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }
    }
}
