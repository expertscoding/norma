CREATE TABLE [norma].[PoliciesPriorityGroups]
(
	[Id] INT NOT NULL IDENTITY,
	[IdPolicy] INT NOT NULL, 
	[IdPriorityGroup] INT NOT NULL, 
    CONSTRAINT [PK_PoliciesPriorityGroups] PRIMARY KEY ([Id]), 
	CONSTRAINT [FK_PoliciesPriorityGroups_Policies] FOREIGN KEY (IdPolicy) REFERENCES [norma].[Policies]([Id]),
    CONSTRAINT [FK_PoliciesPriorityGroups_Permissions] FOREIGN KEY (IdPriorityGroup) REFERENCES [norma].[PriorityGroups]([Id]),
)

GO

CREATE INDEX [IX_PoliciesPriorityGroups] ON [norma].[PermissionsPolicies] ([IdPermission], [IdPolicy])
