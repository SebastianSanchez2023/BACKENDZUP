using ZuperBackend.Application.DTOs.Asset;

namespace ZuperBackend.Application.Services;

/// <summary>
/// Interfaz que define el contrato para operaciones de Activos (Assets).
/// 
/// PRINCIPIO: Una interfaz es como un "contrato" que dice:
/// "Cualquier clase que implemente esto DEBE tener estos métodos".
/// 
/// BENEFICIO: Permite cambiar la implementación sin cambiar el resto del código.
/// Si mañana quieres usar un Data Access Layer diferente, solo cambias la implementación.
/// </summary>
public interface IAssetService
{
    /// <summary>
    /// Obtiene todos los activos de un inquilino específico.
    /// 
    /// PARÁMETRO tenantId: Identificador del inquilino (multi-tenant).
    /// El servicio SIEMPRE filtra por TenantId para seguridad.
    /// 
    /// RETORNA: Lista de AssetDto (sin información interna/sensible)
    /// </summary>
    Task<IEnumerable<AssetDto>> GetAllAssetsByTenantAsync(Guid tenantId);

    /// <summary>
    /// Obtiene un activo específico por su ID.
    /// 
    /// PARÁMETRO tenantId: Valida que el asset pertenezca a este inquilino.
    /// PARÁMETRO assetId: El ID del asset a obtener.
    /// 
    /// RETORNA: AssetDto si existe, null si no existe o no pertenece al tenant.
    /// 
    /// SEGURIDAD: Aunque sea "public", verificamos que pertenezca al tenant correcto.
    /// </summary>
    Task<AssetDto?> GetAssetByIdAsync(Guid tenantId, Guid assetId);

    /// <summary>
    /// Crea un nuevo activo para un inquilino.
    /// 
    /// PARÁMETRUS:
    /// - tenantId: A qué inquilino pertenecerá este asset
    /// - createAssetDto: Los datos enviados por el cliente
    /// - createdByUserId: Quién está creando (para auditoría)
    /// 
    /// LÓGICA INTERNA:
    /// 1. Mapea CreateAssetDto → Asset (Entity)
    /// 2. Asigna TenantId, CreatedBy, timestamp
    /// 3. Guarda en BD
    /// 4. Retorna AssetDto con el ID generado
    /// 
    /// RETORNA: AssetDto del asset creado (con ID)
    /// </summary>
    Task<AssetDto> CreateAssetAsync(Guid tenantId, CreateAssetDto createAssetDto, Guid createdByUserId);

    /// <summary>
    /// Actualiza un activo existente.
    /// 
    /// PARÁMETRUS:
    /// - tenantId: Verifica que el asset pertenezca a este tenant
    /// - assetId: Cuál asset actualizar
    /// - updateAssetDto: Los nuevos valores
    /// - updatedByUserId: Quién está actualizando (para auditoría)
    /// 
    /// LÓGICA INTERNA:
    /// 1. Obtiene el asset actual de la BD
    /// 2. Valida que pertenezca al tenant (seguridad)
    /// 3. Actualiza solo los campos que vienen en el DTO
    /// 4. Actualiza UpdatedAt, UpdatedBy
    /// 5. Guarda cambios
    /// 
    /// RETORNA: AssetDto actualizado
    /// LANZA: Exception si no existe o no pertenece al tenant
    /// </summary>
    Task<AssetDto> UpdateAssetAsync(Guid tenantId, Guid assetId, UpdateAssetDto updateAssetDto, Guid updatedByUserId);

    /// <summary>
    /// "Borra" un activo (soft delete = marca como IsDeleted = true).
    /// 
    /// NOTA IMPORTANTE: Usamos "Soft Delete" no borrado REAL.
    /// Esto significa los datos siguen en base de datos pero marcados como eliminados.
    /// 
    /// BENEFICIO: Auditoría, recuperación accidental, integridad referencial.
    /// Si un incident hace referencia a este asset, no "desaparece" la referencia.
    /// 
    /// PARÁMETRUS:
    /// - tenantId: Verifica que pertenezca a este tenant
    /// - assetId: Cuál asset "borrar"
    /// - deletedByUserId: Quién está borrando (para auditoría)
    /// 
    /// RETORNA: true si se borró exitosamente
    /// LANZA: Exception si no existe
    /// </summary>
    Task<bool> DeleteAssetAsync(Guid tenantId, Guid assetId, Guid deletedByUserId);

    /// <summary>
    /// Obtiene el conteo de activos para un inquilino (para paginación, estadísticas, etc).
    /// 
    /// RETORNA: Número total de assets del tenant (activos, sin borrados lógicos)
    /// </summary>
    Task<int> GetAssetCountAsync(Guid tenantId);
}
