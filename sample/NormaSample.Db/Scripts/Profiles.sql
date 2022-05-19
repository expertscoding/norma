MERGE INTO [norma].[Profiles] AS Target
USING ( VALUES 
	('Administrator'),
	('Manager'),
	('SuperUser'),
	('User')
)
AS Source (Name)
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name])
	VALUES ([Name]);
