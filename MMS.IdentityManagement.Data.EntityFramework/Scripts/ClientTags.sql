SET NOCOUNT ON;
USE MakerIdMgmt;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClientTags]') AND type in (N'U'))
DROP TABLE [dbo].[ClientTags];
GO

CREATE TABLE [dbo].[ClientTags]
(
	[Id] INT NOT NULL,
	[ClientId] INT NOT NULL,
	[Key] VARCHAR(255) NOT NULL,
	--[KeyHash] AS ISNULL(CONVERT(BINARY(32), HASHBYTES('SHA2_256', [Key])), 0x),
	[Value] NVARCHAR(MAX) NOT NULL,

    [CreatedWhen] DATETIMEOFFSET NOT NULL,
    [UpdatedWhen] DATETIMEOFFSET NOT NULL,

	CONSTRAINT PK_ClientTags PRIMARY KEY NONCLUSTERED ([Id]),
	CONSTRAINT FK_ClientTags_Clients_ClientId FOREIGN KEY ([ClientId]) REFERENCES dbo.Clients ([Id])
);
GO
CREATE UNIQUE CLUSTERED INDEX U_ClientTags_ClientId_Key ON dbo.ClientTags ([ClientId], [Key]);
GO
