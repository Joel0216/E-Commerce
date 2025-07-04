# 🚀 Despliegue en Somee - E-Commerce API

## 📋 Resumen
Guía paso a paso para publicar la API E-Commerce en Somee.com

## 📁 Archivos Listos para Subir

### Carpeta: `./publish/`
Contiene todos los archivos necesarios para el despliegue:

```
publish/
├── E-Commerce.dll (aplicación principal)
├── E-Commerce.exe (ejecutable)
├── web.config (configuración IIS)
├── appsettings.json (configuración general)
├── appsettings.Production.json (configuración producción)
├── appsettings.Development.json (configuración desarrollo)
└── [archivos .dll y dependencias]
```

## 🌐 Pasos para Subir a Somee

### **1. Acceder a Somee.com**
- Ve a [somee.com](https://somee.com)
- Inicia sesión en tu cuenta
- Ve al panel de control

### **2. Crear/Seleccionar Sitio Web**
- Si no tienes un sitio, crea uno nuevo
- Selecciona tu sitio web existente
- Ve a la sección "File Manager" o "Administrador de Archivos"

### **3. Subir Archivos**
- **Método A: Subida Masiva**
  1. Selecciona todos los archivos de la carpeta `publish/`
  2. Comprime en un archivo ZIP
  3. Sube el ZIP a Somee
  4. Extrae el contenido en la raíz del sitio

- **Método B: Subida Individual**
  1. Sube cada archivo de `publish/` uno por uno
  2. Mantén la estructura de carpetas

### **4. Configuración del Sitio**
- **Framework**: .NET Core
- **Versión**: .NET 9.0
- **Tipo**: API Web
- **Puerto**: 80 (HTTP) / 443 (HTTPS)

### **5. Configurar Variables de Entorno**
En el panel de Somee, configura:
```
ASPNETCORE_ENVIRONMENT=Production
```

## 🔧 Configuración Específica

### **Base de Datos**
- ✅ Ya configurada en `appsettings.json`
- ✅ Apunta a: `E-Commerce01.mssql.somee.com`
- ✅ Base de datos: `E-Commerce01`

### **Restricción de IP**
- ✅ Solo permite acceso desde: `187.155.101.200`
- ✅ Configurado en `appsettings.Production.json`

### **JWT Authentication**
- ✅ Clave configurada
- ✅ Issuer: `miapi.com`
- ✅ Audience: `miapi.com`

## 🧪 Testing Post-Despliegue

### **1. Verificar Acceso**
```bash
# Desde IP autorizada (187.155.101.200)
curl https://tu-sitio.somee.com/api/products

# Desde IP no autorizada
curl https://tu-sitio.somee.com/api/products
# Debería devolver: 403 Forbidden
```

### **2. Endpoints de Prueba**
- `GET /api/products` - Listar productos (público)
- `GET /api/products/1` - Obtener producto (público)
- `POST /api/products` - Crear producto (requiere token)
- `PUT /api/products/1` - Actualizar producto (requiere token)
- `DELETE /api/products/1` - Eliminar producto (requiere token)

### **3. Validaciones**
- ✅ Stock no puede ser negativo
- ✅ Precio debe ser mayor a 0
- ✅ Nombre es obligatorio

## 🔍 Troubleshooting

### **Error 500 - Internal Server Error**
1. Verificar logs en Somee
2. Revisar configuración de base de datos
3. Verificar variables de entorno

### **Error 403 - Forbidden**
1. Verificar IP del cliente
2. Confirmar configuración de `AllowedIP`
3. Revisar logs de la aplicación

### **Error de Conexión a Base de Datos**
1. Verificar credenciales en `appsettings.json`
2. Confirmar que la base de datos esté activa
3. Verificar firewall de Somee

## 📊 Monitoreo

### **Logs Disponibles**
- Logs de aplicación en Somee
- Logs de base de datos
- Logs de acceso IP

### **Métricas a Monitorear**
- Tiempo de respuesta
- Errores 403 (IPs bloqueadas)
- Errores de validación
- Conexiones a base de datos

## 🔐 Seguridad

### **Implementado**
- ✅ Restricción de IP
- ✅ JWT Authentication
- ✅ Validaciones de datos
- ✅ HTTPS obligatorio

### **Recomendaciones**
- Cambiar clave JWT en producción
- Monitorear logs de acceso
- Configurar alertas de seguridad

## 📞 Soporte

Si tienes problemas:
1. Revisar logs en Somee
2. Verificar configuración
3. Contactar soporte de Somee
4. Revisar documentación de .NET Core en Somee 