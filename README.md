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

## Tests

```powershell
dotnet test UCR.EB.BioMicroscopeAdmin.sln
```

GitHub Actions compila y corre los tests en cada push y PR hacia `main`.

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
