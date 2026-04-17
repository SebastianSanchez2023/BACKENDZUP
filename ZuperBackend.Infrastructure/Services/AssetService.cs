using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZuperBackend.Application.DTOs.Asset;
using ZuperBackend.Application.Services;
using ZuperBackend.Domain.Entities;
using ZuperBackend.Infrastructure.Persistence;

namespace ZuperBackend.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de Activos.
/// 
/// RESPONSABILIDADES:
/// 1. Traducir peticiones de DTOs a operaciones en BD
/// 2. Aplicar lógica de negocio (validaciones, security mulitenant, etc)
/// 3. Usar AutoMapper para convertir Entity ↔ DTO
/// 4. Usar DbContext para acceder a la base de datos
/// 5. Manejar errores y excepciones
/// 
/// ARQUITECTURA:
/// DTO (Cliente) → AssetService → AutoMapper → Entity → DbContext → BD
/// BD → Entity → AutoMapper → AssetService → DTO → Cliente
/// </summary>
public class AssetService : IAssetService
{
    private readonly ZuperBackendDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// 
    /// PARÁMETROS:
    /// - context: DbContext para acceder a la BD (inyectado por DI)
    /// - mapper: AutoMapper para convertir Entity ↔ DTO (inyectado por DI)
    /// 
    /// NOTA: Las dependencias vienen del contenedor DI en Program.cs
    /// Así permitimos que sean reemplazables con mocks en tests
    /// </summary>
    public AssetService(ZuperBackendDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene todos los assets de un tenant.
    /// 
    /// FLUJO:
    /// 1. Consulta BD: WHERE TenantId = tenantId AND !IsDeleted
    /// 2. Mapea cada Entity a DTO
    /// 3. Retorna la lista
    /// 
    /// SEGURIDAD: Filtro por TenantId asegura que cada tenant solo ve sus datos
    /// </summary>
    public async Task<IEnumerable<AssetDto>> GetAllAssetsByTenantAsync(Guid tenantId)
    {
        try
        {
            // Consulta la BD (acceso asincrónico para no bloquear threads)
            var assets = await Task.Run(() =>
                _context.Assets
                    .Where(a => a.TenantId == tenantId && !a.IsDeleted)
                    .OrderBy(a => a.CreatedAt)  // Ordenar por más reciente
                    .ToList()
            );

            // Mapea cada Entity → DTO usando AutoMapper
            return _mapper.Map<IEnumerable<AssetDto>>(assets);
        }
        catch (Exception ex)
        {
            // Log del error (en producción, usar Serilog)
            throw new ApplicationException($"Error al obtener assets del tenant {tenantId}", ex);
        }
    }

    /// <summary>
    /// Obtiene un asset específico por ID con validación de tenant.
    /// 
    /// FLUJO:
    /// 1. Busca el asset por ID
    /// 2. Valida que pertenezca al tenant (SEGURIDAD)
    /// 3. Valida que no esté eliminado (soft delete)
    /// 4. Mapea a DTO o retorna null
    /// 
    /// RETORNA: AssetDto si existe, null si no
    /// </summary>
    public async Task<AssetDto?> GetAssetByIdAsync(Guid tenantId, Guid assetId)
    {
        try
        {
            // Busca en BD (includes para cargar relaciones)
            var asset = await Task.Run(() =>
                _context.Assets
                    .FirstOrDefault(a => 
                        a.Id == assetId && 
                        a.TenantId == tenantId && 
                        !a.IsDeleted
                    )
            );

            // Si no existe, retorna null (el controller maneja esto)
            if (asset == null)
                return null;

            // Mapea a DTO
            return _mapper.Map<AssetDto>(asset);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener asset {assetId} del tenant {tenantId}", ex);
        }
    }

    /// <summary>
    /// Crea un nuevo asset.
    /// 
    /// FLUJO:
    /// 1. Mapea CreateAssetDto → Asset (Entity)
    /// 2. Asigna campos de sistema (TenantId, CreatedBy, CreatedAt, IsDeleted)
    /// 3. Valida que los datos sean correctos
    /// 4. Guardar en BD
    /// 5. Retorna AssetDto del asset creado (con ID generado)
    /// 
    /// PARÁMETROS:
    /// - tenantId: A qué tenant pertenece
    /// - createAssetDto: Datos del cliente
    /// - createdByUserId: ID del usuario que crea (para auditoría)
    /// 
    /// RETORNA: AssetDto con ID asignado
    /// </summary>
    public async Task<AssetDto> CreateAssetAsync(Guid tenantId, CreateAssetDto createAssetDto, Guid createdByUserId)
    {
        try
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(createAssetDto.Name))
                throw new ArgumentException("El nombre del asset es requerido");

            // Mapea DTO → Entity
            var asset = _mapper.Map<Asset>(createAssetDto);

            // Asigna campos de sistema
            asset.Id = Guid.NewGuid();  // Genera nuevo ID
            asset.TenantId = tenantId;   // Pertenece a este tenant
            asset.CreatedBy = createdByUserId;
            asset.CreatedAt = DateTime.UtcNow;
            asset.IsDeleted = false;

            // Agrega a DbContext (marca como "agregado, pendiente de guardar")
            _context.Assets.Add(asset);

            // Guarda en BD (ejecución asincrónica)
            await _context.SaveChangesAsync();

            // Mapea el resultado a DTO
            return _mapper.Map<AssetDto>(asset);
        }
        catch (DbUpdateException ex)
        {
            throw new ApplicationException("Error al guardar el asset en la base de datos", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al crear el asset", ex);
        }
    }

