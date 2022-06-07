CREATE TABLE [norma].[PriorityGroups]
(
	[Id] INT NOT NULL IDENTITY, 
	[Name] NVARCHAR(1024) NOT NULL,
	[Priority] INT NOT NULL,

    CONSTRAINT [PK_PriorityGroups] PRIMARY KEY ([Id]) 
)
