using AutoMapper;
using ZuperBackend.Application.DTOs.Asset;
using ZuperBackend.Application.DTOs.Diagnosis;
using ZuperBackend.Application.DTOs.Incident;
using ZuperBackend.Domain.Entities;

namespace ZuperBackend.Infrastructure.Mapping;

/// <summary>
/// Perfil de AutoMapper para mapeos de Activos (Assets).
/// 
/// CONCEPTO: Un Profile define las reglas de mapeo entre tipos.
/// AutoMapper encontrará automáticamente este Profile y lo registrará.
/// 
/// FLUJO:
/// CreateAssetDto → (AutoMapper según reglas) → Asset (Entity)
/// Asset (Entity) → (AutoMapper según reglas) → AssetDto
/// UpdateAssetDto → Asset (actualiza solo propiedades no-null)
/// </summary>
public class AssetMappingProfile : Profile
{
    /// <summary>
    /// Constructor donde se definen todas las reglas de mapeo de Assets.
    /// </summary>
    public AssetMappingProfile()
    {
        // ============================================================
        // MAPEOS: CreateAssetDto → Asset
        // ============================================================
        /// <summary>
        /// Cuando el cliente envía CreateAssetDto (formulario de creación),
        /// convertimos a Asset (Entity de dominio).
        /// 
        /// CAMPOS QUE MAPEAN AUTOMÁTICAMENTE:
        /// - CreateAssetDto.Name → Asset.Name
        /// - CreateAssetDto.SerialNumber → Asset.SerialNumber
        /// - CreateAssetDto.AssetType → Asset.AssetType
        /// - ... (todos los campos que tienen el mismo nombre)
        /// 
        /// CAMPOS NO MAPEADOS (asignados manualmente en el servicio):
        /// - Asset.Id (generado)
        /// - Asset.TenantId (asignado por tenant del request)
        /// - Asset.CreatedAt (timestamp)
        /// - Asset.CreatedBy (userId)
        /// - Asset.IsDeleted, DeletedAt, DeletedBy (soft delete)
        /// </summary>
        CreateMap<CreateAssetDto, Asset>();

        // ============================================================
        // MAPEOS: UpdateAssetDto → Asset (actualización parcial)
        // ============================================================
        /// <summary>
        /// Actualiza un Asset existente con valores de UpdateAssetDto.
        /// 
        /// USO: Cuando cliente hace PUT para actualizar un asset
        /// COMPORTAMIENTO: Solo actualiza propiedades que no sean null
        ///   (ForAllMembers con Ignore para nulls)
        /// 
        /// RESULTADO: Si UpdateAssetDto.Name es null, no sobrescribe Asset.Name
        /// </summary>
        CreateMap<UpdateAssetDto, Asset>()
            // Ignora propiedades null (update parcial)
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null));

        // ============================================================
        // MAPEOS: Asset → AssetDto (respuesta al cliente)
        // ============================================================
        /// <summary>
        /// Convierte una entidad Asset a AssetDto para enviar al cliente.
        /// 
        /// USO: Cuando el API devuelve un Asset (GET, POST, PUT)
        /// SEGURIDAD: AssetDto no expone campos internos como:
        ///   - DeletedBy, DeletedAt (solo expone IsDeleted si es necesario)
        ///   - CreatedBy, UpdatedBy (auditoria interna)
        /// 
        /// NOTA: AutoMapper automáticamente mapea campos con mismo nombre
        /// </summary>
        CreateMap<Asset, AssetDto>();
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de Incidentes.
/// </summary>
public class IncidentMappingProfile : Profile
{
    /// <summary>
    /// Constructor donde se definen todas las reglas de mapeo de Incidents.
    /// </summary>
    public IncidentMappingProfile()
    {
        // Mapeo: CreateIncidentDto → Incident
        CreateMap<CreateIncidentDto, Incident>();

        // Mapeo: UpdateIncidentDto → Incident (actualización parcial)
        CreateMap<UpdateIncidentDto, Incident>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null));

        // Mapeo: Incident → IncidentDto (respuesta al cliente)
        CreateMap<Incident, IncidentDto>();
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeos de Diagnosis.
/// </summary>
public class DiagnosisMappingProfile : Profile
{
    /// <summary>
    /// Constructor donde se definen todas las reglas de mapeo de Diagnosis.
    /// </summary>
    public DiagnosisMappingProfile()
    {
        // ============================================================
        // MAPEOS PARA NODOS
        // ============================================================
        
        // DiagnosisNode → DiagnosisNodeDto
        CreateMap<DiagnosisNode, DiagnosisNodeDto>();

        // CreateDiagnosisNodeDto → DiagnosisNode
        CreateMap<CreateDiagnosisNodeDto, DiagnosisNode>();

        // UpdateDiagnosisNodeDto → DiagnosisNode (actualización parcial)
        CreateMap<UpdateDiagnosisNodeDto, DiagnosisNode>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null));

        // ============================================================
        // MAPEOS PARA OPCIONES
        // ============================================================
        
        // DiagnosisOption → DiagnosisOptionDto
        CreateMap<DiagnosisOption, DiagnosisOptionDto>();

        // CreateDiagnosisOptionDto → DiagnosisOption
        CreateMap<CreateDiagnosisOptionDto, DiagnosisOption>();

        // UpdateDiagnosisOptionDto → DiagnosisOption (actualización parcial)
        CreateMap<UpdateDiagnosisOptionDto, DiagnosisOption>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null));
    }
}
