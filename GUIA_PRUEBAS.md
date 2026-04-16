# 📋 GUÍA DE PRUEBAS - Backend Zuper + Zendesk

## 🚀 Estado Actual del Backend

✅ **Completado:**
- Autenticación JWT (Login/Token)
- Base de datos con 8 entidades
- Controladores Auth (Login, Validate)
- Multi-tenant preparado
- Seeder con datos de prueba

## 🔧 Requisitos para Probar

1. **SDK .NET 8** (ya instalado)
2. **SQL Server LocalDB** (ya en tu máquina)
3. **Port 5001** libre (HTTPS)
4. **PowerShell** o **Postman** para hacer requests

---

## 🎯 PASOS PARA PROBAR

### 1️⃣ Iniciar la API

```powershell
cd C:\Users\escuela03\Desktop\BACKENDZUPER\ZuperBackend.API
dotnet run
```

**Debería ver:**
```
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/2 GET https://localhost:5001/swagger/index.html
```

Si ve esto significa que **LA API ESTÁ LISTA** ✅

---

### 2️⃣ Acceder a la Documentación Interactiva (Swagger/OpenAPI)

Abre en tu navegador:
```
https://localhost:5001/swagger/index.html
```

Deberías ver una interfaz con todos los endpoints documentados.

---

### 3️⃣ PRUEBA 1: Health Check

**Endpoint:** `GET /api/health/check`

**Sin autenticación requerida**

**Respuesta esperada (200 OK):**
```json
{
  "status": "API is running",
  "timestamp": "2026-04-16T16:30:45.1234567Z",
  "version": "1.0.0"
}
```

---

### 4️⃣ PRUEBA 2: Login (obtener JWT Token)

**Endpoint:** `POST  /api/auth/login`

**Credenciales de prueba:**
- Email: `admin@test.com`
- Contraseña: `Admin@123456`

**Request:**
```json
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "admin@test.com",
  "password": "Admin@123456"
}
```

**Respuesta esperada (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "8yK7mP9vR2qT5xZ1wA3sB6lN...",
  "expiresIn": 3600,
  "user": {
    "id": "guid-del-usuario",
    "fullName": "Administrador de Prueba",
    "email": "admin@test.com",
    "role": "Admin",
    "tenantId": "guid-del-tenant"
  }
}
```

**⚠️ Importante:** Copia el `accessToken`, lo vas a necesitar en las próximas pruebas.

---

### 5️⃣ PRUEBA 3: Validar Token

**Endpoint:** `GET /api/auth/validate`

**Headers requeridos:**
```
Authorization: Bearer YOUR_ACCESS_TOKEN_HERE
Content-Type: application/json
```

**Reemplaza `YOUR_ACCESS_TOKEN_HERE` con el token obtenido en el paso anterior.**

**Respuesta esperada (200 OK):**
```json
{
  "message": "Token válido",
  "userId": "guid-del-usuario",
  "tenantId": "guid-del-tenant",
  "timestamp": "2026-04-16T16:31:20.5678901Z"
}
```

---

## 🧪 Script en PowerShell para Automatizar Pruebas

Guarda este archivo como `test-api-completo.ps1` y ejecuta:

```powershell
# Ignorar certificados SSL autofirmados
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

$baseUrl = "https://localhost:5001"

Write-Host "`n========== TEST 1: Health Check ==========" -ForegroundColor Green

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/health/check" -Method Get
    Write-Host "✅ API está corriendo" -ForegroundColor Green
    $response | ConvertTo-Json | Write-Host
} catch {
    Write-Host "❌ Error: " + $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host "`n========== TEST 2: Login ==========" -ForegroundColor Green

$loginBody = @{
    email = "admin@test.com"
    password = "Admin@123456"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
        -Method Post `
        -ContentType "application/json" `
        -Body $loginBody
    
    Write-Host "✅ Login exitoso" -ForegroundColor Green
    $loginResponse | ConvertTo-Json | Write-Host
    
    $token = $loginResponse.accessToken
} catch {
    Write-Host "❌ Error en login: " + $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host "`n========== TEST 3: Validate Token ==========" -ForegroundColor Green

try {
    $validateResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/validate" `
        -Method Get `
        -Headers @{ Authorization = "Bearer $token" }
    
    Write-Host "✅ Token válido" -ForegroundColor Green
    $validateResponse | ConvertTo-Json | Write-Host
} catch {
    Write-Host "❌ Error validando token: " + $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host "`n✅ TODAS LAS PRUEBAS PASARON" -ForegroundColor Green
```

**Para ejecutar:**
```powershell
.\test-api-completo.ps1
```

---

## 🔗 Endpoints de Swagger

Una vez que la API está en desarrollo, puedes usar Swagger UI en:
```
https://localhost:5001/swagger/index.html
```

Desde ahí puedes:
1. **Ver todos los endpoints** documentados
2. **Probar endpoint s interactivamente** sin herramientas externas
3. **Ver esquemas de request/response**

---

## 🚨 Si algo falla

Si ves un error como:

**Error:** "Failed executing DbCommand"
→ Significa que hay un error en la BD. Verifica que SQL Server LocalDB esté corriendo.

**Error:** "No se puede conectar con el servidor remoto"
→ La API no está iniciada o el puerto 5001 no está libre.

**Error:** "Invalid token"
→ Asegúrate de copiar el token completo del `accessToken`.

---

## 📝 Siguientes Pasos

Una vez que confir mes que la API funciona:

1. **Crear Controlador de Assets** (GET, POST, byId)
2. **Crear Controlador de Incidents** (POST create, GET byId, GET historial)
3. **Crear Controlador de QR** (Validar QR escaneado)
4. **Integración Zendesk** (crear tickets automáticamente)
5. **Integración Zuper** (crear OT automáticamente)

---

Haceme saber cuando la API esté corriendo y tú confirmame si los tests pasaron ✅
