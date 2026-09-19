USE TbtbSalud;
GO

IF OBJECT_ID('dbo.Contacto', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Contacto (
        Id             UNIQUEIDENTIFIER  NOT NULL CONSTRAINT DF_Contacto_Id DEFAULT NEWID(),
        PacienteId     UNIQUEIDENTIFIER  NOT NULL,
        GestorId       UNIQUEIDENTIFIER  NOT NULL,
        Fecha          DATE               NOT NULL,
        Canal          NVARCHAR(20)       NOT NULL,
        Resultado      NVARCHAR(20)       NOT NULL,
        FechaRegistro  DATETIME2          NOT NULL CONSTRAINT DF_Contacto_FechaRegistro DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Contacto PRIMARY KEY (Id),
        CONSTRAINT FK_Contacto_Paciente FOREIGN KEY (PacienteId) REFERENCES dbo.Paciente (Id),
        CONSTRAINT FK_Contacto_Gestor FOREIGN KEY (GestorId) REFERENCES dbo.Gestor (Id),
        CONSTRAINT CK_Contacto_Canal CHECK (Canal IN ('Llamada', 'WhatsApp', 'Correo')),
        CONSTRAINT CK_Contacto_Resultado CHECK (Resultado IN ('Contactado', 'NoContesta', 'Rechazada', 'DatoInvalido'))
    );
END
GO

-- Consulta con criterio (02-plan.md, sección 4): listar los contactos de un paciente
-- ordenados por fecha es el filtro más frecuente de la pantalla de detalle, por eso
-- el índice va sobre (PacienteId, Fecha) y no solo sobre PacienteId.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_Contacto_PacienteId_Fecha' AND object_id = OBJECT_ID('dbo.Contacto')
)
BEGIN
    CREATE INDEX IX_Contacto_PacienteId_Fecha ON dbo.Contacto (PacienteId, Fecha DESC);
END
GO
