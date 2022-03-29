CREATE TABLE [norma].[Resources]
(
	[Id] INT NOT NULL IDENTITY, 
	[Name] VARCHAR(20) NOT NULL,
	[IdModule] int not null,
    CONSTRAINT [PK_Resources] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Resources_Modules] FOREIGN KEY (IdModule) REFERENCES [norma].[Modules]([Id]),
)
