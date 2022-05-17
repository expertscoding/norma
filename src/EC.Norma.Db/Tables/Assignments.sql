CREATE TABLE [norma].[Assignments]
(
	[Id] INT NOT NULL IDENTITY,
	[IdPermission] INT NOT NULL,
	[IdProfile] INT NOT NULL, 
    CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Assignments_Permissions] FOREIGN KEY (IdPermission) REFERENCES [norma].[Permissions]([Id]),
	CONSTRAINT [FK_Assignments_Profiles] FOREIGN KEY (IdProfile) REFERENCES [norma].[Profiles]([Id]),
)

GO

CREATE INDEX [IX_Assignments] ON [norma].[Assignments] ([IdPermission], [IdProfile])

