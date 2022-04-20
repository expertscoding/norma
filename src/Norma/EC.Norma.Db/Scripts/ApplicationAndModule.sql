SET IDENTITY_INSERT [norma].[Applications] ON

MERGE INTO [norma].[Applications] AS Target
USING ( VALUES 
	(-1, 'Default App', '05FAF113-E8EA-44F1-BDB5-2455B2F18D24')
)
AS Source ([Id], [Name], [Key])
ON Target.[Id] = Source.[Id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Id], [Name], [Key])
	VALUES ([Id], [Name], [Key]);

SET IDENTITY_INSERT [norma].[Applications] OFF

SET IDENTITY_INSERT [norma].[Modules] ON

MERGE INTO [norma].[Modules] AS Target
USING ( VALUES 
	(-1, 'Default Module', -1)
)
AS Source ([Id], [Name], [IdApplication])
ON Target.[Id] = Source.[Id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Id], [Name], [IdApplication])
	VALUES ([Id], [Name], [IdApplication]);

SET IDENTITY_INSERT [norma].[Modules] OFF


GO
PRINT N'Dropping Foreign Key [norma].[FK_Actions_Modules]...';

GO
ALTER TABLE [norma].[Actions] DROP CONSTRAINT [FK_Actions_Modules];

GO
PRINT N'Altering Table [norma].[Actions]... [IdModule] to NOT NULL';

UPDATE [norma].[Actions] SET [IdModule] = -1 WHERE [IdModule] IS NULL

GO
ALTER TABLE [norma].[Actions] ALTER COLUMN [IdModule] INT NOT NULL;


GO
PRINT N'Creating Foreign Key [norma].[FK_Actions_Modules]...';

GO
ALTER TABLE [norma].[Actions] WITH NOCHECK
    ADD CONSTRAINT [FK_Actions_Modules] FOREIGN KEY ([IdModule]) REFERENCES [norma].[Modules] ([Id]);


GO
PRINT N'Dropping Foreign Key [norma].[FK_Resources_Modules]...';

GO
ALTER TABLE [norma].[Resources] DROP CONSTRAINT [FK_Resources_Modules];

GO
PRINT N'Altering Table [norma].[Resources]... [IdModule] to NOT NULL';

UPDATE [norma].[Resources] SET [IdModule] = -1 WHERE [IdModule] IS NULL

GO
ALTER TABLE [norma].[Resources] ALTER COLUMN [IdModule] INT NOT NULL;

GO
PRINT N'Creating Foreign Key [norma].[FK_Resources_Modules]...';


GO
ALTER TABLE [norma].[Resources] WITH NOCHECK
    ADD CONSTRAINT [FK_Resources_Modules] FOREIGN KEY ([IdModule]) REFERENCES [norma].[Modules] ([Id]);

GO
ALTER TABLE [norma].[Actions] WITH CHECK CHECK CONSTRAINT [FK_Actions_Modules];

ALTER TABLE [norma].[Resources] WITH CHECK CHECK CONSTRAINT [FK_Resources_Modules];

GO
