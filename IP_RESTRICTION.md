# 🔒 Restricción de IP - E-Commerce API

## 📋 Resumen
La API está configurada para permitir acceso únicamente desde la IP `187.155.101.200`.

## ⚙️ Configuración

### Producción (`appsettings.json`)
```json
{
  "AllowedIP": "187.155.101.200"
}
```

### Desarrollo (`appsettings.Development.json`)
```json
{
  "AllowedIP": "127.0.0.1"
}
```

## 🛡️ Funcionamiento

### Middleware de Seguridad
- **Ubicación**: `Program.cs` (líneas 75-105)
- **Función**: Verifica la IP del cliente antes de procesar cualquier solicitud
- **Métodos de detección de IP**:
  - `X-Real-IP` header
  - `X-Forwarded-For` header  
  - `RemoteIpAddress` directo

### Comportamiento por Entorno

#### 🏗️ Desarrollo (Development)
- ✅ Permite acceso desde `127.0.0.1` (localhost)
- ✅ Permite acceso desde `::1` (IPv6 localhost)
- ✅ Permite acceso desde `localhost`
- ❌ Bloquea otras IPs

#### 🚀 Producción
- ✅ Solo permite acceso desde `187.155.101.200`
- ❌ Bloquea todas las demás IPs

## 📊 Respuestas de Error

### IP No Autorizada (403 Forbidden)
```json
{
  "Message": "Acceso denegado",
  "Error": "Tu IP no está autorizada para acceder a esta API",
  "YourIP": "192.168.1.100",
  "AllowedIP": "187.155.101.200",
  "Environment": "Production"
}
```

## 🔍 Logs de Debugging

El middleware registra en consola:
```
🌐 IP del cliente: 192.168.1.100
🔒 IP autorizada: 187.155.101.200
🏗️ Entorno: Production
❌ Acceso denegado - IP no autorizada
```

## 🧪 Testing

### Para probar localmente:
1. La aplicación en desarrollo permite `localhost`
2. Usa el archivo `test-validations.http`
3. Las peticiones funcionarán desde tu máquina local

### Para probar desde IP externa:
1. Solo `187.155.101.200` tendrá acceso
2. Otras IPs recibirán error 403
3. El error incluye información detallada

## 🔧 Modificación de IPs

### Cambiar IP autorizada:
1. Edita `appsettings.json`
2. Cambia el valor de `AllowedIP`
3. Reinicia la aplicación

### Agregar múltiples IPs:
```csharp
// En Program.cs, modificar la validación:
var allowedIps = new[] { "187.155.101.200", "192.168.1.100" };
if (!allowedIps.Contains(clientIp))
{
    // Bloquear acceso
}
```

## ⚠️ Consideraciones

- **Proxies**: El middleware maneja headers de proxy comunes
- **Load Balancers**: Compatible con `X-Forwarded-For`
- **IPv6**: Soporte completo para IPv6
- **Logging**: Todos los intentos de acceso se registran

## 🚀 Despliegue

En producción, asegúrate de:
1. ✅ Configurar la IP correcta en `appsettings.json`
2. ✅ Verificar que el entorno sea `Production`
3. ✅ Probar acceso desde la IP autorizada
4. ✅ Verificar logs de acceso 