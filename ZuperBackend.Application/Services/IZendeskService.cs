namespace ZuperBackend.Application.Services;

public interface IZendeskService
{
    /// <summary>
    /// Crea un ticket en Zendesk para reportar una falla de un activo.
    /// </summary>
    /// <param name="assetId">ID del activo</param>
    /// <param name="description">Descripción del fallo</param>
    /// <returns>ID del ticket creado en Zendesk</returns>
    Task<string> CreateTicketAsync(Guid assetId, string description);
}
