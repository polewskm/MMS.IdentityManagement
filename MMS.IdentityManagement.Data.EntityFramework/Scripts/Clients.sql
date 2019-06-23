SET NOCOUNT ON;
USE MakerIdMgmt;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
DROP TABLE [dbo].[Clients];
GO

CREATE TABLE [dbo].[Clients]
(
    [Id] INT NOT NULL,
    [ClientId] VARCHAR(255) NOT NULL,
    [Disabled] BIT NOT NULL,
    [RequireSecret] BIT NOT NULL,

    [CreatedWhen] DATETIMEOFFSET NOT NULL,
    [UpdatedWhen] DATETIMEOFFSET NOT NULL,

    CONSTRAINT PK_Clients PRIMARY KEY CLUSTERED ([Id])
);
GO
CREATE UNIQUE NONCLUSTERED INDEX U_Clients_ClientId ON dbo.Clients ([ClientId]);
GO
