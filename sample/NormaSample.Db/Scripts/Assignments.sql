MERGE INTO [norma].[Assignments] AS Target
USING ( VALUES 
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='ListarA')),
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='ConsultarA')),
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='EditA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='ListarA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='ConsultarA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='EditA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='DeleteA'))
)
AS Source ([IdProfile], [IdPermission])
ON Target.[IdProfile] = Source.[IdProfile] 
	and Target.[IdPermission] = Source.[IdPermission]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([IdProfile], [IdPermission])
	VALUES ([IdProfile], [IdPermission]);