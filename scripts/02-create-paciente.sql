USE TbtbSalud;
GO

IF OBJECT_ID('dbo.Paciente', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Paciente (
        Id                      UNIQUEIDENTIFIER   NOT NULL CONSTRAINT DF_Paciente_Id DEFAULT NEWID(),
        Nombre                  NVARCHAR(200)       NOT NULL,
        DocumentoIdentidad      NVARCHAR(30)        NOT NULL,
        Telefono                NVARCHAR(20)        NULL,
        Correo                  NVARCHAR(200)       NULL,
        Ciudad                  NVARCHAR(100)       NOT NULL,
        FechaInicioTratamiento  DATE                NOT NULL,
        Estado                  NVARCHAR(20)        NOT NULL CONSTRAINT DF_Paciente_Estado DEFAULT 'Incompleto',
        FechaCreacion           DATETIME2           NOT NULL CONSTRAINT DF_Paciente_FechaCreacion DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Paciente PRIMARY KEY (Id),
        CONSTRAINT UQ_Paciente_DocumentoIdentidad UNIQUE (DocumentoIdentidad),
        CONSTRAINT CK_Paciente_Estado CHECK (Estado IN ('Incompleto', 'Activo'))
    );
END
GO
