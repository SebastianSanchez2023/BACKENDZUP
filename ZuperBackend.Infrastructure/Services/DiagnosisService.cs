using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ZuperBackend.Application.DTOs.Diagnosis;
using ZuperBackend.Application.Services;
using ZuperBackend.Domain.Entities;
using ZuperBackend.Infrastructure.Persistence;

namespace ZuperBackend.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de Diagnóstico.
/// 
/// Gestiona el árbol de diagnóstico interactivo.
/// 
/// RESPONSABILIDADES:
/// 1. Navegar por el árbol de diagnóstico
/// 2. Procesar selecciones del usuario
/// 3. Retornar siguiente pregunta o acción final
/// 4. Administración del árbol (crear/actualizar nodos y opciones)
/// </summary>
public class DiagnosisService : IDiagnosisService
{
    private readonly ZuperBackendDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// </summary>
    public DiagnosisService(ZuperBackendDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene el nodo inicial del árbol.
    /// 
    /// BÚSQUEDA: Busca node con NodeCode = "START"
    /// ALTERNATIVA: Si no existe START, retorna el node con DisplayOrder más bajo
    /// 
    /// RETORNA: DiagnosisNodeDto del punto de partida
    /// USO: Cuando el usuario comienza un nuevo diagnóstico
    /// </summary>
    public async Task<DiagnosisNodeDto?> GetStartingNodeAsync()
    {
        try
        {
            // Busca el nodo de inicio (preferencia: NodeCode = "START")
            var startNode = await Task.Run(() =>
                _context.DiagnosisNodes
                    .FirstOrDefault(n => n.NodeCode == "START" && n.IsActive)
            );

            // Si no existe, obtiene el node con orden más bajo
            if (startNode == null)
            {
                startNode = await Task.Run(() =>
                    _context.DiagnosisNodes
                        .Where(n => n.IsActive)
                        .OrderBy(n => n.DisplayOrder)
                        .FirstOrDefault()
                ) ?? null;
            }

            if (startNode == null)
                return null;

            // Mapea a DTO y carga sus opciones
            var nodeDto = _mapper.Map<DiagnosisNodeDto>(startNode);
            nodeDto.Options = await GetNodeOptionsAsync(startNode.Id);

            return nodeDto;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al obtener nodo de inicio", ex);
        }
    }

    /// <summary>
    /// Obtiene un nodo específico por ID.
    /// 
    /// INCLUYE: Sus opciones (respuestas) ordenadas por DisplayOrder
    /// </summary>
    public async Task<DiagnosisNodeDto?> GetNodeByIdAsync(Guid nodeId)
    {
        try
        {
            var node = await Task.Run(() =>
                _context.DiagnosisNodes
                    .FirstOrDefault(n => n.Id == nodeId && n.IsActive)
            );

            if (node == null)
                return null;

            var nodeDto = _mapper.Map<DiagnosisNodeDto>(node);
            nodeDto.Options = await GetNodeOptionsAsync(nodeId);

            return nodeDto;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener nodo {nodeId}", ex);
        }
    }

    /// <summary>
    /// Obtiene un nodo por su código único.
    /// 
    /// EJEMPLO: GetNodeByCodeAsync("POWER_CHECK")
    /// </summary>
    public async Task<DiagnosisNodeDto?> GetNodeByCodeAsync(string nodeCode)
    {
        try
        {
            var node = await Task.Run(() =>
                _context.DiagnosisNodes
                    .FirstOrDefault(n => n.NodeCode == nodeCode && n.IsActive)
            );

            if (node == null)
                return null;

            var nodeDto = _mapper.Map<DiagnosisNodeDto>(node);
            nodeDto.Options = await GetNodeOptionsAsync(node.Id);

            return nodeDto;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener nodo con código {nodeCode}", ex);
        }
    }

    /// <summary>
    /// Obtiene todas las opciones de un nodo, ordenadas.
    /// 
    /// RETORNA: Opciones activas ordenadas por DisplayOrder
    /// </summary>
    public async Task<IEnumerable<DiagnosisOptionDto>> GetNodeOptionsAsync(Guid nodeId)
    {
        try
        {
            var options = await Task.Run(() =>
                _context.DiagnosisOptions
                    .Where(o => o.DiagnosisNodeId == nodeId && o.IsActive)
                    .OrderBy(o => o.DisplayOrder)
                    .ToList()
            );

            return _mapper.Map<IEnumerable<DiagnosisOptionDto>>(options);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener opciones del nodo {nodeId}", ex);
        }
    }

    /// <summary>
    /// Procesa la selección de una opción por parte del usuario.
    /// 
    /// FLUJO:
    /// 1. Obtiene la opción seleccionada
    /// 2. Si NextNodeId es null → es FINAL (retorna DiagnosisFlowResultDto con action)
    /// 3. Si NextNodeId tiene valor → retorna el siguiente nodo
    /// 
    /// RETORNA: DiagnosisFlowResultDto que el cliente interpreta
    /// </summary>
    public async Task<DiagnosisFlowResultDto> ProcessOptionSelectionAsync(Guid optionId)
    {
        try
        {
            // Obtiene la opción seleccionada
            var option = await Task.Run(() =>
                _context.DiagnosisOptions
                    .FirstOrDefault(o => o.Id == optionId && o.IsActive)
            );

            if (option == null)
                throw new KeyNotFoundException(
                    $"Opción {optionId} no encontrada");

            // Verifica si es una acción final
            if (option.IsFinalAction || !option.NextNodeId.HasValue)
            {
                // Retorna resultado con acción final
                return new DiagnosisFlowResultDto
                {
                    IsFinal = true,
                    ActionResult = new DiagnosisActionResultDto
                    {
                        ActionCode = option.ActionCode,
                        ActionDescription = option.ActionDescription,
                        UserMessage = $"{option.Label}: {option.ActionDescription}",
                        AdditionalData = new Dictionary<string, object>
                        {
                            { "selectedLabel", option.Label }
                        }
                    }
                };
            }

            // Si hay próximo nodo, lo obtiene y retorna
            var nextNode = await GetNodeByIdAsync(option.NextNodeId.Value);

            return new DiagnosisFlowResultDto
            {
                IsFinal = false,
                NextNode = nextNode
            };
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al procesar opción {optionId}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los nodos del árbol (operación administrativa).
    /// </summary>
    public async Task<IEnumerable<DiagnosisNodeDto>> GetAllNodesAsync()
    {
        try
        {
            var nodes = await Task.Run(() =>
                _context.DiagnosisNodes
                    .OrderBy(n => n.DisplayOrder)
                    .ToList()
            );

            // Para cada nodo, carga sus opciones
            var nodeDtos = _mapper.Map<List<DiagnosisNodeDto>>(nodes);

            foreach (var nodeDto in nodeDtos)
            {
                nodeDto.Options = await GetNodeOptionsAsync(nodeDto.Id);
            }

            return nodeDtos;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al obtener todos los nodos", ex);
        }
    }

    /// <summary>
    /// Crea un nuevo nodo en el árbol (operación administrativa).
    /// </summary>
    public async Task<DiagnosisNodeDto> CreateNodeAsync(CreateDiagnosisNodeDto createNodeDto)
    {
        try
        {
            // Validación
            if (string.IsNullOrWhiteSpace(createNodeDto.NodeCode))
                throw new ArgumentException("NodeCode es requerido");

            // Verifica que NodeCode sea único
            var existingNode = await Task.Run(() =>
                _context.DiagnosisNodes
                    .FirstOrDefault(n => n.NodeCode == createNodeDto.NodeCode)
            );

            if (existingNode != null)
                throw new InvalidOperationException(
                    $"NodeCode '{createNodeDto.NodeCode}' ya existe");

            // Mapea DTO → Entity
            var node = _mapper.Map<DiagnosisNode>(createNodeDto);

            // Asigna sistema
            node.Id = Guid.NewGuid();
            node.IsActive = true;
            node.CreatedAt = DateTime.UtcNow;

            // Guarda
            _context.DiagnosisNodes.Add(node);
            await _context.SaveChangesAsync();

            return _mapper.Map<DiagnosisNodeDto>(node);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al crear nodo", ex);
        }
    }

    /// <summary>
    /// Actualiza un nodo existente.
    /// </summary>
    public async Task<DiagnosisNodeDto> UpdateNodeAsync(Guid nodeId, UpdateDiagnosisNodeDto updateNodeDto)
    {
        try
        {
            var node = await Task.Run(() =>
                _context.DiagnosisNodes
                    .FirstOrDefault(n => n.Id == nodeId)
            );

            if (node == null)
                throw new KeyNotFoundException($"Nodo {nodeId} no encontrado");

            // Mapea actualizaciones
            _mapper.Map(updateNodeDto, node);

            // Auditoría
            node.UpdatedAt = DateTime.UtcNow;

            _context.DiagnosisNodes.Update(node);
            await _context.SaveChangesAsync();

            return _mapper.Map<DiagnosisNodeDto>(node);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al actualizar nodo {nodeId}", ex);
        }
    }

    /// <summary>
    /// Crea una nueva opción dentro de un nodo.
    /// </summary>
    public async Task<DiagnosisOptionDto> CreateOptionAsync(CreateDiagnosisOptionDto createOptionDto)
    {
        try
        {
            // Validación: el nodo debe existir
            var node = await Task.Run(() =>
                _context.DiagnosisNodes
                    .FirstOrDefault(n => n.Id == createOptionDto.DiagnosisNodeId)
            );

            if (node == null)
                throw new KeyNotFoundException(
                    $"Nodo {createOptionDto.DiagnosisNodeId} no encontrado");

            // Validación: si hay NextNodeId, debe existir
            if (createOptionDto.NextNodeId.HasValue)
            {
                var nextNode = await Task.Run(() =>
                    _context.DiagnosisNodes
                        .FirstOrDefault(n => n.Id == createOptionDto.NextNodeId)
                );

                if (nextNode == null)
                    throw new KeyNotFoundException(
                        $"Nodo siguiente {createOptionDto.NextNodeId} no encontrado");
            }

            // Mapea
            var option = _mapper.Map<DiagnosisOption>(createOptionDto);

            // Asigna sistema
            option.Id = Guid.NewGuid();
            option.IsActive = true;
            option.CreatedAt = DateTime.UtcNow;

            // Guarda
            _context.DiagnosisOptions.Add(option);
            await _context.SaveChangesAsync();

            return _mapper.Map<DiagnosisOptionDto>(option);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al crear opción", ex);
        }
    }

    /// <summary>
    /// Obtiene el árbol completo en formato jerárquico.
    /// 
    /// USO: Dashboard administrativo, visualización completa
    /// </summary>
    public async Task<DiagnosisTreeDto> GetCompleteTreeAsync()
    {
        try
        {
            var allNodes = await GetAllNodesAsync();
            var allOptions = await Task.Run(() =>
                _context.DiagnosisOptions
                    .OrderBy(o => o.DiagnosisNodeId)
                    .ThenBy(o => o.DisplayOrder)
                    .ToList()
            );

            var mappedOptions = _mapper.Map<IEnumerable<DiagnosisOptionDto>>(allOptions);

            var tree = new DiagnosisTreeDto
            {
                RootNodeId = (await GetStartingNodeAsync())?.Id ?? Guid.Empty,
                AllNodes = allNodes,
                AllOptions = mappedOptions,
                Metadata = new DiagnosisTreeMetadataDto
                {
                    TotalNodes = allNodes.Count(),
                    TotalOptions = mappedOptions.Count(),
                    MaxDepth = CalculateTreeDepth(allNodes, (await GetStartingNodeAsync())?.Id ?? Guid.Empty),
                    LastUpdatedAt = DateTime.UtcNow  // TODO: Obtener del último update real
                }
            };

            return tree;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al obtener árbol completo", ex);
        }
    }

    /// <summary>
    /// Calcula la profundidad máxima del árbol (también llamada altura).
    /// 
    /// LÓGICA: Recorre desde raíz siguiendo NextNodeId de cada opción
    /// </summary>
    private int CalculateTreeDepth(IEnumerable<DiagnosisNodeDto> allNodes, Guid rootId)
    {
        // TODO: Implementar cálculo real con DFS (Depth-First Search)
        // Por ahora retorna placeholder
        return 5;
    }
    public async Task<bool> DeleteNodeAsync(Guid nodeId)
    {
        var node = await _context.DiagnosisNodes
            .Include(n => n.Options)
            .FirstOrDefaultAsync(n => n.Id == nodeId);

        if (node == null) return false;

        _context.DiagnosisNodes.Remove(node);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOptionAsync(Guid optionId)
    {
        var option = await _context.DiagnosisOptions
            .FirstOrDefaultAsync(o => o.Id == optionId);

        if (option == null) return false;

        _context.DiagnosisOptions.Remove(option);
        await _context.SaveChangesAsync();
        return true;
    }
}
