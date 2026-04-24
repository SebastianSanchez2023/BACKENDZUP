using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZuperBackend.Application.DTOs.Asset;
using ZuperBackend.Application.Services;

namespace ZuperBackend.API.Controllers
{
    /// <summary>
    /// Controller para gestionar Activos (Assets)
    /// Proporciona endpoints CRUD para activos multi-tenant
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IZuperService _zuperService;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(IAssetService assetService, IZuperService zuperService, ILogger<AssetsController> logger)
        {
            _assetService = assetService;
            _zuperService = zuperService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los activos del tenant
        /// GET: api/assets
        /// </summary>
        /// <returns>Lista de activos</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AssetDto>>> GetAll()
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                _logger.LogInformation($"Obteniendo activos para tenant: {tenantId}");
                
                var assets = await _assetService.GetAllAssetsByTenantAsync(tenantId);
                return Ok(assets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo activos: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un activo específico por ID
        /// GET: api/assets/{id}
        /// </summary>
        /// <param name="id">ID del activo</param>
        /// <returns>Activo encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AssetDto>> GetById(Guid id)
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                _logger.LogInformation($"Obteniendo activo {id} para tenant: {tenantId}");
                
                var asset = await _assetService.GetAssetByIdAsync(tenantId, id);
                
                if (asset == null)
                    return NotFound(new { error = "Activo no encontrado" });
                
                return Ok(asset);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo activo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo activo
        /// POST: api/assets
        /// </summary>
        /// <param name="createDto">Datos del activo a crear</param>
        /// <returns>Activo creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AssetDto>> Create([FromBody] CreateAssetDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tenantId = GetTenantIdFromToken();
                var userId = GetUserIdFromToken();
                
                _logger.LogInformation($"Creando activo para tenant: {tenantId}, user: {userId}");
                
                var asset = await _assetService.CreateAssetAsync(tenantId, createDto, userId);
                
                return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creando activo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un activo existente
        /// PUT: api/assets/{id}
        /// </summary>
        /// <param name="id">ID del activo a actualizar</param>
        /// <param name="updateDto">Datos actualizados</param>
        /// <returns>Activo actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AssetDto>> Update(Guid id, [FromBody] UpdateAssetDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tenantId = GetTenantIdFromToken();
                var userId = GetUserIdFromToken();
                
                _logger.LogInformation($"Actualizando activo {id} para tenant: {tenantId}");
                
                var asset = await _assetService.UpdateAssetAsync(tenantId, id, updateDto, userId);
                
                if (asset == null)
                    return NotFound(new { error = "Activo no encontrado" });
                
                return Ok(asset);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error actualizando activo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un activo (soft delete)
        /// DELETE: api/assets/{id}
        /// </summary>
        /// <param name="id">ID del activo a eliminar</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                var userId = GetUserIdFromToken();
                
                _logger.LogInformation($"Eliminando activo {id} para tenant: {tenantId}");
                
                var success = await _assetService.DeleteAssetAsync(tenantId, id, userId);
                
                if (!success)
                    return NotFound(new { error = "Activo no encontrado" });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error eliminando activo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el conteo total de activos del tenant
        /// GET: api/assets/count
        /// </summary>
        /// <returns>Número total de activos</returns>
        [HttpGet("count/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetAssetCount()
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                var count = await _assetService.GetAssetCountAsync(tenantId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo conteo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Sincroniza un activo con la plataforma Zuper
        /// POST: api/assets/{id}/sync-zuper
        /// </summary>
        [HttpPost("{id}/sync-zuper")]
        public async Task<IActionResult> SyncToZuper(Guid id)
        {
            try
            {
                var zuperId = await _zuperService.SyncAssetAsync(id);
                return Ok(new { message = "Sincronización con Zuper exitosa", zuperExternalId = zuperId });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Activo no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en sincronización Zuper: {ex.Message}");
                return StatusCode(500, new { error = "Error en la integración con Zuper" });
            }
        }

        /// <summary>
        /// Extrae TenantId del token JWT
        /// </summary>
        private Guid GetTenantIdFromToken()
        {
            var tenantIdClaim = User.FindFirst("TenantId");
            if (tenantIdClaim == null || !Guid.TryParse(tenantIdClaim.Value, out var tenantId))
                throw new UnauthorizedAccessException("TenantId no encontrado en token");
            
            return tenantId;
        }

        /// <summary>
        /// Extrae UserId del token JWT
        /// </summary>
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("UserId no encontrado en token");
            
            return userId;
        }
    }
}
