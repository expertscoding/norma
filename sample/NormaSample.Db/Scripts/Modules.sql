
MERGE INTO [norma].[Modules] AS Target
USING ( VALUES 
	('DefaultModule', (Select Id from norma.Applications where Name='DefaultApplication'))
)
AS Source ([Name], [IdApplication])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name], [IdApplication])
	VALUES ([Name], [IdApplication]);