USE TbtbSalud;
GO

IF OBJECT_ID('dbo.Gestor', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Gestor (
        Id      UNIQUEIDENTIFIER  NOT NULL CONSTRAINT DF_Gestor_Id DEFAULT NEWID(),
        Nombre  NVARCHAR(200)      NOT NULL,
        CONSTRAINT PK_Gestor PRIMARY KEY (Id)
    );
END
GO
