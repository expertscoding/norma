CREATE TABLE [norma].[PermissionsPolicies]
(
	[Id] INT NOT NULL IDENTITY,
	[IdPermission] INT NOT NULL,
	[IdPolicy] INT NOT NULL, 
    CONSTRAINT [PK_PermissionsPolicies] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_PermissionsPolicies_Permissions] FOREIGN KEY (IdPermission) REFERENCES [norma].[Permissions]([Id]),
	CONSTRAINT [FK_PermissionsPolicies_Policies] FOREIGN KEY (IdPolicy) REFERENCES [norma].[Policies]([Id]),
)

GO

CREATE INDEX [IX_PermissionsPolicies] ON [norma].[PermissionsPolicies] ([IdPermission], [IdPolicy])
