CREATE TABLE [norma].[ActionsPolicies]
(
	[Id] INT NOT NULL IDENTITY,
	[IdAction] INT NOT NULL,
	[IdPolicy] INT NOT NULL, 
    CONSTRAINT [PK_ActionsPolicies] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_ActionsPolicies_Actions] FOREIGN KEY (IdAction) REFERENCES [norma].[Actions]([Id]),
	CONSTRAINT [FK_ActionsPolicies_Policies] FOREIGN KEY (IdPolicy) REFERENCES [norma].[Policies]([Id]),
)

GO

CREATE INDEX [IX_ActionsPolicies] ON [norma].[ActionsPolicies] ([IdAction], [IdPolicy])

