using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZuperBackend.Application.Services;
using ZuperBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ZuperBackend.Infrastructure.Services.Integrations;

public class ZuperService : IZuperService
{
    private readonly HttpClient _httpClient;
    private readonly ZuperBackendDbContext _dbContext;
    private readonly ILogger<ZuperService> _logger;
    private readonly string _apiKey;

    public ZuperService(
        HttpClient httpClient, 
        ZuperBackendDbContext dbContext, 
        IConfiguration configuration,
        ILogger<ZuperService> logger)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
        
        _apiKey = configuration["ZuperApi:ApiKey"] ?? "";
        var baseUrl = configuration["ZuperApi:BaseUrl"] ?? "https://api.zuperpro.com/v2/";
        
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // Zuper v2 usa x-api-key en el header
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }
    }

    public async Task<string> SyncAssetAsync(Guid assetId)
    {
        try
        {
            var asset = await _dbContext.Assets.FindAsync(assetId);
            if (asset == null) return "ASSET_NOT_FOUND";

            _logger.LogInformation($"Sincronizando activo {asset.Name} con Zuper Pro Real API...");

            // Estructura para crear un JOB en Zuper v2
            var jobData = new
            {
                job = new
                {
                    job_title = $"Mantenimiento: {asset.Name}",
                    job_description = $"Falla reportada vía QR para el activo {asset.Name} ({asset.SerialNumber}). Ubicación: {asset.Location}",
                    priority = "MEDIUM",
                    category = "Reparación"
                }
            };

            var json = JsonSerializer.Serialize(jobData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("jobs", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Respuesta de Zuper: {responseBody}");
                
                // Intentar extraer el ID del Job
                using var doc = JsonDocument.Parse(responseBody);
                string realId = "CREATED_SUCCESSFULLY";
                
                if (doc.RootElement.TryGetProperty("data", out var data) && 
                    data.TryGetProperty("job_id", out var idProp))
                {
                    realId = idProp.GetString() ?? realId;
                }
                
                return realId;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error API Zuper: {response.StatusCode} - {error}");
                return $"ERROR_API_{response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fatal en ZuperService: {ex.Message}");
            return "ZUPER_CONNECTION_ERROR";
        }
    }

    public async Task<string> CreateJobAsync(Guid incidentId)
    {
        // Este método se puede mapear a la misma lógica de creación de Jobs
        return await SyncAssetAsync(incidentId); 
    }
}
