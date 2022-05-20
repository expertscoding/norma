CREATE TABLE [norma].[RequirementsApplications]
(
	[Id] INT NOT NULL IDENTITY,
	[IdRequirement] INT NOT NULL, 
	[IdApplication] INT NOT NULL, 
    [IsDefault] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_RequirementsApplications] PRIMARY KEY ([Id]), 
	CONSTRAINT [FK_RequirementsApplications_Requirements] FOREIGN KEY (IdRequirement) REFERENCES [norma].[Requirements]([Id]),
    CONSTRAINT [FK_RequirementsApplications_Permissions] FOREIGN KEY (IdApplication) REFERENCES [norma].[Applications]([Id]),
)

GO

CREATE INDEX [IX_RequirementsApplications] ON [norma].[RequirementsApplications] ( [IdRequirement], [IdApplication])
