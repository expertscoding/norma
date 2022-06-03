CREATE TABLE [norma].[Permissions]
(
	[Id] INT NOT NULL IDENTITY,
	[Name] VARCHAR(20) NOT NULL,
	[IdAction] INT NOT NULL,
	[IdResource] INT NOT NULL, 
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id]),

	CONSTRAINT [FK_Permissions_Actions] FOREIGN KEY (IdAction) REFERENCES [norma].[Actions]([Id]),
	CONSTRAINT [FK_Permissions_Resources] FOREIGN KEY (IdResource) REFERENCES [norma].[Resources]([Id]),
)

GO

CREATE UNIQUE INDEX [IX_Permissions_Functional] ON [norma].[Permissions] ([IdAction], [IdResource])
GO
