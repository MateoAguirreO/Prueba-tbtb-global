USE TbtbSalud;
GO

-- Solo siembra si la base está vacía: evita duplicados si docker compose up
-- corre más de una vez sobre el mismo volumen. Todos los pacientes llevan
-- teléfono (Activo) porque es lo único que POST /api/pacientes puede producir
-- hoy — no hay pantalla de autorregistro ni de edición de paciente.
IF NOT EXISTS (SELECT 1 FROM dbo.Gestor)
BEGIN
    INSERT INTO dbo.Gestor (Nombre) VALUES
        ('Laura Restrepo'),
        ('Carlos Medina'),
        ('Ana Torres');

    INSERT INTO dbo.Paciente (Nombre, DocumentoIdentidad, Telefono, Correo, Ciudad, FechaInicioTratamiento, Estado) VALUES
        ('Maria Fernanda Gomez', '1032456789', '3001112233', 'mfgomez@mail.com', 'Bogota', '2026-06-01', 'Activo'),
        ('Jorge Andres Perez', '1045678912', '3002223344', 'japerez@mail.com', 'Medellin', '2026-06-05', 'Activo'),
        ('Lucia Ramirez', '1078912345', '3003334455', NULL, 'Cali', '2026-06-10', 'Activo'),
        ('Pedro Salazar', '45678912', '987654321', 'psalazar@mail.com', 'Lima', '2026-06-12', 'Activo'),
        ('Rosa Quispe', '78912345', '976543210', 'rquispe@mail.com', 'Lima', '2026-06-15', 'Activo'),
        ('Diego Vargas', '12345678', '956781234', NULL, 'Arequipa', '2026-06-18', 'Activo'),
        ('Camila Torres', '1712345678', '998877665', 'ctorres@mail.com', 'Quito', '2026-06-20', 'Activo'),
        ('Andres Molina', '1798765432', '987123456', NULL, 'Guayaquil', '2026-06-22', 'Activo');

    INSERT INTO dbo.Contacto (PacienteId, GestorId, Fecha, Canal, Resultado)
    SELECT p.Id, g.Id, v.Fecha, v.Canal, v.Resultado
    FROM (VALUES
        ('1032456789', 'Laura Restrepo',  '2026-06-08', 'Llamada',  'Contactado'),
        ('1032456789', 'Laura Restrepo',  '2026-07-08', 'WhatsApp', 'Contactado'),
        ('1045678912', 'Carlos Medina',   '2026-06-12', 'Llamada',  'NoContesta'),
        ('1045678912', 'Carlos Medina',   '2026-06-19', 'Llamada',  'NoContesta'),
        ('1045678912', 'Carlos Medina',   '2026-06-26', 'Llamada',  'Contactado'),
        ('1078912345', 'Ana Torres',      '2026-06-17', 'WhatsApp', 'Rechazada'),
        ('45678912',   'Ana Torres',      '2026-06-20', 'Correo',   'DatoInvalido'),
        ('12345678',   'Laura Restrepo',  '2026-06-25', 'Llamada',  'Contactado'),
        ('1712345678', 'Carlos Medina',   '2026-06-27', 'WhatsApp', 'Contactado')
    ) AS v(DocumentoIdentidad, GestorNombre, Fecha, Canal, Resultado)
    JOIN dbo.Paciente p ON p.DocumentoIdentidad = v.DocumentoIdentidad
    JOIN dbo.Gestor g ON g.Nombre = v.GestorNombre;
END
GO
