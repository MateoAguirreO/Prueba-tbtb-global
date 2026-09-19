USE TbtbSalud;
GO

IF OBJECT_ID('dbo.ContactoHistorial', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContactoHistorial (
        Id                UNIQUEIDENTIFIER  NOT NULL CONSTRAINT DF_ContactoHistorial_Id DEFAULT NEWID(),
        ContactoId        UNIQUEIDENTIFIER  NOT NULL,
        GestorId          UNIQUEIDENTIFIER  NOT NULL,
        CampoModificado   NVARCHAR(50)       NOT NULL,
        ValorAnterior     NVARCHAR(200)      NOT NULL,
        ValorNuevo        NVARCHAR(200)      NOT NULL,
        Motivo            NVARCHAR(300)      NOT NULL,
        FechaCambio       DATETIME2          NOT NULL CONSTRAINT DF_ContactoHistorial_FechaCambio DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_ContactoHistorial PRIMARY KEY (Id),
        CONSTRAINT FK_ContactoHistorial_Contacto FOREIGN KEY (ContactoId) REFERENCES dbo.Contacto (Id),
        CONSTRAINT FK_ContactoHistorial_Gestor FOREIGN KEY (GestorId) REFERENCES dbo.Gestor (Id)
    );
END
GO

-- Append-only por diseño (01-hallazgos.md, hallazgo 5): esta tabla nunca se actualiza
-- ni se borra desde la aplicación, solo se inserta. Es la traza de cada corrección de CA-3.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_ContactoHistorial_ContactoId' AND object_id = OBJECT_ID('dbo.ContactoHistorial')
)
BEGIN
    CREATE INDEX IX_ContactoHistorial_ContactoId ON dbo.ContactoHistorial (ContactoId, FechaCambio DESC);
END
GO
