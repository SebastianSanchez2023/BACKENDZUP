using AutoMapper;
using ZuperBackend.Application.DTOs.Incident;
using ZuperBackend.Application.Services;
using ZuperBackend.Domain.Entities;
using ZuperBackend.Infrastructure.Persistence;

namespace ZuperBackend.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de Incidentes.
/// 
/// UN INCIDENTE es un problema reportado sobre un asset.
/// 
/// RESPONSABILIDADES ADICIONALES vs AssetService:
/// 1. Validar que el asset existe antes de crear incident
/// 2. Manejar estado del incidente (Open, InProgress, Resolved, Closed)
/// 3. Cargar relaciones (Asset, Tenant)
/// 4. Manejo de fecha de resolución (ClosedAt)
/// </summary>
public class IncidentService : IIncidentService
{
    private readonly ZuperBackendDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// </summary>
    public IncidentService(ZuperBackendDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene todos los incidentes de un tenant.
    /// 
    /// INCLUYE: Asset relacionado (para mostrar nombre del equipo)
    /// ORDENADO: Por fecha de creación más reciente
    /// </summary>
    public async Task<IEnumerable<IncidentDto>> GetAllIncidentsByTenantAsync(Guid tenantId)
    {
        try
        {
            var incidents = await Task.Run(() =>
                _context.Incidents
                    .Where(i => i.TenantId == tenantId && !i.IsDeleted)
                    .OrderByDescending(i => i.CreatedAt)  // Más recientes primero
                    .ToList()
            );

            return _mapper.Map<IEnumerable<IncidentDto>>(incidents);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener incidentes del tenant {tenantId}", ex);
        }
    }

    /// <summary>
    /// Obtiene un incidente específico con su asset relacionado.
    /// </summary>
    public async Task<IncidentDto?> GetIncidentByIdAsync(Guid tenantId, Guid incidentId)
    {
        try
        {
            var incident = await Task.Run(() =>
                _context.Incidents
                    .FirstOrDefault(i => 
                        i.Id == incidentId && 
                        i.TenantId == tenantId && 
                        !i.IsDeleted
                    )
            );

            if (incident == null)
                return null;

            return _mapper.Map<IncidentDto>(incident);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener incidente {incidentId}", ex);
        }
    }

    /// <summary>
    /// Obtiene incidentes filtrados por estado.
    /// 
    /// ESTADOS POSIBLES:
    /// - "Open": Recién reportado
    /// - "InProgress": Técnico está trabajando en ello
    /// - "Resolved": Se encontró solución but aún no cerrado
    /// - "Closed": Completamente cerrado
    /// 
    /// USO: Filtros en dashboard
    /// </summary>
    public async Task<IEnumerable<IncidentDto>> GetIncidentsByStatusAsync(Guid tenantId, string status)
    {
        try
        {
            var incidents = await Task.Run(() =>
                _context.Incidents
                    .Where(i => 
                        i.TenantId == tenantId && 
                        i.Status == status && 
                        !i.IsDeleted
                    )
                    .OrderByDescending(i => i.CreatedAt)
                    .ToList()
            );

            return _mapper.Map<IEnumerable<IncidentDto>>(incidents);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener incidentes con status {status}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los incidentes de un asset específico.
    /// 
    /// USO: Ver historial de problemas de un equipo
    /// </summary>
    public async Task<IEnumerable<IncidentDto>> GetIncidentsByAssetIdAsync(Guid tenantId, Guid assetId)
    {
        try
        {
            var incidents = await Task.Run(() =>
                _context.Incidents
                    .Where(i => 
                        i.TenantId == tenantId && 
                        i.AssetId == assetId && 
                        !i.IsDeleted
                    )
                    .OrderByDescending(i => i.CreatedAt)
                    .ToList()
            );

            return _mapper.Map<IEnumerable<IncidentDto>>(incidents);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al obtener incidentes del asset {assetId}", ex);
        }
    }

    /// <summary>
    /// Crea un nuevo incidente.
    /// 
    /// VALIDACIONES:
    /// 1. El asset debe existir y pertenecer al tenant
    /// 2. El AssetId viene en el DTO
    /// 
    /// INICIALIZACIÓN:
    /// - Status = "Open" (recién reportado)
    /// - CreatedAt = ahora
    /// - ReportedBy = reportedByUserId (quién reportó)
    /// - DiagnosisPath = se guardapara historial
    /// 
    /// INTEGRACIONES FUTURAS:
    /// - Crear ticket en Zendesk
    /// - Crear work order en Zuper
    /// </summary>
    public async Task<IncidentDto> CreateIncidentAsync(
        Guid tenantId, 
        CreateIncidentDto createIncidentDto, 
        Guid reportedByUserId)
    {
        try
        {
            // VALIDACIÓN: El asset debe existir
            var asset = await Task.Run(() =>
                _context.Assets.FirstOrDefault(a => 
                    a.Id == createIncidentDto.AssetId && 
                    a.TenantId == tenantId && 
                    !a.IsDeleted
                )
            );

            if (asset == null)
                throw new KeyNotFoundException(
                    $"Asset {createIncidentDto.AssetId} no encontrado en el tenant");

            // Mapea DTO → Entity
            var incident = _mapper.Map<Incident>(createIncidentDto);

            // Asigna campos de sistema
            incident.Id = Guid.NewGuid();
            incident.TenantId = tenantId;
            incident.Status = "Open";  // Estado inicial
            incident.ReportedBy = reportedByUserId;
            incident.CreatedAt = DateTime.UtcNow;
            incident.CreatedBy = reportedByUserId;
            incident.IsDeleted = false;

            // Agrega a contexto
            _context.Incidents.Add(incident);

            // Guarda en BD
            await _context.SaveChangesAsync();

            // TODO: Crear ticket en Zendesk si está configurado
            // TODO: Crear work order en Zuper si está configurado

            return _mapper.Map<IncidentDto>(incident);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al crear incidente", ex);
        }
    }

    /// <summary>
    /// Actualiza un incidente existente.
    /// 
    /// CAMPOS ACTUALIZABLES:
    /// - Status
    /// - Priority
    /// - ResolutionNotes
    /// - ResolutionHours
    /// - AssignedTechnicianId
    /// </summary>
    public async Task<IncidentDto> UpdateIncidentAsync(
        Guid tenantId, 
        Guid incidentId, 
        UpdateIncidentDto updateIncidentDto, 
        Guid updatedByUserId)
    {
        try
        {
            // Obtiene el incidente actual
            var incident = await Task.Run(() =>
                _context.Incidents.FirstOrDefault(i => 
                    i.Id == incidentId && 
                    i.TenantId == tenantId && 
                    !i.IsDeleted
                )
            );

            if (incident == null)
                throw new KeyNotFoundException(
                    $"Incidente {incidentId} no encontrado");

            // Actualiza los campos
            _mapper.Map(updateIncidentDto, incident);

            // Lógica especial: si está marcado como resuelto
            if (updateIncidentDto.MarkAsResolved == true)
            {
                incident.Status = "Resolved";
                incident.ClosedAt = DateTime.UtcNow;
            }

            // Asigna auditoría
            incident.UpdatedBy = updatedByUserId;
            incident.UpdatedAt = DateTime.UtcNow;

            // Guarda
            _context.Incidents.Update(incident);
            await _context.SaveChangesAsync();

            // TODO: Sincronizar con Zendesk/Zuper si es necesario

            return _mapper.Map<IncidentDto>(incident);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al actualizar incidente {incidentId}", ex);
        }
    }

    /// <summary>
    /// Marca un incidente como resuelto con notas.
    /// 
    /// FLUJO:
    /// 1. Obtiene el incidente
    /// 2. Asigna Status = "Resolved"
    /// 3. Asigna ClosedAt = ahora
    /// 4. Guarda ResolutionNotes
    /// 5. Guarda quién resolvió
    /// </summary>
    public async Task<IncidentDto> ResolveIncidentAsync(
        Guid tenantId, 
        Guid incidentId, 
        string resolutionNotes, 
        Guid resolvedByUserId)
    {
        try
        {
            var incident = await Task.Run(() =>
                _context.Incidents.FirstOrDefault(i => 
                    i.Id == incidentId && 
                    i.TenantId == tenantId && 
                    !i.IsDeleted
                )
            );

            if (incident == null)
                throw new KeyNotFoundException(
                    $"Incidente {incidentId} no encontrado");

            // Asigna valores de resolución
            incident.Status = "Resolved";
            incident.ResolutionNotes = resolutionNotes;
            incident.ClosedAt = DateTime.UtcNow;
            incident.UpdatedBy = resolvedByUserId;
            incident.UpdatedAt = DateTime.UtcNow;

            // Guarda
            _context.Incidents.Update(incident);
            await _context.SaveChangesAsync();

            return _mapper.Map<IncidentDto>(incident);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al resolver incidente {incidentId}", ex);
        }
    }

    /// <summary>
    /// Cuenta incidentes abiertos (status != Closed).
    /// 
    /// USO: Para dashboard - "Mostrar incidentes pendientes"
    /// </summary>
    public async Task<int> GetOpenIncidentsCountAsync(Guid tenantId)
    {
        try
        {
            return await Task.Run(() =>
                _context.Incidents
                    .Count(i => 
                        i.TenantId == tenantId && 
                        i.Status != "Closed" && 
                        !i.IsDeleted
                    )
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Error al contar incidentes abiertos", ex);
        }
    }
}
