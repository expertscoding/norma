MERGE INTO [norma].[Actions] AS Target
USING ( VALUES 
	('Listar', (Select Id from norma.Modules where Name='DefaultModule')),
	('Consultar', (Select Id from norma.Modules where Name='DefaultModule')),
	('Edit', (Select Id from norma.Modules where Name='DefaultModule')),
	('Delete', (Select Id from norma.Modules where Name='DefaultModule'))
)
AS Source ([Name],[IdModule])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name],[IdModule])
	VALUES ([Name],[IdModule]);