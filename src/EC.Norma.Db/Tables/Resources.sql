CREATE TABLE [norma].[Resources]
(
	[Id] INT NOT NULL IDENTITY, 
	[Name] NVARCHAR(1024) NOT NULL,
	[IdModule] INT NULL,
    CONSTRAINT [PK_Resources] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Resources_Modules] FOREIGN KEY (IdModule) REFERENCES [norma].[Modules]([Id]),
)
