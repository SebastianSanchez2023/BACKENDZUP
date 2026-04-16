#!/bin/bash
# Script de pruebas para la API Zuper Backend
# Uso en PowerShell: Invoke-WebRequest o en Linux/Mac: curl

# ============================================
# 1. HEALTH CHECK - Verificar que la API funciona
# ============================================
echo "=== 1. Health Check ==="
curl -X GET "https://localhost:5001/api/health/check" \
     -H "Content-Type: application/json" \
     -k

echo ""
echo ""

# ============================================
# 2. LOGIN - Obtener JWT Token
# ============================================
echo "=== 2. Login (obtener token) ==="
curl -X POST "https://localhost:5001/api/auth/login" \
     -H "Content-Type: application/json" \
     -d '{
       "email": "admin@test.com",
       "password": "Admin@123456"
     }' \
     -k

echo ""
echo ""

# ============================================
# 3. VALIDATE TOKEN - Verificar que el token es válido
# ============================================
# Nota: Reemplaza YOUR_TOKEN con el accessToken devuelto en el paso 2
echo "=== 3. Validate Token (require bearer token) ==="
curl -X GET "https://localhost:5001/api/auth/validate" \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json" \
     -k

echo ""
