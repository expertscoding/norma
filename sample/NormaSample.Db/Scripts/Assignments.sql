MERGE INTO [norma].[Assignments] AS Target
USING ( VALUES 
    -- For Applicaiton 1
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='ListarA')),
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='ConsultarA')),
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='EditA')),
	((Select Id from norma.Profiles where Name='Manager'), (Select Id from norma.[Permissions] where Name='ManageA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='ListarA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='ConsultarA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='EditA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='DeleteA')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='ProtectB')),
	((Select Id from norma.Profiles where Name='Administrator'), (Select Id from norma.[Permissions] where Name='ManageA')),

    -- For Applicaiton 2
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='ListarA|App2')),
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='ConsultarA|App2')),
	((Select Id from norma.Profiles where Name='User'), (Select Id from norma.[Permissions] where Name='EditA|App2')),
	((Select Id from norma.Profiles where Name='Manager'), (Select Id from norma.[Permissions] where Name='ManageA|App2')),
	((Select Id from norma.Profiles where Name='Manager'), (Select Id from norma.[Permissions] where Name='VerySpecialPermission'))
)
AS Source ([IdProfile], [IdPermission])
ON Target.[IdProfile] = Source.[IdProfile] 
	and Target.[IdPermission] = Source.[IdPermission]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([IdProfile], [IdPermission])
	VALUES ([IdProfile], [IdPermission]);