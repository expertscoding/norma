MERGE INTO [norma].[Permissions] AS Target
USING ( VALUES 
	('ListarA', (Select Id from norma.Actions where Name='Listar'), (Select Id from norma.Resources where Name='A')),
	('ConsultarA', (Select Id from norma.Actions where Name='Consultar'), (Select Id from norma.Resources where Name='A')),
	('EditA', (Select Id from norma.Actions where Name='Edit'), (Select Id from norma.Resources where Name='A')),
	('DeleteA', (Select Id from norma.Actions where Name='Delete'), (Select Id from norma.Resources where Name='A')),
	('ListarB', (Select Id from norma.Actions where Name='Listar'), (Select Id from norma.Resources where Name='B')),
	('ConsultarB', (Select Id from norma.Actions where Name='Consultar'), (Select Id from norma.Resources where Name='B')),
	('EditarB', (Select Id from norma.Actions where Name='Edit'), (Select Id from norma.Resources where Name='B')),
	('BorrarB', (Select Id from norma.Actions where Name='Delete'), (Select Id from norma.Resources where Name='B')),
	('MenuA', (Select Id from norma.Actions where Name='Consultar'), (Select Id from norma.Resources where Name='MenuA')),
	('MenuB', (Select Id from norma.Actions where Name='Consultar'), (Select Id from norma.Resources where Name='MenuB'))

)
AS Source ([Name], [IdAction], [IdResource])
ON Target.[IdAction] = Source.[IdAction] 
	and Target.[IdResource] = Source.[IdResource]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name], [IdAction], [IdResource])
	VALUES ([Name], [IdAction], [IdResource]);