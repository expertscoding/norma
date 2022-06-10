MERGE INTO [norma].[Applications] AS Target
USING ( VALUES 
	('DefaultApplication without default requirements', 'APPKEY-WITHOUT-DEFAULT-REQUIREMENTS'),
	('DefaultApplication with default requirements', 'APPKEY-WITH-DEFAULT-REQUIREMENTS')
)
AS Source ([Name], [Key])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name],[Key])
	VALUES ([Name],[Key]);