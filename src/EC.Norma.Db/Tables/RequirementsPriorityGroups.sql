CREATE TABLE [norma].[RequirementsPriorityGroups]
(
	[Id] INT NOT NULL IDENTITY,
	[IdRequirement] INT NOT NULL, 
	[IdPriorityGroup] INT NOT NULL, 
    CONSTRAINT [PK_RequirementsPriorityGroups] PRIMARY KEY ([Id]), 
	CONSTRAINT [FK_RequirementsPriorityGroups_Requirements] FOREIGN KEY (IdRequirement) REFERENCES [norma].[Requirements]([Id]),
    CONSTRAINT [FK_RequirementsPriorityGroups_Permissions] FOREIGN KEY (IdPriorityGroup) REFERENCES [norma].[PriorityGroups]([Id]),
)

GO

CREATE INDEX [IX_RequirementsPriorityGroups] ON [norma].[RequirementsPriorityGroups] ( [IdRequirement], [IdPriorityGroup])
