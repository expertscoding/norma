CREATE TABLE [norma].[Modules]
(
	[Id] INT NOT NULL IDENTITY ,
	[Name] NVARCHAR(1024) NOT NULL, 
	[IdApplication] INT NOT NULL 
    CONSTRAINT [PK_Modules] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Modules_Applications] FOREIGN KEY (IdApplication) REFERENCES [norma].[Applications]([Id])

)
