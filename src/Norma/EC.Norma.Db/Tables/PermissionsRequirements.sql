CREATE TABLE [norma].[PermissionsRequirements]
(
	[Id] INT NOT NULL IDENTITY,
	[IdPermission] INT NOT NULL,
	[IdRequirement] INT NOT NULL, 
    CONSTRAINT [PK_PermissionsRequirements] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_PermissionsRequirements_Permissions] FOREIGN KEY (IdPermission) REFERENCES [norma].[Permissions]([Id]),
	CONSTRAINT [FK_PermissionsRequirements_Requirements] FOREIGN KEY (IdRequirement) REFERENCES [norma].[Requirements]([Id]),
)

GO

CREATE INDEX [IX_PermissionsRequirements] ON [norma].[PermissionsRequirements] ([IdPermission], [IdRequirement])
