MERGE INTO [norma].[RequirementsApplications] AS Target
USING ( VALUES 
    ((Select Id from [norma].[Requirements] where Name='IsAdmin'), (Select Id from [norma].[Applications] where [Key] = 'APPKEY-WITH-DEFAULT-REQUIREMENTS'))
)
AS Source ([IdRequirement], [IdApplication])
ON Target.[IdRequirement] = Source.[IdRequirement] and Target.[IdApplication] = Source.[IdApplication]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([IdRequirement], [IdApplication])
	VALUES ([IdRequirement], [IdApplication]);