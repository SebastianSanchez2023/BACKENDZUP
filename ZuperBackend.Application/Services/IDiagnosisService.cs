using ZuperBackend.Application.DTOs.Diagnosis;

namespace ZuperBackend.Application.Services;

/// <summary>
/// Interfaz que define el servicio para interactuar con el árbol de diagnóstico.
/// 
/// CONTEXTO:
/// El sistema tiene un "árbol de decisión" para diagnosticar problemas.
/// Funciona así:
/// 
/// 1. El usuario comienza en la raíz (DiagnosisNode más simple)
/// 2. Se presenta una pregunta (Question)
/// 3. El usuario elige una opción (DiagnosisOption)
/// 4. Según la opción, el sistema:
///    - Va a la siguiente pregunta (NextNodeId)
///    - O ejecuta una acción (ActionCode: RESOLVED, ESCALATE, DOCUMENTATION)
/// 5. Continúa hasta que se resuelva o se escale
/// 
/// EJEMPLO:
/// Q1: ¿El equipo enciende?
///  ├─ Opción A: No → ActionCode=ESCALATE → "Contactar técnico"
///  └─ Opción B: Sí → NextNodeId=Q2
/// 
/// Q2: ¿Hace ruido anormal?
///  ├─ Opción A: No → ActionCode=RESOLVED → "Funcionando correctamente"
///  └─ Opción B: Sí → ActionCode=DOCUMENTATION → "Ver manual de mantenimiento"
/// </summary>
public interface IDiagnosisService
{
    /// <summary>
    /// Obtiene el nodo raíz del árbol de diagnóstico.
    /// 
    /// RETORNA: DiagnosisNodeDto del punto de partida (generalmente con NodeCode = "START")
    /// 
    /// USO: Cuando el usuario inicia un nuevo diagnóstico
    /// </summary>
    Task<DiagnosisNodeDto?> GetStartingNodeAsync();

    /// <summary>
    /// Obtiene un nodo específico del árbol.
    /// 
    /// PARÁMETRO nodeId: ID del nodo a obtener
    /// 
    /// RETORNA: DiagnosisNodeDto con sus opciones (Options)
    /// </summary>
    Task<DiagnosisNodeDto?> GetNodeByIdAsync(Guid nodeId);

    /// <summary>
    /// Obtiene un nodo por su código único (alternativa a ID).
    /// 
    /// PARÁMETRO nodeCode: Código único (ej: "START", "POWER_CHECK", "NOISE_CHECK")
    /// 
    /// RETORNA: DiagnosisNodeDto
    /// 
    /// USO: A veces es más fácil usar códigos que IDs en el cliente
    /// </summary>
    Task<DiagnosisNodeDto?> GetNodeByCodeAsync(string nodeCode);

    /// <summary>
    /// Obtiene todas las opciones de un nodo.
    /// 
    /// PARÁMETRO nodeId: ID del nodo padre
    /// 
    /// RETORNA: Lista de DiagnosisOptionDto del nodo (ordenadas por DisplayOrder)
    /// 
    /// USO: Cuando necesitas renderizar las opciones de respuesta
    /// </summary>
    Task<IEnumerable<DiagnosisOptionDto>> GetNodeOptionsAsync(Guid nodeId);

    /// <summary>
    /// Procesa la selección de una opción por el usuario.
    /// 
    /// PARÁMETRUS:
    /// - optionId: Cuál opción seleccionó el usuario
    /// 
    /// LÓGICA INTERNA:
    /// 1. Obtiene la opción
    /// 2. Si NextNodeId es null → es una acción final (retorna DiagnosisResultDto)
    /// 3. Si NextNodeId tiene valor → retorna el próximo nodo
    /// 4. La aplicación cliente sigue la ruta
    /// 
    /// RETORNA: DiagnosisFlowResultDto con:
    ///   - IsFinal: ¿Es el fin del árbol?
    ///   - NextNode: Siguiente nodo (si no es final)
    ///   - Action: Acción a ejecutar (si es final)
    /// </summary>
    Task<DiagnosisFlowResultDto> ProcessOptionSelectionAsync(Guid optionId);

    /// <summary>
    /// Obtiene todos los nodos del árbol (para administración/visualización).
    /// 
    /// RETORNA: Lista completa de todos los nodos (sin filtrar por estado)
    /// 
    /// USO: Dashboard administrativo, visualizar todo el árbol, auditoría
    /// </summary>
    Task<IEnumerable<DiagnosisNodeDto>> GetAllNodesAsync();

    /// <summary>
    /// Crea un nuevo nodo en el árbol (para administración).
    /// 
    /// PARÁMETRO createNodeDto: Datos del nuevo nodo
    /// 
    /// RETORNA: DiagnosisNodeDto creado (con ID asignado)
    /// 
    /// NOTA: Esta es una operación adminstrativa, requiere permisos
    /// </summary>
    Task<DiagnosisNodeDto> CreateNodeAsync(CreateDiagnosisNodeDto createNodeDto);

    /// <summary>
    /// Actualiza un nodo existente.
    /// 
    /// PARÁMETRUS:
    /// - nodeId: Cuál nodo actualizar
    /// - updateNodeDto: Nuevos datos
    /// 
    /// RETORNA: DiagnosisNodeDto actualizado
    /// </summary>
    Task<DiagnosisNodeDto> UpdateNodeAsync(Guid nodeId, UpdateDiagnosisNodeDto updateNodeDto);

    /// <summary>
    /// Crea una nueva opción dentro de un nodo.
    /// 
    /// PARÁMETRS:
    /// - createOptionDto: Datos de la opción (DiagnosisNodeId, Label, NextNodeId, ActionCode, etc)
    /// 
    /// RETORNA: DiagnosisOptionDto creado
    /// </summary>
    Task<DiagnosisOptionDto> CreateOptionAsync(CreateDiagnosisOptionDto createOptionDto);

    /// <summary>
    /// Obtiene el árbol completo en formato jerárquico (para visualización).
    /// 
    /// RETORNA: Árbol completo con todos los nodos y opciones estructurados
    /// </summary>
    Task<DiagnosisTreeDto> GetCompleteTreeAsync();

    /// <summary>
    /// Elimina un nodo y todas sus opciones asociadas.
    /// </summary>
    Task<bool> DeleteNodeAsync(Guid nodeId);

    /// <summary>
    /// Elimina una opción de respuesta específica.
    /// </summary>
    Task<bool> DeleteOptionAsync(Guid optionId);
}
