MERGE INTO [norma].[ActionsRequirements] AS Target
USING ( VALUES 
	((Select Id from norma.Actions where Name='Listar'), (Select Id from norma.Requirements where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Consultar'), (Select Id from norma.Requirements where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Edit'), (Select Id from norma.Requirements where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Delete'), (Select Id from norma.Requirements where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Protect'), (Select Id from norma.Requirements where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Manage'), (Select Id from norma.Requirements where Name='IsAdmin')),
	((Select Id from norma.Actions where Name='Manage'), (Select Id from norma.Requirements where Name='HasPermission')),
	((Select Id from norma.Actions where Name='Manage'), (Select Id from norma.Requirements where Name='HeadQuarters'))
)
AS Source ([IdAction], [IdRequirement])
ON Target.[IdAction] = Source.[IdAction] 
	and Target.[IdRequirement] = Source.[IdRequirement]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([IdAction], [IdRequirement])
	VALUES ([IdAction], [IdRequirement]);