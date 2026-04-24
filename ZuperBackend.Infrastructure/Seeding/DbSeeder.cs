using System.Security.Cryptography;
using System.Text;
using ZuperBackend.Domain.Entities;
using ZuperBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ZuperBackend.Infrastructure.Seeding;

public static class DbSeeder
{
    // IDs Estáticos para consistencia
    private static readonly Guid StartNodeId = Guid.Parse("aaf96616-1e15-40b6-8b8d-1d84d951140d");
    private static readonly Guid SlownessNodeId = Guid.Parse("7692b86e-915e-4bb1-b914-f99d315117d4");
    
    private static readonly Guid OptionNoPowerId = Guid.Parse("3c6427ee-eb4f-4467-87ae-fbe9fd95988f");
    private static readonly Guid OptionBrokenScreenId = Guid.Parse("71c9385f-5f58-4601-b70c-506cedcf4f19");
    private static readonly Guid OptionLentoId = Guid.Parse("c22c127d-bbbc-4bfc-98c5-547298a213ac");
    
    private static readonly Guid OptionSlownessBootId = Guid.Parse("65904d9c-f851-41fd-a337-21ff842b6486");
    private static readonly Guid OptionSlownessNetId = Guid.Parse("dbe22c1a-b2b5-46c1-9e59-4919887dca33");

    public static readonly Guid AssetId = Guid.Parse("755b325e-d856-4775-9deb-006bebd96446");
    public static readonly Guid TenantId = Guid.Parse("89a88788-0adf-40d4-b9d3-806b201d5130");

    public static async Task SeedAsync(ZuperBackendDbContext dbContext)
    {
        // 1. Limpiar datos viejos si queremos consistencia total en pruebas
        // dbContext.DiagnosisOptions.RemoveRange(dbContext.DiagnosisOptions);
        // dbContext.DiagnosisNodes.RemoveRange(dbContext.DiagnosisNodes);
        // await dbContext.SaveChangesAsync();

        if (!await dbContext.DiagnosisNodes.AnyAsync(n => n.Id == StartNodeId))
        {
            var startNode = new DiagnosisNode
            {
                Id = StartNodeId,
                NodeCode = "START",
                Question = "¿Qué problema presenta el equipo?",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.DiagnosisNodes.Add(startNode);

            dbContext.DiagnosisOptions.Add(new DiagnosisOption
            {
                Id = OptionNoPowerId,
                DiagnosisNodeId = StartNodeId,
                Label = "No enciende",
                ActionDescription = "El equipo no responde al botón de encendido",
                DisplayOrder = 1,
                IsActive = true,
                IsFinalAction = true,
                ActionCode = "CREATE_TICKET_ZENDESK",
                CreatedAt = DateTime.UtcNow
            });

            dbContext.DiagnosisOptions.Add(new DiagnosisOption
            {
                Id = OptionBrokenScreenId,
                DiagnosisNodeId = StartNodeId,
                Label = "Pantalla rota/dañada",
                ActionDescription = "Daño físico visible en el panel",
                DisplayOrder = 2,
                IsActive = true,
                IsFinalAction = true,
                ActionCode = "CREATE_TICKET_ZENDESK",
                CreatedAt = DateTime.UtcNow
            });

            dbContext.DiagnosisOptions.Add(new DiagnosisOption
            {
                Id = OptionLentoId,
                DiagnosisNodeId = StartNodeId,
                Label = "Equipo lento",
                ActionDescription = "El rendimiento no es el óptimo",
                DisplayOrder = 3,
                IsActive = true,
                IsFinalAction = false,
                NextNodeId = SlownessNodeId,
                CreatedAt = DateTime.UtcNow
            });

            var slownessNode = new DiagnosisNode
            {
                Id = SlownessNodeId,
                NodeCode = "SLOWNESS_TYPE",
                Question = "¿Cuándo nota principalmente la lentitud?",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.DiagnosisNodes.Add(slownessNode);

            dbContext.DiagnosisOptions.Add(new DiagnosisOption
            {
                Id = OptionSlownessBootId,
                DiagnosisNodeId = SlownessNodeId,
                Label = "Al encender/arrancar",
                ActionDescription = "Posible problema de disco o programas de inicio",
                DisplayOrder = 1,
                IsActive = true,
                IsFinalAction = true,
                ActionCode = "CREATE_TICKET_ZENDESK",
                CreatedAt = DateTime.UtcNow
            });

            dbContext.DiagnosisOptions.Add(new DiagnosisOption
            {
                Id = OptionSlownessNetId,
                DiagnosisNodeId = SlownessNodeId,
                Label = "Navegando por Internet",
                ActionDescription = "Posible problema de red o navegador",
                DisplayOrder = 2,
                IsActive = true,
                IsFinalAction = true,
                ActionCode = "CREATE_TICKET_ZENDESK",
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Tenants.AnyAsync(t => t.Id == TenantId))
        {
            var tenant = new Tenant
            {
                Id = TenantId,
                Name = "Tenant de Prueba",
                Code = "TENANT-001",
                Email = "admin@tenant.com",
                IsActive = true,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Tenants.Add(tenant);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                TenantId = TenantId,
                FullName = "Administrador de Prueba",
                Email = "admin@test.com",
                PasswordHash = HashPassword("Admin@123456"),
                Role = "Admin",
                IsActive = true,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Users.Add(user);

            var asset = new Asset
            {
                Id = AssetId,
                TenantId = TenantId,
                Name = "Servidor de Prueba",
                Description = "Servidor para pruebas de QR",
                SerialNumber = "SN-001-TEST",
                AssetType = "Server",
                Location = "Data Center 1",
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Assets.Add(asset);

            var qrCode = new AssetQRCode
            {
                Id = Guid.NewGuid(),
                AssetId = AssetId,
                TenantId = TenantId,
                QRCode = "ASSET-SN-001-TEST-00001",
                IsActive = true,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.AssetQRCodes.Add(qrCode);

            await dbContext.SaveChangesAsync();
        }
    }

    private static string HashPassword(string password)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, new byte[16], 10000, HashAlgorithmName.SHA256))
        {
            byte[] salt = pbkdf2.Salt;
            byte[] hash = pbkdf2.GetBytes(32);
            return $"{Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }
    }
}
