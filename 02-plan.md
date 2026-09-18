# Plan y cierre de alcance

## 1. Alcance cerrado

Cubro CA-1, CA-2 y CA-3: alta de paciente, registro de contacto y corrección de un contacto ya registrado. Es el ciclo completo de un contacto de principio a fin, y conecta directo con el hallazgo más fuerte que saqué: que un registro no se puede sobrescribir sin dejar rastro. Prefiero que esa regla funcione bien a meterle un cuarto criterio de relleno.

- **CA-1** — el gestor registra un paciente nuevo y queda disponible para agendar contactos.
- **CA-2** — el gestor registra un contacto (fecha, canal, resultado) asociado a un paciente.
- **CA-3** — el gestor corrige un contacto con error, y el valor original queda accesible, no se pierde.

## 2. Fuera de alcance

- CA-4 y CA-5 dependen de preguntas que dejé abiertas al PO en los hallazgos (5 y 6). No las voy a resolver a adivinas solo para sumar un criterio.
- CA-6 depende de definir "paciente activo", que también quedó pendiente.
- Autorregistro del paciente: el modelo lo deja contemplado (teléfono nullable), pero no construyo el flujo ni la pantalla.
- Login/roles: modelo un `Gestor` mínimo porque lo necesito para atribuir contactos y correcciones, pero sin autenticación real.
- Consentimiento informado por país: quedó como riesgo en hallazgos, no lo toco acá.

## 3. Modelo de datos

**Paciente**
| Campo | Tipo | Restricción |
|---|---|---|
| Id | uniqueidentifier | PK |
| Nombre | nvarchar(200) | requerido |
| DocumentoIdentidad | nvarchar(30) | requerido, único |
| Telefono | nvarchar(20) | nullable |
| Correo | nvarchar(200) | nullable |
| Ciudad | nvarchar(100) | requerido |
| FechaInicioTratamiento | date | requerido |
| Estado | enum: `Incompleto`, `Activo` | default `Incompleto` si falta teléfono |
| FechaCreacion | datetime2 | default getutcdate() |

**Gestor**
| Campo | Tipo | Restricción |
|---|---|---|
| Id | uniqueidentifier | PK |
| Nombre | nvarchar(200) | requerido |

**Contacto**
| Campo | Tipo | Restricción |
|---|---|---|
| Id | uniqueidentifier | PK |
| PacienteId | uniqueidentifier | FK → Paciente |
| GestorId | uniqueidentifier | FK → Gestor |
| Fecha | date | requerido |
| Canal | enum: `Llamada`, `WhatsApp`, `Correo` | requerido |
| Resultado | enum: `Contactado`, `NoContesta`, `Rechazada`, `DatoInvalido` | requerido |
| FechaRegistro | datetime2 | default getutcdate() |

**ContactoHistorial**
| Campo | Tipo | Restricción |
|---|---|---|
| Id | uniqueidentifier | PK |
| ContactoId | uniqueidentifier | FK → Contacto |
| GestorId | uniqueidentifier | FK → Gestor (quién corrigió) |
| CampoModificado | nvarchar(50) | requerido |
| ValorAnterior | nvarchar(200) | requerido |
| ValorNuevo | nvarchar(200) | requerido |
| Motivo | nvarchar(300) | requerido |
| FechaCambio | datetime2 | default getutcdate() |

Corregir un contacto nunca es un UPDATE directo: el endpoint escribe primero en `ContactoHistorial` (un registro por campo que cambia) y después actualiza `Contacto`. Nada de DELETE. Así el valor viejo sigue ahí si alguien lo necesita.

## 4. Contrato de la interfaz

**`POST /api/pacientes`** — CA-1

- Entrada: `nombre`, `documentoIdentidad`, `telefono`, `correo?`, `ciudad`, `fechaInicioTratamiento` (teléfono obligatorio en este endpoint — es el registro del gestor, y el PRD lo pide así; queda nullable solo a nivel de tabla para no bloquear un futuro autorregistro)
- Salida: `201` con el paciente creado
- Errores: `400` datos inválidos · `409` documento duplicado

**`POST /api/pacientes/{pacienteId}/contactos`** — CA-2

- Entrada: `fecha`, `canal`, `resultado`, `gestorId`
- Salida: `201` con el contacto creado
- Errores: `400` datos inválidos o fuera de catálogo · `404` paciente no existe

**`PUT /api/contactos/{id}`** — CA-3

