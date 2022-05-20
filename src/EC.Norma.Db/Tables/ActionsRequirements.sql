CREATE TABLE [norma].[ActionsRequirements]
(
	[Id] INT NOT NULL IDENTITY,
	[IdAction] INT NOT NULL,
	[IdRequirement] INT NOT NULL, 
    CONSTRAINT [PK_ActionsRequirements] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_ActionsRequirements_Actions] FOREIGN KEY (IdAction) REFERENCES [norma].[Actions]([Id]),
	CONSTRAINT [FK_ActionsRequirements_Requirements] FOREIGN KEY (IdRequirement) REFERENCES [norma].[Requirements]([Id]),
)

GO

CREATE INDEX [IX_ActionsRequirements] ON [norma].[ActionsRequirements] ([IdAction], [IdRequirement])