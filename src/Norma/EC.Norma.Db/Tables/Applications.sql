CREATE TABLE [norma].[Applications]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Key] [nvarchar](250) NOT NULL,
	CONSTRAINT [PK_Applications] PRIMARY KEY ([Id])
)
