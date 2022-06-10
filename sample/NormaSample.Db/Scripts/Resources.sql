MERGE INTO [norma].[Resources] AS Target
USING ( VALUES 
	('A', (Select Id from norma.Modules where Name='DefaultModule 1')),
	('B', (Select Id from norma.Modules where Name='DefaultModule 1')),
	('A', (Select Id from norma.Modules where Name='DefaultModule 2')),
	('B', (Select Id from norma.Modules where Name='DefaultModule 2'))
)
AS Source (Name,[IdModule])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name],[IdModule])
	VALUES ([Name],[IdModule]);