# Script de pruebas para la API Zuper Backend
# Ejecutar en PowerShell

$baseUrl = "https://localhost:5001"

# Ignorar certificados SSL autofirmados en desarrollo
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "1. HEALTH CHECK - Verificar que la API funciona" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan

$response = Invoke-RestMethod -Uri "$baseUrl/api/health/check" `
    -Method Get `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "Respuesta:" -ForegroundColor Yellow
$response | ConvertTo-Json | Write-Host

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "2. LOGIN - Obtener JWT Token" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan

$loginBody = @{
    email = "admin@test.com"
    password = "Admin@123456"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body $loginBody `
    -SkipCertificateCheck

Write-Host "Respuesta:" -ForegroundColor Yellow
$loginResponse | ConvertTo-Json | Write-Host

# Extraer el token
$accessToken = $loginResponse.accessToken
Write-Host ""
Write-Host "Token obtenido (primeros 50 caracteres):" -ForegroundColor Yellow
Write-Host $accessToken.Substring(0, 50) + "..." -ForegroundColor Magenta

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "3. VALIDATE TOKEN - Verificar que el token es válido" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan

$validateResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/validate" `
    -Method Get `
    -ContentType "application/json" `
    -Headers @{ Authorization = "Bearer $accessToken" } `
    -SkipCertificateCheck

Write-Host "Respuesta:" -ForegroundColor Yellow
$validateResponse | ConvertTo-Json | Write-Host

Write-Host ""
Write-Host "✅ Pruebas completadas exitosamente" -ForegroundColor Green
