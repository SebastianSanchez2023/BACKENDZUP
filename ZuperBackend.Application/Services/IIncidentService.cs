using ZuperBackend.Application.DTOs.Incident;

namespace ZuperBackend.Application.Services;

/// <summary>
/// Interfaz que define el contrato para operaciones de Incidentes (Incidents).
/// 
/// Un INCIDENTE es un "problema reportado" que necesita ser resuelto.
/// Puede ser un asset averiado, una consulta técnica, etc.
/// 
/// RELACIONES:
/// - Cada Incident está asociado a un Asset (el equipo problemático)
/// - Cada Incident está asociado a un Tenant (seguridad multi-tenant)
/// - Cada Incident tiene un DiagnosisPath (árbol de diagnóstico seguido)
/// - Cada Incident puede sync-earse con Zendesk (tickets) y Zuper (work orders)
/// </summary>
public interface IIncidentService
{
    /// <summary>
    /// Obtiene todos los incidentes de un inquilino.
    /// 
    /// PARÁMETRO tenantId: Solo incidentes de este tenant.
    /// 
    /// RETORNA: Lista de IncidentDto (sin datos internos sensibles)
    /// </summary>
    Task<IEnumerable<IncidentDto>> GetAllIncidentsByTenantAsync(Guid tenantId);

    /// <summary>
    /// Obtiene un incidente específico por ID.
    /// 
    /// SEGURIDAD: Verifica que el incident pertenezca al tenant.
    /// RETORNA: IncidentDto o null si no existe.
    /// </summary>
    Task<IncidentDto?> GetIncidentByIdAsync(Guid tenantId, Guid incidentId);

    /// <summary>
    /// Obtiene incidentes filtrados por estado.
    /// 
    /// PARÁMETRUS:
    /// - tenantId: Del inquilino
    /// - status: Ej: "Open", "InProgress", "Resolved", "Closed"
    /// 
    /// USO: Filtrar en dashboard - "Mostrar solo incidentes abiertos"
    /// 
    /// RETORNA: Lista de IncidentDto con ese estado
    /// </summary>
    Task<IEnumerable<IncidentDto>> GetIncidentsByStatusAsync(Guid tenantId, string status);

    /// <summary>
    /// Obtiene incidentes filtrados por activo.
    /// 
    /// PARÁMETRUS:
    /// - tenantId: Del inquilino
    /// - assetId: ID del asset/equipo
    /// 
    /// USO: Ver el historial de problemas de un equipo específico
    /// 
    /// RETORNA: Lista de todos los incidentes reportados para ese asset
    /// </summary>
    Task<IEnumerable<IncidentDto>> GetIncidentsByAssetIdAsync(Guid tenantId, Guid assetId);

    /// <summary>
    /// Crea un nuevo incidente.
    /// 
    /// PARÁMETRUS:
    /// - tenantId: A qué inquilino pertenece
    /// - createIncidentDto: Datos del cliente (AssetId, Category, Description, etc)
    /// - reportedByUserId: Quién reporta el problema
    /// 
    /// LÓGICA INTERNA:
    /// 1. Valida que el Asset existe y pertenece al tenant
    /// 2. Mapea CreateIncidentDto → Incident (Entity)
    /// 3. Asigna estado inicial "Open"
    /// 4. Guarda en BD
    /// 5. PODRÍA: Crear ticket en Zendesk automáticamente (para integración)
    /// 6. PODRÍA: Crear work order en Zuper automáticamente
    /// 
    /// RETORNA: IncidentDto del incidente creado (con ID)
    /// </summary>
    Task<IncidentDto> CreateIncidentAsync(Guid tenantId, CreateIncidentDto createIncidentDto, Guid reportedByUserId);

    /// <summary>
    /// Actualiza un incidente existente (cambiar prioridad, asignación, etc).
    /// 
    /// PARÁMETRUS:
    /// - tenantId: Verifica que el incident pertenezca a este tenant
    /// - incidentId: Cuál incident actualizar
    /// - updateIncidentDto: Nuevos valores
    /// - updatedByUserId: Quién está actualizando
    /// 
    /// LÓGICA INTERNA:
    /// 1. Obtiene el incident de BD
    /// 2. Valida que pertenezca al tenant
    /// 3. Actualiza los campos (Status, Priority, ResolutionNotes, etc)
    /// 4. Si está marcado como "Resolved", asigna ClosedAt
    /// 5. Actualiza UpdatedBy, UpdatedAt
    /// 6. PODRÍA: Sincronizar cambios con Zendesk/Zuper
    /// 
    /// RETORNA: IncidentDto actualizado
    /// </summary>
    Task<IncidentDto> UpdateIncidentAsync(Guid tenantId, Guid incidentId, UpdateIncidentDto updateIncidentDto, Guid updatedByUserId);

    /// <summary>
    /// Marca un incidente como resuelto.
    /// 
    /// PARÁMETRUS:
    /// - tenantId: Validación de tenant
    /// - incidentId: Cuál incident resolver
    /// - resolutionNotes: Notas de la solución (ej: "Reemplazado motor")
    /// - resolvedByUserId: Quién resolvió
    /// 
    /// LÓGICA INTERNA:
    /// 1. Obtiene el incident
    /// 2. Asigna Status = "Resolved"
    /// 3. Asigna ClosedAt = DateTime.Now
    /// 4. Guarda ResolutionNotes y AssignedTo
    /// 5. PODRÍA: Cerrar ticket en Zendesk
    /// 6. PODRÍA: Marcar work order como completado en Zuper
    /// 
    /// RETORNA: IncidentDto del incidente resuelto
    /// </summary>
    Task<IncidentDto> ResolveIncidentAsync(Guid tenantId, Guid incidentId, string resolutionNotes, Guid resolvedByUserId);

    /// <summary>
    /// Obtiene el conteo de incidentes abiertos (para dashboard).
    /// 
    /// RETORNA: Número de incidentes con Status != "Closed"
    /// </summary>
    Task<int> GetOpenIncidentsCountAsync(Guid tenantId);
}
