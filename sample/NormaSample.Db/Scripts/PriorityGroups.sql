MERGE INTO [norma].[PriorityGroups] AS Target
USING ( VALUES 
	('Priority Group 1', 1),
    ('Priority Group 2', 2)
)
AS Source ([Name], [Priority])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name], [Priority])
	VALUES ([Name], [Priority]);