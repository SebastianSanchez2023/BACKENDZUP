namespace ZuperBackend.Application.Services;

public interface IZuperService
{
    /// <summary>
    /// Sincroniza un activo local con la plataforma de Zuper.
    /// </summary>
    /// <param name="assetId">ID del activo local</param>
    /// <returns>El ID del activo en la plataforma de Zuper</returns>
    Task<string> SyncAssetAsync(Guid assetId);

    /// <summary>
    /// Crea una orden de trabajo (Job) en Zuper basada en un incidente.
    /// </summary>
    Task<string> CreateJobAsync(Guid incidentId);
}