- Entrada: `gestorId`, `motivo`, y los campos a corregir (`fecha?`, `canal?`, `resultado?`)
- Salida: `200` con el contacto actualizado
- Errores: `400` falta motivo o no hay nada que corregir · `404` contacto no existe

**`GET /api/contactos/{id}/historial`** — no es un CA, pero es cómo demuestro que CA-3 realmente audita.

**`GET /api/pacientes/{id}/contactos`** — esta es la consulta con criterio que pide la Parte III (Stack, punto 5). Junta `Paciente`, `Contacto` y `Gestor` para traer el historial de contactos de un paciente con quién lo registró, ordenado por fecha. Con volumen real necesitaría un índice en `Contacto(PacienteId, Fecha)`, porque ese es exactamente el filtro y el orden que usa la consulta cada vez que se abre el detalle de un paciente.

Pantallas: registro de paciente, detalle de paciente con lista de contactos + alta de contacto, y edición de contacto (motivo obligatorio) con su historial debajo.

## 5. Secuencia de trabajo

Cada bloque de CA es un commit: endpoint y su prueba xUnit juntos, no en un commit de tests separado al final. Así la matriz de trazabilidad de `03-bitacora.md` apunta a un commit limpio por criterio.

| Bloque | Tarea                                                       | Tiempo |
| ------ | ------------------------------------------------------------ | ------ |
| 1      | docker-compose + scaffolding (`api/`, `web/`, `scripts/`)   | 30 min |
| 2      | Scripts SQL: Paciente, Gestor, Contacto, ContactoHistorial  | 45 min |
| 3      | API: entidades, DTOs, acceso a datos                        | 40 min |
| 4      | API: CA-1 + prueba                                          | 40 min |
| 5      | API: CA-2 + prueba                                          | 40 min |
| 6      | API: CA-3 + escritura en historial + prueba                 | 55 min |
| 7      | Consulta con criterio (`GET /pacientes/{id}/contactos`)     | 20 min |
| 8      | Angular: servicios + las 3 pantallas                         | 60 min |
| 9      | Script de datos de prueba                                   | 20 min |
| 10     | README, bitácora, probar en limpio con `docker compose up`  | 30 min |

## 6. Riesgos

SQL Server en contenedor no queda listo de inmediato, así uso healthcheck y hago que la API espere a que esté sano antes de migrar

Tres capas más Docker en pocas horas es ajustado. Si se me acaba el tiempo, recorto estética de UI antes que pruebas o el flujo funcionando.

Y en `web/`, si dejo la imagen de Node sin especificar la versión en el Dockerfile, un día el build se rompe solo porque cambió la versión. Fijo la versión exacta.

Replicabilidad en otro equipo: para garantizar que cualquiera pueda clonar y levantar el proyecto, pongo todo en Docker y dejo un README con instrucciones claras.

## 6.1 Docker

Con `docker compose up` desde la raíz levanta todo, sin instalar SQL Server, .NET ni Node en la máquina así cualquiera lo clona y lo prueba sin pelear con el entorno.

- `db`: imagen oficial de SQL Server, con healthcheck y volumen para que los datos sobrevivan a un reinicio.
- `migrate`: contenedor que corre una vez, después de que `db` esté sano, aplica los scripts de `scripts/` en orden y se apaga.
- `api`: build en dos etapas (SDK para compilar, runtime para correr), espera a `migrate`.
- `web`: build en dos etapas con Node y luego nginx sirviendo el Angular ya compilado.

Contraseñas y cadena de conexión van por variables de entorno, con `.env.example` en el repo y `.env` real en `.gitignore`.

## 7. Extensión móvil

El gestor en campo suele estar sin señal, así que en el teléfono guardo localmente lo que va registrando o corrigiendo (SQLite si es Ionic, IndexedDB si se queda en navegador), con un id generado en el mismo dispositivo para que dos gestores no choquen creando contactos al mismo tiempo.

Sincroniza sola apenas hay señal, o el gestor la fuerza a mano, mandando primero lo más viejo pendiente.

Lo complicado es cuando el mismo contacto se corrigió en el servidor mientras el teléfono estaba desconectado y trae otra corrección distinta pendiente. Como el historial nunca sobrescribe nada, técnicamente no se pierde ninguna de las dos — pero igual hay que elegir cuál queda como valor vigente. Gana la más reciente por reloj del servidor al sincronizar, y la otra queda en el historial marcada como conflicto, para que el gestor la vea y decida si la vuelve a aplicar. Lo que no hago es dejar que el teléfono pise en silencio algo que ya cambió en el servidor.
