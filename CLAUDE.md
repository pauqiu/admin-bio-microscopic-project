# CLAUDE.md — Colección de microscopía (UCR · Escuela de Biología)

Contexto compartido para Claude Code. Todo el equipo trabaja con este archivo en la raíz del repo, así que mantenerlo corto y actualizado.

## Proyecto

Curso CI-0139 Administración de Proyectos (UCR/ECCI, G1, II-2026). Proyecto: **Mejora en las opciones de visualización de imágenes de la colección de microscopía de la Escuela de Biología**.

Equipo: cuatro integrantes (una persona PM y tres de desarrollo). La asignación por persona se maneja fuera de este repo público.

### Qué exige el Acta de Constitución (v1.0, 4-sep-2026)

Objetivos:
1. Consulta en línea de las láminas sin depender de la persona administradora.
2. Un solo repositorio con varias vistas por lámina.
3. Navegación y búsqueda por grupos, cajas, láminas y códigos.
4. La persona administradora solo carga y cura el contenido.
5. Vista protegida (acceso controlado a las imágenes).

Dentro del alcance: plataforma en línea desplegada en el servidor de la Escuela de Biología, vista protegida y control de acceso, búsqueda/navegación, documentación de uso.

Fuera del alcance: digitalización, préstamos, hardware, apps móviles nativas, contenido didáctico, mantenimiento posterior al semestre, **integración con sistemas institucionales UCR (no hay SSO UCR)**.

Restricciones: la estructura de la Escuela es fija, no hay presupuesto, las condiciones del servidor son desconocidas.

El trabajo se guía por la **lista de actividades (EDT)** del equipo, no solo por el Acta. La EDT no incluye autenticación ni roles, así que no se programan mientras no cambie por el Plan de Gestión de Cambios.

## Estado actual del código (verificado)

- Sin autenticación ni roles. Todos los endpoints son públicos.
- Las imágenes se guardan en `wwwroot/uploads` (`LocalFileStorageService`) y se sirven con `UseStaticFiles`.
- El detalle de la lámina muestra las imágenes como tarjetas, sin visor ni zoom.
- Una lámina admite hasta 6 imágenes ordenadas. El formulario de lámina nueva acepta varias fotos; el detalle sube una por una.
- Hay búsqueda de láminas por código o nombre y filtro por caja. Las cajas solo se filtran por grupo.
- CORS abierto (`AllowAnyOrigin`) solo en Development. En desarrollo el front corre en 5200 y el API en 5138.
- La contraseña de Postgres está en `appsettings.json` (`postgres/postgres`, solo desarrollo).

## Stack y mapa del repo

.NET 9, Clean Architecture, EF Core + PostgreSQL 17, Blazor WASM + MudBlazor, cliente HTTP escrito a mano (`BioMicroscopeApiClient`), shell de escritorio con Electron (heredado, no es parte del Acta).

- `Backend.Domain` · `Backend.Application` · `Backend.Infrastructure` · `Backend.Presentation` (endpoints minimal API en `Endpoints/`) · `Backend.DependencyInjection` · `Backend.Api` (host).
- `Frontend` (host WASM) · `Frontend.Domain` · `Frontend.Application` · `Frontend.Infrastructure` (cliente HTTP en `ApiClient/`) · `Frontend.Presentation` (páginas MudBlazor) · `Frontend.DependencyInjection`.
- Un proyecto `*.Tests.Unit` por capa.
- Entidades: Group → Box → Slide → Image (la lámina tiene varias imágenes con orden).
- Páginas: Home, Groups, Boxes, Slides, SlideDetail, GuiaUso.

## Comandos

```bash
docker compose up -d                       # Postgres (127.0.0.1:5432, db biomicroscope_admin)
dotnet run --project Backend.Api           # API en http://localhost:5138 (migra la BD al arrancar)
dotnet run --project Frontend              # Front en http://localhost:5200
dotnet test UCR.EB.BioMicroscopeAdmin.sln  # todos los tests
```

URL del API para el front: `Frontend/wwwroot/appsettings*.json` → `ApiBaseUrl`.

## Decisiones técnicas

1. **Sin auth por ahora.** No está en la EDT. Cualquier cambio pasa por el Plan de Gestión de Cambios.
2. **Cada paquete toca sus propios archivos** para que nadie tenga que esperar a otra persona (ver la tabla de abajo).
3. **Secretos fuera del repo.** La cadena de conexión de producción va por variables de entorno.

## Reglas de colaboración

- El repo oficial pertenece a la persona PM y todo el equipo tiene permiso de escritura. Se trabaja ahí, con una rama y un PR por paquete.
- Una rama por paquete: `feat/<paquete>-<tema>`. PR pequeños hacia `main`; al menos una persona revisa antes de mezclar.
- **Migraciones EF:** las crea solo quien toca el modelo, y se rebasa antes de crear otra. Si dos ramas crean migraciones, se borra la propia y se regenera sobre `main` (no mezclar `AppDbContextModelSnapshot.cs` a mano).
- Cada paquete trae sus tests en el proyecto `*.Tests.Unit` de la capa que toca.
- No subir `wwwroot/uploads`, `build/`, `Electron/dist` ni secretos.
- Cualquier cambio de alcance (no solo de código) pasa por el Plan de Gestión de Cambios del curso.

## Paquetes de trabajo según la EDT

Primero se mezclan el esqueleto y la EDT 1. Después cada paquete avanza por su cuenta.

| EDT | Paquete | Responsable | Archivos que toca | No toca |
|-----|---------|-------------|-------------------|---------|
| 1 | Visualización: ampliar, acercar y alejar | Dev 1 | `ImageViewerDialog.razor`, `ImageUrls.cs`, detalle de lámina | — |
| 2 | Carga simultánea de fotografías | Dev 3 | `ImageUploadDialog.razor`, servicio de imágenes, endpoint de carga | Páginas de grupos, cajas y láminas; visor |
| 3 y 4 | Navegación jerárquica, búsqueda y filtrado de cajas | Dev 2 | `GroupsPage.razor`, `BoxesPage.razor`, `SlidesPage.razor`, listado de cajas | Detalle de lámina e imágenes |
| 5 | Documentación y puesta en funcionamiento | PM | `docs/`, archivos de despliegue | Código de la app |

Si alguien necesita tocar un archivo de otro paquete, lo avisa antes en el grupo.
