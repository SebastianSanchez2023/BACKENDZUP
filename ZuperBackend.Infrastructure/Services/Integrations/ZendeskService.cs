using Microsoft.Extensions.Logging;
using ZuperBackend.Application.Services;

namespace ZuperBackend.Infrastructure.Services.Integrations;

public class ZendeskService : IZendeskService
{
    private readonly ILogger<ZendeskService> _logger;

    public ZendeskService(ILogger<ZendeskService> logger)
    {
        _logger = logger;
    }

    public async Task<string> CreateTicketAsync(Guid assetId, string description)
    {
        _logger.LogInformation($"Creando TICKET en ZENDESK para el activo {assetId}...");
        _logger.LogInformation($"Detalle: {description}");

        // Simulación de llamada a API de Zendesk
        await Task.Delay(500);

        var ticketId = "ZD-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        _logger.LogInformation($"Ticket creado exitosamente: {ticketId}");

        return ticketId;
    }
}
