CREATE TABLE [norma].[Actions]
(
	[Id] INT NOT NULL IDENTITY ,
	[Name] VARCHAR(20) NOT NULL, 
	[IdModule] int not null,
    CONSTRAINT [PK_Actions] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Actions_Modules] FOREIGN KEY (IdModule) REFERENCES [norma].[Modules]([Id]),
)
