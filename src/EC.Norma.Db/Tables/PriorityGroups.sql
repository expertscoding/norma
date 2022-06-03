CREATE TABLE [norma].[PriorityGroups]
(
	[Id] INT NOT NULL IDENTITY, 
	[Name] VARCHAR(20) NOT NULL,
	[Priority] INT NOT NULL,

    CONSTRAINT [PK_PriorityGroups] PRIMARY KEY ([Id]) 
)
