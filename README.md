# MauiAppPeliculasV2
Aplicacion de Blazor Hybrid version 2.0 con integracion de API

# README — MauiAppPeliculas (MAUI Blazor Hybrid)

# 📱 MauiAppPeliculas (MAUI + Blazor Hybrid)

Aplicación móvil/desktop (MAUI Blazor) que consume la **PeliculasApi**.  
Permite **login**, listar **películas** (cards), y según **rol** (`Admin`/`Usuario`), **crear/editar/eliminar**. Lo mismo para **usuarios** (solo Admin).

## ✨ Features
- Login y sesión en memoria.
- Roles:
  - **Admin**: CRUD completo de usuarios y películas.
  - **Usuario**: solo lectura.
- Listado de películas en **cards** con buscador.
- Imágenes por URL o desde `wwwroot/Imagenes/*`.
- Formularios modales y confirmación de borrado.
- Botón **Cerrar sesión** (redirige a `Login`).

## 🧱 Stack
- .NET 8 — .NET MAUI + BlazorWebView
- HttpClient + JSON
- Bootstrap para estilos (wwwroot)

## 📦 Estructura rápida
MauiAppPeliculas/
├─ wwwroot/
│ ├─ css/ (bootstrap, estilos)
│ └─ Imagenes/ (placeholders)
├─ Components/
│ └─ Pages/
│ ├─ Home.razor
│ ├─ Login.razor
│ ├─ TablaUsuario.razor
│ └─ TablaPelicula.razor
├─ Services/
│ ├─ UsuarioDtoServiceHttp.cs
│ └─ PeliculasDtoServiceHttp.cs
├─ Repositories/
│ ├─ UsuariosRepositoryHttp.cs
│ └─ PeliculasRepositoryHttp.cs
├─ Routes.razor / _Imports.razor
└─ MauiProgram.cs

bash
Copiar código

## 🔧 Configuración

1) **Clonar**
   
```bash
git clone https://github.com/<tu-usuario>/MauiAppPeliculas.git
cd MauiAppPeliculas
Base URL de la API
En MauiProgram.cs setear la dirección pública o local de tu API:

csharp
Copiar código
#if ANDROID
builder.Services.AddHttpClient("api", client =>
{
    // Android emulador -> host.docker interno: 10.0.2.2
    client.BaseAddress = new Uri("https://10.0.2.2:7179/"); // ajustá puerto
});
#else
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7179/"); // o URL de Somee
});
#endif
Y en tus servicios:

csharp
Copiar código
// UsuarioDtoServiceHttp / PeliculasDtoServiceHttp
public UsuarioDtoServiceHttp(IHttpClientFactory f) => _http = f.CreateClient("api");
Roles
El servicio de usuario guarda la sesión en UsuarioDtoServiceHttp.Sesion.
Sesion.Usuario.Rol == "Admin" habilita botones de Agregar/Editar/Eliminar (vista ya lo chequea).

▶️ Ejecutar
Windows: Debug > Windows Machine

Android: emulador; acordate de usar 10.0.2.2 para la API local

iOS/macOS: si tenés entorno Apple.

🔐 Login
En Login.razor, usa credenciales creadas por API (p.ej. admin@peliculas.com / admin123).
Tras login, Home muestra accesos a Películas y Usuarios (según rol).

🖼 Imágenes
En formularios podés indicar:

URL (https://...)

Nombre de archivo, y la app resolverá como Imagenes/<archivo> dentro de wwwroot.

🚪 Cerrar Sesión
El Home incluye botón Cerrar sesión que:

limpia UsuarioDtoServiceHttp.Logout()

redirige a /login

❗️Errores comunes
Pantalla “Not Found”: revisá Routes.razor y que exista la página Login.razor con @page "/login".

No carga API en Android: usa https://10.0.2.2:<puerto>/ como base URL.

405/400 al guardar: verificá que el servicio use las rutas exactas del controlador (por ejemplo api/Usuario vs api/Usuarios). La app ya fue ajustada a UsuarioController/PeliculasController.

🧪 QA rápido
Login como Admin → CRUD usuarios/películas OK.

Login como Usuario → solo lectura.

Imágenes: si la URL falla, la card usa un placeholder de wwwroot/Imagenes.

📄 Licencia
Uso académico.
