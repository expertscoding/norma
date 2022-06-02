CREATE TABLE [norma].[Applications]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(1024) NOT NULL,
	[Key] NVARCHAR(1024) NOT NULL,
	CONSTRAINT [PK_Applications] PRIMARY KEY ([Id])
)
