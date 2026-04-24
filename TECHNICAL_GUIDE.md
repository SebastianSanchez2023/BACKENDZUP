# 🚀 ZuperBackend - Guía de Entrega Técnica

Este documento resume el estado actual del backend y los pasos necesarios para que los equipos de **Frontend** y **DevOps** puedan integrarlo y desplegarlo.

## 🛠️ Estado de Funcionalidad
El backend está construido sobre **.NET 8** siguiendo una **Arquitectura Limpia (Clean Architecture)**.

| Módulo | Estado | Descripción |
| :--- | :--- | :--- |
| **Autenticación** | ✅ 100% | JWT Bearer Tokens con roles (Admin/User). |
| **Gestión de Activos** | ✅ 100% | CRUD completo para equipos y dispositivos. |
| **Generación QR** | ✅ 100% | Generación dinámica de imágenes QR apuntando a la API. |
| **Flujo de Diagnóstico** | ✅ 100% | Motor de árbol de decisión para resolución de fallas. |
| **Integración Zuper** | ⚙️ 90% | Sincronización de activos (Falta URL/Key real). |
| **Integración Zendesk** | ⚙️ 90% | Creación de tickets automática (Falta URL/Key real). |

---

## 🔑 Configuración de Integraciones Externas
Para activar las licencias reales, se deben modificar los siguientes campos en `appsettings.json`:

### Zuper API
```json
"ZuperApi": {
  "BaseUrl": "https://api.zuper.com/v1", // Cambiar por URL oficial
  "ApiKey": "TU_API_KEY_AQUI"
}
```

### Zendesk API
*Nota: Se requiere actualizar el `ZendeskService.cs` en la capa de Infrastructure con el cliente HTTP real una vez obtenida la documentación de su sandbox.*

---

## 🗄️ Base de Datos
Actualmente utiliza **Entity Framework Core** con SQL Server LocalDB.
**Para Producción:**
1. Crear una base de datos SQL Server.
2. Actualizar la `DefaultConnection` en `appsettings.json`.
3. Ejecutar `dotnet ef database update` para migrar el esquema.

---

## 🎨 Guía para el Frontend
La documentación interactiva (Swagger) está disponible en: `http://localhost:5246/swagger`.

### Flujos Principales para el Frontend:
1. **Login:** `POST /api/Auth/login` -> Almacenar token en LocalStorage.
2. **Scan QR:** Al escanear el QR, el frontend debe llamar a `GET /api/QRCodes/scan/{id}`.
3. **Diagnóstico:** Usar `POST /api/diagnosis/process-option` enviando el `assetId` y el `optionId` seleccionado.

---

## 🚀 Próximos Pasos Sugeridos
1. **CRUD de Árbol de Diagnóstico:** Implementar interfaz web para gestionar las preguntas.
2. **Logs y Auditoría:** Implementar registro de quién escanea qué y cuándo.
3. **Manejo de Archivos:** Permitir que el usuario suba fotos del activo dañado desde el móvil.

---
**Desarrollado por Antigravity AI Coding Assistant** 🤖🦾
