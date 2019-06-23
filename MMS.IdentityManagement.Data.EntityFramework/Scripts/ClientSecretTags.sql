SET NOCOUNT ON;
USE MakerIdMgmt;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClientSecretTags]') AND type in (N'U'))
DROP TABLE [dbo].[ClientSecretTags];
GO

CREATE TABLE [dbo].[ClientSecretTags]
(
	[Id] INT NOT NULL,
    [ClientSecretId] INT NOT NULL,
	[Key] VARCHAR(255) NOT NULL,
	[Value] NVARCHAR(MAX) NOT NULL,

    [CreatedWhen] DATETIMEOFFSET NOT NULL,
    [UpdatedWhen] DATETIMEOFFSET NOT NULL,

	CONSTRAINT PK_ClientSecretTags PRIMARY KEY NONCLUSTERED ([Id]),
	CONSTRAINT FK_ClientSecretTags_ClientSecrets_ClientSecretId FOREIGN KEY ([ClientSecretId]) REFERENCES dbo.ClientSecrets ([Id])
);
GO
CREATE UNIQUE INDEX U_ClientSecretTags_ClientSecretId_Key ON dbo.[ClientSecretTags] ([ClientSecretId], [Key]);
GO
