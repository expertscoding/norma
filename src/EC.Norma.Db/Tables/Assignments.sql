CREATE TABLE [norma].[Assignments]
(
	[Id] INT NOT NULL IDENTITY,
	[IdPermission] INT NOT NULL,
	[IdProfile] INT NOT NULL, 
	[StartDate] DATETIME2(7) NULL, 
	[EndDate] DATETIME2(7) NULL, 
    CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Assignments_Permissions] FOREIGN KEY (IdPermission) REFERENCES [norma].[Permissions]([Id]),
	CONSTRAINT [FK_Assignments_Profiles] FOREIGN KEY (IdProfile) REFERENCES [norma].[Profiles]([Id]),
    CONSTRAINT CHK_Assignments_Date CHECK ((StartDate is null AND EndDate is null) OR (StartDate is not null AND EndDate is not null))
)

GO

CREATE INDEX [IX_Assignments] ON [norma].[Assignments] ([IdPermission], [IdProfile])

