
MERGE INTO [norma].[Modules] AS Target
USING ( VALUES 
	('DefaultModule 1', (Select Id from norma.Applications where [Key] = 'APPKEY-WITHOUT-DEFAULT-REQUIREMENTS')),
	('DefaultModule 2', (Select Id from norma.Applications where [Key] = 'APPKEY-WITH-DEFAULT-REQUIREMENTS'))
)
AS Source ([Name], [IdApplication])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name], [IdApplication])
	VALUES ([Name], [IdApplication]);