using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZuperBackend.Application.DTOs.Diagnosis;
using ZuperBackend.Application.Services;

namespace ZuperBackend.API.Controllers
{
    /// <summary>
    /// Controller para gestionar el árbol de decisión de Diagnóstico
    /// Proporciona endpoints para navegación y administración del árbol de diagnóstico
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class DiagnosisController : ControllerBase
    {
        private readonly IDiagnosisService _diagnosisService;
        private readonly IZuperService _zuperService;
        private readonly IZendeskService _zendeskService;
        private readonly ILogger<DiagnosisController> _logger;

        public DiagnosisController(
            IDiagnosisService diagnosisService, 
            IZuperService zuperService,
            IZendeskService zendeskService,
            ILogger<DiagnosisController> logger)
        {
            _diagnosisService = diagnosisService;
            _zuperService = zuperService;
            _zendeskService = zendeskService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el nodo inicial del árbol de diagnóstico
        /// GET: api/diagnosis/start
        /// Este es el primer nodo que ve el usuario (NodeCode="START")
        /// </summary>
        /// <returns>Nodo de inicio con sus opciones</returns>
        [HttpGet("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisNodeDto>> GetStartNode()
        {
            try
            {
                _logger.LogInformation("Obteniendo nodo de inicio del árbol de diagnóstico");
                
                var node = await _diagnosisService.GetStartingNodeAsync();
                
                if (node == null)
                    return NotFound(new { error = "Nodo de inicio no encontrado" });
                
                return Ok(node);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo nodo de inicio: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un nodo específico del árbol por ID
        /// GET: api/diagnosis/node/{nodeId}
        /// </summary>
        /// <param name="nodeId">ID del nodo</param>
        /// <returns>Nodo con sus opciones disponibles</returns>
        [HttpGet("node/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisNodeDto>> GetNode(Guid nodeId)
        {
            try
            {
                _logger.LogInformation($"Obteniendo nodo {nodeId}");
                
                var node = await _diagnosisService.GetNodeByIdAsync(nodeId);
                
                if (node == null)
                    return NotFound(new { error = "Nodo no encontrado" });
                
                return Ok(node);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo nodo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un nodo por su código identificador
        /// GET: api/diagnosis/node-by-code/{nodeCode}
        /// Ejemplo: START, ENGINE_CHECK, BATTERY_CHECK, etc.
        /// </summary>
        /// <param name="nodeCode">Código del nodo</param>
        /// <returns>Nodo con sus opciones disponibles</returns>
        [HttpGet("node-by-code/{nodeCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisNodeDto>> GetNodeByCode(string nodeCode)
        {
            try
            {
                _logger.LogInformation($"Obteniendo nodo con código {nodeCode}");
                
                var node = await _diagnosisService.GetNodeByCodeAsync(nodeCode);
                
                if (node == null)
                    return NotFound(new { error = "Nodo no encontrado" });
                
                return Ok(node);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo nodo por código: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las opciones disponibles de un nodo
        /// GET: api/diagnosis/options/{nodeId}
        /// Las opciones están ordenadas por DisplayOrder
        /// </summary>
        /// <param name="nodeId">ID del nodo</param>
        /// <returns>Lista de opciones de respuesta</returns>
        [HttpGet("options/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<DiagnosisOptionDto>>> GetNodeOptions(Guid nodeId)
        {
            try
            {
                _logger.LogInformation($"Obteniendo opciones para nodo {nodeId}");
                
                var options = await _diagnosisService.GetNodeOptionsAsync(nodeId);
                
                return Ok(options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo opciones del nodo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Procesa la selección de una opción por parte del usuario
        /// POST: api/diagnosis/process-option
        /// Devuelve el siguiente nodo o la acción final (RESOLVED, ESCALATE, DOCUMENTATION)
        /// </summary>
        /// <param name="request">ID de la opción seleccionada</param>
        /// <returns>Resultado del flujo: próximo nodo o acción final</returns>
        [HttpPost("process-option")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisFlowResultDto>> ProcessOption([FromBody] ProcessOptionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation($"Procesando selección de opción {request.OptionId} para activo {request.AssetId}");
                
                var result = await _diagnosisService.ProcessOptionSelectionAsync(request.OptionId);
                
                if (result == null)
                    return NotFound(new { error = "Opción no encontrada" });

                // Lógica de Integraciones Externas si es una acción final
                if (result.IsFinal && result.ActionResult != null && request.AssetId.HasValue)
                {
                    var assetId = request.AssetId.Value;
                    var description = result.ActionResult.ActionDescription ?? "Falla reportada vía QR";

                    if (result.ActionResult.ActionCode == "CREATE_TICKET_ZENDESK")
                    {
                        // Asegurar que AdditionalData no sea nulo
                        result.ActionResult.AdditionalData ??= new Dictionary<string, object>();

                        // 1. Crear Ticket en Zendesk
                        var zendeskTicketId = await _zendeskService.CreateTicketAsync(assetId, description);
                        result.ActionResult.AdditionalData["zendeskTicketId"] = zendeskTicketId;

                        // 2. Sincronizar con Zuper (crear Job)
                        var zuperJobId = await _zuperService.SyncAssetAsync(assetId);
                        result.ActionResult.AdditionalData["zuperJobId"] = zuperJobId;

                        result.ActionResult.UserMessage += $"\nTicket generado: {zendeskTicketId}. Un técnico ha sido asignado vía Zuper.";
                    }
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error procesando opción: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, detail = ex.StackTrace });
            }
        }

        /// <summary>
        /// Obtiene el árbol completo de diagnóstico (solo administradores)
        /// GET: api/diagnosis/tree
        /// Útil para visualización y administración del árbol completo
        /// </summary>
        /// <returns>Árbol completo con todos los nodos y opciones</returns>
        [HttpGet("tree")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisTreeDto>> GetCompleteTree()
        {
            try
            {
                _logger.LogInformation("Obteniendo árbol completo de diagnóstico");
                
                var tree = await _diagnosisService.GetCompleteTreeAsync();
                
                return Ok(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo árbol completo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todos los nodos del árbol (solo administradores)
        /// GET: api/diagnosis/all-nodes
        /// </summary>
        /// <returns>Lista de todos los nodos</returns>
        [HttpGet("all-nodes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<DiagnosisNodeDto>>> GetAllNodes()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los nodos del árbol");
                
                var nodes = await _diagnosisService.GetAllNodesAsync();
                
                return Ok(nodes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo todos los nodos: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo nodo en el árbol (solo administradores)
        /// POST: api/diagnosis/node
        /// </summary>
        /// <param name="createDto">Datos del nuevo nodo</param>
        /// <returns>Nodo creado</returns>
        [HttpPost("node")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisNodeDto>> CreateNode([FromBody] CreateDiagnosisNodeDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation($"Creando nodo de diagnóstico: {createDto.NodeCode}");
                
                var node = await _diagnosisService.CreateNodeAsync(createDto);
                
                return CreatedAtAction(nameof(GetNode), new { nodeId = node.Id }, node);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creando nodo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un nodo existente (solo administradores)
        /// PUT: api/diagnosis/node/{nodeId}
        /// </summary>
        /// <param name="nodeId">ID del nodo a actualizar</param>
        /// <param name="updateDto">Datos actualizados</param>
        /// <returns>Nodo actualizado</returns>
        [HttpPut("node/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisNodeDto>> UpdateNode(Guid nodeId, [FromBody] UpdateDiagnosisNodeDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation($"Actualizando nodo {nodeId}");
                
                var node = await _diagnosisService.UpdateNodeAsync(nodeId, updateDto);
                
                if (node == null)
                    return NotFound(new { error = "Nodo no encontrado" });
                
                return Ok(node);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error actualizando nodo: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva opción en un nodo (solo administradores)
        /// POST: api/diagnosis/option
        /// </summary>
        /// <param name="createDto">Datos de la nueva opción</param>
        /// <returns>Opción creada</returns>
        [HttpPost("option")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DiagnosisOptionDto>> CreateOption([FromBody] CreateDiagnosisOptionDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation($"Creando opción para nodo {createDto.DiagnosisNodeId}");
                
                var option = await _diagnosisService.CreateOptionAsync(createDto);
                
                return CreatedAtAction(nameof(GetNodeOptions), new { nodeId = createDto.DiagnosisNodeId }, option);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creando opción: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un nodo del diagnóstico
        /// DELETE: api/diagnosis/node/{nodeId}
        /// </summary>
        [HttpDelete("node/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNode(Guid nodeId)
        {
            var success = await _diagnosisService.DeleteNodeAsync(nodeId);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Elimina una opción del diagnóstico
        /// DELETE: api/diagnosis/option/{optionId}
        /// </summary>
        [HttpDelete("option/{optionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOption(Guid optionId)
        {
            var success = await _diagnosisService.DeleteOptionAsync(optionId);
            if (!success) return NotFound();
            return NoContent();
        }
    }

    /// <summary>
    /// Modelo para solicitud de procesamiento de opción
    /// </summary>
    public class ProcessOptionRequest
    {
        public Guid OptionId { get; set; }
        public Guid? AssetId { get; set; }
    }
}
