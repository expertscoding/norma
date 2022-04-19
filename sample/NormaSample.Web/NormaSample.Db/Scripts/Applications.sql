MERGE INTO [norma].[Applications] AS Target
USING ( VALUES 
	('DefaultApplication', '1234567890')
)
AS Source ([Name], [ApplicationKey])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name],[ApplicationKey])
	VALUES ([Name],[ApplicationKey]);