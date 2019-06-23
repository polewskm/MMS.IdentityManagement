SET NOCOUNT ON;
USE MakerIdMgmt;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClientSecrets]') AND type in (N'U'))
DROP TABLE [dbo].[ClientSecrets];
GO

CREATE TABLE [dbo].[ClientSecrets]
(
    [Id] INT NOT NULL,
    [ClientId] INT NOT NULL,
    [SecretId] VARCHAR(255) NOT NULL,
    [CipherType] VARCHAR(255) NOT NULL,
    [CipherText] VARCHAR(MAX) NOT NULL,

    [CreatedWhen] DATETIMEOFFSET NOT NULL,
    [UpdatedWhen] DATETIMEOFFSET NOT NULL,

    CONSTRAINT PK_ClientSecrets PRIMARY KEY NONCLUSTERED ([Id]),
    CONSTRAINT FK_ClientSecrets_Clients_ClientId FOREIGN KEY ([ClientId]) REFERENCES dbo.Clients ([Id])
);
GO
CREATE UNIQUE CLUSTERED INDEX U_ClientSecrets_ClientId_SecretId ON dbo.ClientSecrets ([ClientId], [SecretId]);
GO
