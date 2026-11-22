# Colección de microscopía · Escuela de Biología (UCR)

Plataforma web para consultar y administrar las láminas de la colección de microscopía. Organiza el contenido en **Grupo → Caja → Lámina → Imágenes**.

Proyecto del curso CI-0139 Administración de Proyectos (UCR/ECCI, II-2026). El contexto y las reglas del equipo están en [CLAUDE.md](CLAUDE.md).

## Requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/), con el motor encendido

## Levantar el proyecto

Desde la raíz del repo, cada comando del API y del front en su propia terminal.

```powershell
docker compose up -d
```

```powershell
dotnet run --project Backend.Api
```

```powershell
dotnet run --project Frontend
```

| Servicio | Dirección |
|---|---|
| Front | http://localhost:5200 |
| API | http://localhost:5138 |
| Swagger | http://localhost:5138/swagger |
| Salud del API | http://localhost:5138/health |
| PostgreSQL | `127.0.0.1:5432`, base `biomicroscope_admin` |

El API aplica las migraciones al arrancar. Las credenciales de `appsettings.json` son solo para desarrollo.

## Usuarios de desarrollo

El API crea los usuarios iniciales desde la configuración. En local se guardan con *user secrets*, fuera del repo. Usá una contraseña propia de al menos 10 caracteres, con mayúscula, minúscula y número.

```powershell
dotnet user-secrets set "Auth:AdminEmail" "admin@biomicroscope.test" --project Backend.Api
```

```powershell
dotnet user-secrets set "Auth:AdminPassword" "<contraseña-admin>" --project Backend.Api
```

```powershell
dotnet user-secrets set "Auth:ReaderEmail" "lector@biomicroscope.test" --project Backend.Api
```

```powershell
dotnet user-secrets set "Auth:ReaderPassword" "<contraseña-lector>" --project Backend.Api
```

El usuario Lector de prueba nunca se crea en producción.

## API de sesión

Todas las rutas del API están bajo `/api`. Las escrituras exigen el header `X-Requested-With: BioMicroscope`.

| Método | Ruta | Acceso | Respuesta |
|---|---|---|---|
| POST | `/api/auth/login` | Público | `200 { email, roles }` o `401` |
| POST | `/api/auth/logout` | Con sesión | `204` |
| GET | `/api/auth/me` | Con sesión | `200 { email, roles }` o `401` |

Las lecturas requieren sesión. Las escrituras requieren el rol `Administrador` y responden `403` a un Lector.

## Tests

```powershell
dotnet test UCR.EB.BioMicroscopeAdmin.sln
```

Los tests de integración levantan su propio PostgreSQL, así que Docker debe estar encendido. GitHub Actions compila y corre todos los tests en cada push y PR hacia `main`.

Las migraciones se crean con la herramienta local del repo:

```powershell
dotnet tool restore
```

```powershell
dotnet ef migrations add <Nombre> --project Backend.Infrastructure --startup-project Backend.Infrastructure --output-dir Persistence/Migrations
```

## Estructura

| Carpeta | Responsabilidad |
|---|---|
| `Backend.Domain` | Entidades y reglas del negocio |
| `Backend.Application` | Casos de uso e interfaces |
| `Backend.Infrastructure` | EF Core, PostgreSQL y almacenamiento |
| `Backend.Presentation` | Endpoints minimal API y handlers |
| `Backend.DependencyInjection` | Registro de servicios |
| `Backend.Api` | Host del backend |
| `Frontend` | Host Blazor WebAssembly |
| `Frontend.Domain`, `Frontend.Application` | Modelos y servicios del front |
| `Frontend.Infrastructure` | Cliente HTTP (`BioMicroscopeApiClient`) |
| `Frontend.Presentation` | Páginas y componentes MudBlazor |
| `*.Tests.Unit` | Tests de cada capa |

## Cómo colaborar

1. Crear una rama `feat/<paquete>-<tema>` desde `main`.
2. Incluir tests en el proyecto `*.Tests.Unit` de la capa que se toca.
3. Abrir un PR pequeño hacia `main`. Al menos una persona lo revisa antes de mezclar.

No subir `wwwroot/uploads`, `build/`, `Electron/dist` ni secretos.
