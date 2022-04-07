
MERGE INTO [norma].[Applications] AS Target
USING ( VALUES 
	('DefaultApplication', '1234567890')
)
AS Source ([Name], [ApplicatonId])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name],[ApplicatonId])
	VALUES ([Name],[ApplicatonId]);