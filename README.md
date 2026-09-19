# Programa de acompañamiento a pacientes

Prueba técnica de ingreso — TBTB Global. Cubre CA-1 Registro de pacientes, CA-2 registro de contacto y CA-3 corrección de contacto con historial de auditoría. El resto del alcance, y por qué se dejó fuera, está documentado en [`02-plan.md`](02-plan.md).

Antes de tocar código está [`01-hallazgos.md`](01-hallazgos.md) (lectura crítica del PRD) y [`02-plan.md`](02-plan.md) (alcance, modelo de datos, contrato de API). [`03-bitacora.md`](03-bitacora.md) tiene la matriz de trazabilidad y el registro de decisiones.

## Stack

- **Base de datos:** SQL Server, esquema en scripts `.sql` versionados (`scripts/`)
- **API:** .NET 8 / ASP.NET Core, EF Core como ORM (sin migraciones propias — el esquema lo mandan los scripts)
- **Web:** Angular 17, componentes standalone, servicios inyectados
- **Todo corre en Docker** — no hace falta instalar SQL Server, .NET ni Node en la máquina para garantizar que cualquiera pueda clonar y levantar el proyecto. Instrucciones en la sección "Levantar el proyecto" más abajo.

## Levantar el proyecto

Requisito único: Docker Desktop corriendo.

```bash
git clone https://github.com/MateoAguirreO/Prueba-tbtb-global.git
cd <carpeta-del-repo>
cp .env.example .env
docker compose up -d --build
```

Eso levanta 4 contenedores: `db` (SQL Server, con healthcheck), `migrate` (aplica los scripts de `scripts/` en orden y se apaga), `api` y `web`. La primera vez tarda unos minutos por las imágenes; las siguientes son rápidas.

Cuando termine:

- **App:** http://localhost:4200
- **API:** http://localhost:8080/api/health → `{"status":"ok","database":"ok"}` confirma que la API está viva y conectada a la base

La base arranca con 8 pacientes y 9 contactos de prueba (`scripts/06-seed-data.sql`), para no tener que inventar datos a mano.

Para bajarlo: `docker compose down`. Para bajarlo y borrar también los datos: `docker compose down -v` (la próxima subida vuelve a sembrar desde cero).

## Correr las pruebas del backend

El proyecto de pruebas (`api/Api.Tests`, xUnit) no necesita SQL Server — usa EF Core InMemory y SQLite en memoria. Si tienes el SDK de .NET 8 instalado:

```bash
cd api/Api.Tests
dotnet test
```

Si no lo tienes instalado localmente, corre igual dentro de Docker:

```bash
docker run --rm -v "$(pwd)/api":/src -w /src/Api.Tests mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

## Lint del frontend

```bash
cd web
npm install
npx ng lint
```

## Estructura

```
raiz/
├── 01-hallazgos.md
├── 02-plan.md
├── 03-bitacora.md
├── docker-compose.yml
├── .env.example
├── scripts/          esquema SQL versionado + datos de prueba
├── api/              servicio .NET (api/Api.Tests para las pruebas)
└── web/              cliente Angular
```