    /// <summary>
    /// Actualiza un asset existente.
    /// 
    /// FLUJO:
    /// 1. Obtiene el asset actual de BD
    /// 2. Valida que pertenezca al tenant (SEGURIDAD)
    /// 3. Actualiza solo los campos que vienen en el DTO
    /// 4. Asigna UpdatedBy, UpdatedAt
    /// 5. Guarda en BD
    /// 6. Retorna AssetDto actualizado
    /// 
    /// PARÁMETROS:
    /// - tenantId: Validación de tenant
    /// - assetId: Cuál asset actualizar
    /// - updateAssetDto: Nuevos valores (pueden ser parciales)
    /// - updatedByUserId: Quién actualiza (para auditoría)
    /// </summary>
    public async Task<AssetDto> UpdateAssetAsync(
        Guid tenantId, 
        Guid assetId, 
        UpdateAssetDto updateAssetDto, 
        Guid updatedByUserId)
    {
        try
        {
            // Obtiene el asset actual
            var asset = await Task.Run(() =>
                _context.Assets
                    .FirstOrDefault(a => 
                        a.Id == assetId && 
                        a.TenantId == tenantId && 
                        !a.IsDeleted
                    )
            );

            // Valida que exista
            if (asset == null)
                throw new KeyNotFoundException(
                    $"Asset {assetId} no encontrado en tenant {tenantId}");

            // Actualiza usando AutoMapper (mapea propiedades con valor, ignora nulls)
            _mapper.Map(updateAssetDto, asset);

            // Asigna campos de auditoría
            asset.UpdatedBy = updatedByUserId;
            asset.UpdatedAt = DateTime.UtcNow;

            // Actualiza en DbContext
            _context.Assets.Update(asset);

            // Guarda en BD
            await _context.SaveChangesAsync();

            // Retorna DTO actualizado
            return _mapper.Map<AssetDto>(asset);
        }
        catch (KeyNotFoundException)
        {
            throw;  // Re-lanza errores de validación
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al actualizar asset {assetId}", ex);
        }
    }

    /// <summary>
    /// Borra un asset (soft delete).
    /// 
    /// FLUJO:
    /// 1. Obtiene el asset
    /// 2. Valida que pertenezca al tenant
    /// 3. Marca IsDeleted = true
    /// 4. Asigna DeletedAt, DeletedBy
    /// 5. Guarda en BD
    /// 6. Retorna true
    /// 
    /// NOTA: El asset sigue en BD pero no se devuelve en queries normales
    /// </summary>
    public async Task<bool> DeleteAssetAsync(Guid tenantId, Guid assetId, Guid deletedByUserId)
    {
        try
        {
            // Obtiene el asset
            var asset = await Task.Run(() =>
                _context.Assets
                    .FirstOrDefault(a => 
                        a.Id == assetId && 
                        a.TenantId == tenantId && 
                        !a.IsDeleted
                    )
            );

            // Valida existencia
            if (asset == null)
                throw new KeyNotFoundException(
                    $"Asset {assetId} no encontrado o ya está eliminado");

            // Soft delete: marca como eliminado pero mantiene datos
            asset.IsDeleted = true;
            asset.DeletedAt = DateTime.UtcNow;
            asset.UpdatedBy = deletedByUserId;
            asset.UpdatedAt = DateTime.UtcNow;

            // Actualiza en BD
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al eliminar asset {assetId}", ex);
        }
    }

    /// <summary>
    /// Obtiene el conteo de assets de un tenant.
    /// 
    /// USO: Para paginación, estadísticas, etc.
    /// RETORNA: Total de assets no eliminados del tenant
    /// </summary>
    public async Task<int> GetAssetCountAsync(Guid tenantId)
    {
        try
        {
            return await Task.Run(() =>
                _context.Assets
                    .Count(a => a.TenantId == tenantId && !a.IsDeleted)
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al contar assets del tenant {tenantId}", ex);
        }
    }
}
