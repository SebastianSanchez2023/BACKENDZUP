using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZuperBackend.Application.Services;

namespace ZuperBackend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QRCodesController : ControllerBase
{
    private readonly IQRCodeService _qrCodeService;
    private readonly IDiagnosisService _diagnosisService;
    private readonly ILogger<QRCodesController> _logger;

    public QRCodesController(IQRCodeService qrCodeService, IDiagnosisService diagnosisService, ILogger<QRCodesController> logger)
    {
        _qrCodeService = qrCodeService;
        _diagnosisService = diagnosisService;
        _logger = logger;
    }

    /// <summary>
    /// Genera un nuevo código QR para un activo.
    /// Si ya existe uno, lo desactiva y genera uno nuevo (rotación).
    /// </summary>
    [HttpPost("generate/{assetId}")]
    public async Task<IActionResult> Generate(Guid assetId)
    {
        try
        {
            var tenantId = GetTenantIdFromToken();
            var userId = GetUserIdFromToken();

            var qrCode = await _qrCodeService.GenerateQRCodeAsync(tenantId, assetId, userId);
            
            // Generar la imagen en Base64 para que el cliente pueda mostrarla
            var base64Image = _qrCodeService.GenerateQRCodeImage(qrCode.QRCodeUrl!);

            return Ok(new
            {
                qrCode.Id,
                qrCode.AssetId,
                qrCode.QRCodeUrl,
                qrCode.Version,
                qrCode.CreatedAt,
                imageHtml = $"data:image/png;base64,{base64Image}"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generando QR: {ex.Message}");
            return StatusCode(500, new { error = "Error interno al generar el código QR" });
        }
    }

    /// <summary>
    /// Obtiene el QR activo de un activo.
    /// </summary>
    [HttpGet("asset/{assetId}")]
    public async Task<IActionResult> GetByAsset(Guid assetId)
    {
        try
        {
            var tenantId = GetTenantIdFromToken();
            var qrCode = await _qrCodeService.GetActiveQRCodeAsync(tenantId, assetId);

            if (qrCode == null)
                return NotFound(new { error = "No hay un QR activo para este activo" });

            var base64Image = _qrCodeService.GenerateQRCodeImage(qrCode.QRCodeUrl!);

            return Ok(new
            {
                qrCode.Id,
                qrCode.AssetId,
                qrCode.QRCodeUrl,
                qrCode.Version,
                imageHtml = $"data:image/png;base64,{base64Image}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error obteniendo QR: {ex.Message}");
            return StatusCode(500, new { error = "Error interno" });
        }
    }

    /// <summary>
    /// Endpoint público para simular el escaneo de un QR.
    /// No requiere autenticación para que cualquier usuario pueda escanear.
    /// </summary>
    [HttpGet("scan/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Scan(Guid id)
    {
        try
        {
            // Buscamos el QR por su ID o por el ID del activo
            var qrCode = await _qrCodeService.GetActiveQRCodeAsync(Guid.Empty, id);
            
            if (qrCode == null)
            {
                // Si no es un ID de QR, probamos si es un ID de activo directamente
                // En un sistema real, el QR tendría su propio token de seguridad
                return NotFound(new { error = "Código QR no válido o expirado" });
            }

            // Obtenemos la primera pregunta del diagnóstico para guiar al usuario
            var firstQuestion = await _diagnosisService.GetStartingNodeAsync();

            return Ok(new
            {
                message = "¡Escaneo exitoso!",
                assetId = qrCode.AssetId,
                scanTime = DateTime.UtcNow,
                diagnosisStart = firstQuestion,
                helpMessage = firstQuestion != null 
                    ? "Responda a la primera pregunta para continuar" 
                    : "No hay preguntas de diagnóstico configuradas todavía"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en escaneo: {ex.Message}");
            return StatusCode(500, new { error = "Error al procesar el escaneo" });
        }
    }

    private Guid GetTenantIdFromToken()
    {
        var claim = User.FindFirst("TenantId");
        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    private Guid GetUserIdFromToken()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }
}
