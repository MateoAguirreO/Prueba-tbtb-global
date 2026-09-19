# Bitacora de desarrollo Prueba TBTB

Uso claude code con Sonnet5 como modelo

## 1. Documentos base

- [01-hallazgos.md](01-hallazgos.md) — Hallazgos sobre el PRD
- [02-plan.md](02-plan.md) — Plan de desarrollo y modelo de datos
- [03-bitacora.md](03-bitacora.md) — Bitácora de desarrollo

## 2. Matriz de trazabilidad

| #   | criterio de aceptación | commit o archivos relacionados                                                                      | Prueba de verificacion                                                                                                                                                                                                                                                                                                                    | estado (cubierto, parcial, fuera de alcance) |
| --- | ---------------------- | --------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| 1   | CA-1                   | https://github.com/MateoAguirreO/Prueba-tbtb-global/commit/f175a0df4a2c8c57d2823bd1ac080736a50a6b77 | Api.Tests/PacienteServiceTests.cs: CA1_CrearPaciente_ConDatosValidos_QuedaActivoYDisponibleParaAgendarContactos, CA1_CrearPaciente_ConDocumentoDuplicado_LanzaDocumentoDuplicadoException                                                                                                                                                 | Cubierto                                     |
| 2   | CA-2                   | https://github.com/MateoAguirreO/Prueba-tbtb-global/commit/b9ef8094abd510ad1e608df9898dce35528959b7 | Api.Tests/ContactoServiceTests.cs: CA2_RegistrarContacto_ConDatosValidos_QuedaAsociadoAlPaciente, CA2_RegistrarContacto_ConPacienteInexistente_LanzaPacienteNoEncontradoException, CA2_RegistrarContacto_ConGestorInexistente_LanzaGestorNoEncontradoException                                                                            | Cubierto                                     |
| 3   | CA-3                   | https://github.com/MateoAguirreO/Prueba-tbtb-global/commit/71af7166032c019fa47924a0928cc24c01423cf8 | Api.Tests/CorregirContactoTests.cs: CA3_CorregirContacto_ConResultadoDistinto_RegistraHistorialYActualizaContacto, CA3_CorregirContacto_ConContactoInexistente_LanzaContactoNoEncontradoException, CA3_CorregirContacto_ConGestorInexistente_LanzaGestorNoEncontradoException, CA3_CorregirContactoDto_SinCamposACorregir_FallaValidacion | Cubierto                                     |
| 4   | CA-4                   | fuera de alcance                                                                                    |                                                                                                                                                                                                                                                                                                                                           | fuera de alcance                             |
| 5   | CA-5                   | fuera de alcance                                                                                    |                                                                                                                                                                                                                                                                                                                                           | fuera de alcance                             |
| 6   | CA-6                   | fuera de alcance                                                                                    |                                                                                                                                                                                                                                                                                                                                           | fuera de alcance                             |

## 3. Registro de decisiones y uso de IA

    - Apoyo de IA para legibilidad y redacción de hallazgos y plan de desarrollo.
    - Revision de vacios en el plan en base a los hallazgos, y documentacion de supuestos para poder avanzar con el desarrollo. Propuesta de endponints faltantes ej: Get/pacientes para recibir una lista y los detalles de cada contacto, descarte de login y roles de usuario por fuera de alcance, etc.
    - generacion textos de commit y mensajes para documentar el avance del desarrollo.
    -sugerencia revision de la secuencia de trabajo, aceptada para generar pruebas por cada CA y no al final del desarrollo, para poder tener trazabilidad de cada criterio de aceptación y su prueba unitaria asociada.
    - Decisión de la IA, aceptada: un paciente creado por este endpoint  `POST /api/pacientes` siempre nace en estado Activo, porque el teléfono ya es obligatorio ahí; "Incompleto" solo tendría sentido para un autorregistro que no se construye en esta entrega.
    - Uso de IA para generar pruebas unitarias xUnit para cada endpoint
    - Pruebas de interfaz con playwright, generadas con ayuda de IA, para validar la experiencia de usuario y la integración de los endpoints con la interfaz web.
    - Uso de IA para generar la interfaz web en Angular, con formularios reactivos y validaciones, y la integración con los endpoints del backend.
