#!/bin/bash
# Script para iniciar la API con manejo de errores

Set-Location "C:\Users\escuela03\Desktop\BACKENDZUPER"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. Compilando solución..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error en compilation" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Compilación exitosa" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "2. Iniciando API..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

Set-Location ".\ZuperBackend.API"
dotnet run --configuration Debug

Write-Host "✅ API iniciada" -ForegroundColor Green
