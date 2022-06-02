MERGE INTO [norma].[RequirementsPriorityGroups] AS Target
USING ( VALUES 
	((Select Id from norma.Requirements where Name='HasPermission'), (Select Id from norma.PriorityGroups where Name='Priority Group 1')),
    ((Select Id from norma.Requirements where Name='IsAdmin'), (Select Id from norma.PriorityGroups where Name='Priority Group 1'))
)
AS Source ([IdRequirement], [IdPriorityGroup])
ON Target.[IdRequirement] = Source.[IdRequirement]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([IdRequirement], [IdPriorityGroup])
	VALUES ([IdRequirement], [IdPriorityGroup]);