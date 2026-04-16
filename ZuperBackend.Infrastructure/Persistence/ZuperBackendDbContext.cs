using Microsoft.EntityFrameworkCore;
using ZuperBackend.Domain.Entities;

namespace ZuperBackend.Infrastructure.Persistence;

/// <summary>
/// DbContext principal de la aplicación.
/// Configura todas las entidades y sus relaciones.
/// Soporta multi-tenant con filtrado automático por TenantId.
/// </summary>
public class ZuperBackendDbContext : DbContext
{
    public ZuperBackendDbContext(DbContextOptions<ZuperBackendDbContext> options)
        : base(options)
    {
    }

    // DbSets para cada entidad
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<AssetQRCode> AssetQRCodes { get; set; } = null!;
    public DbSet<Incident> Incidents { get; set; } = null!;
    public DbSet<IncidentAttachment> IncidentAttachments { get; set; } = null!;
    public DbSet<ExternalLink> ExternalLinks { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Tenant
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasMany(e => e.Users).WithOne(u => u.Tenant).HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Assets).WithOne(a => a.Tenant).HasForeignKey(a => a.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Incidents).WithOne(i => i.Tenant).HasForeignKey(i => i.TenantId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Email });
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.HasMany(e => e.CreatedIncidents).WithOne(i => i.ReportedByUser).HasForeignKey(i => i.ReportedBy).OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Asset
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.SerialNumber });
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SerialNumber).HasMaxLength(255);
            entity.HasMany(e => e.QRCodes).WithOne(q => q.Asset).HasForeignKey(q => q.AssetId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Incidents).WithOne(i => i.Asset).HasForeignKey(i => i.AssetId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de AssetQRCode
        modelBuilder.Entity<AssetQRCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.QRCode).IsUnique();
            entity.HasIndex(e => new { e.AssetId, e.IsActive });
            entity.Property(e => e.QRCode).IsRequired().HasMaxLength(500);
        });

        // Configuración de Incident
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.AssetId, e.Status });
            entity.HasIndex(e => e.ZendeskTicketId);
            entity.HasIndex(e => e.ZuperWorkOrderId);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(50);
            entity.HasMany(e => e.Attachments).WithOne(a => a.Incident).HasForeignKey(a => a.IncidentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.ExternalLinks).WithOne(el => el.Incident).HasForeignKey(el => el.IncidentId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de IncidentAttachment
        modelBuilder.Entity<IncidentAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IncidentId);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FilePath).IsRequired();
        });

        // Configuración de ExternalLink
        modelBuilder.Entity<ExternalLink>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.IncidentId, e.ExternalSystem }).IsUnique();
            entity.HasIndex(e => e.ExternalId);
            entity.Property(e => e.ExternalSystem).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ExternalId).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SyncDirection).IsRequired().HasMaxLength(50);
        });

        // Configuración de AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.CreatedAt });
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.Property(e => e.ActionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Result).IsRequired().HasMaxLength(50);
        });

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
