# Bitacora de desarrollo Prueba TBTB

Uso claude code con Sonnet5 como modelo

## 1. Documentos base

- [01-hallazgos.md](01-hallazgos.md) — Hallazgos sobre el PRD
- [02-plan.md](02-plan.md) — Plan de desarrollo y modelo de datos
- [03-bitacora.md](03-bitacora.md) — Bitácora de desarrollo

## 2. Matriz de trazabilidad

| # | criterio de aceptación | commit o archivos relacionados | Prueba de verificacion | estado (cubierto, parcial, fuera de alcance) |
|---|---|---|---|---|
| 1 | CA-1 | https://github.com/MateoAguirreO/Prueba-tbtb-global/commit/f175a0df4a2c8c57d2823bd1ac080736a50a6b77 | Api.Tests/PacienteServiceTests.cs: CA1_CrearPaciente_ConDatosValidos_QuedaActivoYDisponibleParaAgendarContactos, CA1_CrearPaciente_ConDocumentoDuplicado_LanzaDocumentoDuplicadoException | Cubierto |
| 2 | CA-2 | pendiente | | |
| 3 | CA-3 | pendiente | | |
| 4 | CA-4 | fuera de alcance | | fuera de alcance |
| 5 | CA-5 | fuera de alcance | | fuera de alcance |
| 6 | CA-6 | fuera de alcance | | fuera de alcance |

## 3. Registro de decisiones y uso de IA

    - Apoyo de IA para legibilidad y redacción de hallazgos y plan de desarrollo.
    - Revision de vacios en el plan en base a los hallazgos, y documentacion de supuestos para poder avanzar con el desarrollo. Propuesta de endponints faltantes ej: Get/pacientes para recibir una lista y los detalles de cada contacto, descarte de login y roles de usuario por fuera de alcance, etc.
    - generacion textos de commit y mensajes para documentar el avance del desarrollo.
    -sugerencia revision de la secuencia de trabajo, aceptada para generar pruebas por cada CA y no al final del desarrollo, para poder tener trazabilidad de cada criterio de aceptación y su prueba unitaria asociada.
    - Decisión de la IA, aceptada: un paciente creado por este endpoint  `POST /api/pacientes` siempre nace en estado Activo, porque el teléfono ya es obligatorio ahí; "Incompleto" solo tendría sentido para un autorregistro que no se construye en esta entrega.
    -
