using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZuperBackend.Application.DTOs.Incident;
using ZuperBackend.Application.Services;

namespace ZuperBackend.API.Controllers
{
    /// <summary>
    /// Controller para gestionar Incidentes (Incidents)
    /// Proporciona endpoints para crear, actualizar y consultar incidentes multi-tenant
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly ILogger<IncidentsController> _logger;

        public IncidentsController(IIncidentService incidentService, ILogger<IncidentsController> logger)
        {
            _incidentService = incidentService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los incidentes del tenant
        /// GET: api/incidents
        /// </summary>
        /// <returns>Lista de incidentes</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<IncidentDto>>> GetAll()
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                _logger.LogInformation($"Obteniendo incidentes para tenant: {tenantId}");
                
                var incidents = await _incidentService.GetAllIncidentsByTenantAsync(tenantId);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo incidentes: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un incidente específico por ID
        /// GET: api/incidents/{id}
        /// </summary>
        /// <param name="id">ID del incidente</param>
        /// <returns>Incidente encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IncidentDto>> GetById(Guid id)
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                _logger.LogInformation($"Obteniendo incidente {id} para tenant: {tenantId}");
                
                var incident = await _incidentService.GetIncidentByIdAsync(tenantId, id);
                
                if (incident == null)
                    return NotFound(new { error = "Incidente no encontrado" });
                
                return Ok(incident);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo incidente: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene incidentes filtrados por estado
        /// GET: api/incidents/status/{status}
        /// Estados válidos: Open, InProgress, Resolved, Closed
        /// </summary>
        /// <param name="status">Estado del incidente</param>
        /// <returns>Lista de incidentes por estado</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<IncidentDto>>> GetByStatus(string status)
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                _logger.LogInformation($"Obteniendo incidentes con estado {status} para tenant: {tenantId}");
                
                var incidents = await _incidentService.GetIncidentsByStatusAsync(tenantId, status);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo incidentes por estado: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene incidentes relacionados a un activo específico
        /// GET: api/incidents/asset/{assetId}
        /// </summary>
        /// <param name="assetId">ID del activo</param>
        /// <returns>Lista de incidentes del activo</returns>
        [HttpGet("asset/{assetId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<IncidentDto>>> GetByAssetId(Guid assetId)
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                _logger.LogInformation($"Obteniendo incidentes para activo {assetId}, tenant: {tenantId}");
                
                var incidents = await _incidentService.GetIncidentsByAssetIdAsync(tenantId, assetId);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo incidentes por activo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo incidente
        /// POST: api/incidents
        /// </summary>
        /// <param name="createDto">Datos del incidente a crear</param>
        /// <returns>Incidente creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IncidentDto>> Create([FromBody] CreateIncidentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tenantId = GetTenantIdFromToken();
                var userId = GetUserIdFromToken();
                
                _logger.LogInformation($"Creando incidente para tenant: {tenantId}, user: {userId}");
                
                var incident = await _incidentService.CreateIncidentAsync(tenantId, createDto, userId);
                
                return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creando incidente: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un incidente existente
        /// PUT: api/incidents/{id}
        /// </summary>
        /// <param name="id">ID del incidente a actualizar</param>
        /// <param name="updateDto">Datos actualizados</param>
        /// <returns>Incidente actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IncidentDto>> Update(Guid id, [FromBody] UpdateIncidentDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tenantId = GetTenantIdFromToken();
                var userId = GetUserIdFromToken();
                
                _logger.LogInformation($"Actualizando incidente {id} para tenant: {tenantId}");
                
                var incident = await _incidentService.UpdateIncidentAsync(tenantId, id, updateDto, userId);
                
                if (incident == null)
                    return NotFound(new { error = "Incidente no encontrado" });
                
                return Ok(incident);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error actualizando incidente: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Resuelve un incidente con notas finales
        /// POST: api/incidents/{id}/resolve
        /// </summary>
        /// <param name="id">ID del incidente</param>
        /// <param name="request">Notas de resolución</param>
        /// <returns>Incidente resuelto</returns>
        [HttpPost("{id}/resolve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IncidentDto>> Resolve(Guid id, [FromBody] ResolveIncidentRequest request)
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                var userId = GetUserIdFromToken();
                
                _logger.LogInformation($"Resolviendo incidente {id} para tenant: {tenantId}");
                
                var incident = await _incidentService.ResolveIncidentAsync(tenantId, id, request.ResolutionNotes, userId);
                
                if (incident == null)
                    return NotFound(new { error = "Incidente no encontrado" });
                
                return Ok(incident);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error resolviendo incidente: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el conteo de incidentes abiertos (para dashboard)
        /// GET: api/incidents/count/open
        /// </summary>
        /// <returns>Número de incidentes abiertos</returns>
        [HttpGet("count/open")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetOpenIncidentsCount()
        {
            try
            {
                var tenantId = GetTenantIdFromToken();
                var count = await _incidentService.GetOpenIncidentsCountAsync(tenantId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo conteo abiertos: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Extrae TenantId del token JWT
        /// </summary>
        private Guid GetTenantIdFromToken()
        {
            var tenantIdClaim = User.FindFirst("tenantid");
            if (tenantIdClaim == null || !Guid.TryParse(tenantIdClaim.Value, out var tenantId))
                throw new UnauthorizedAccessException("TenantId no encontrado en token");
            
            return tenantId;
        }

        /// <summary>
        /// Extrae UserId del token JWT
        /// </summary>
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("UserId no encontrado en token");
            
            return userId;
        }
    }

    /// <summary>
    /// Modelo para solicitud de resolución de incidente
    /// </summary>
    public class ResolveIncidentRequest
    {
        public required string ResolutionNotes { get; set; }
    }
}
