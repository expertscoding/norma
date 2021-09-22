MERGE INTO [norma].[ActionsPolicies] AS Target
USING ( VALUES 
	((Select Id from norma.Actions where Name='Listar'), (Select Id from norma.Policies where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Consultar'), (Select Id from norma.Policies where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Edit'), (Select Id from norma.Policies where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Delete'), (Select Id from norma.Policies where Name='HasPermission'))
)
AS Source ([IdAction], [IdPolicy])
ON Target.[IdAction] = Source.[IdAction] 
	and Target.[IdPolicy] = Source.[IdPolicy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([IdAction], [IdPolicy])
	VALUES ([IdAction], [IdPolicy]);