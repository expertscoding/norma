MERGE INTO [norma].[Requirements] AS Target
USING ( VALUES 
	('HasPermission'),
	('IsAdmin'),
    ('HeadQuarters')
)
AS Source (Name)
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name])
	VALUES ([Name]);