MERGE INTO [norma].[Resources] AS Target
USING ( VALUES 
	('A', (Select Id from norma.Modules where Name='DefaultModule')),
	('B', (Select Id from norma.Modules where Name='DefaultModule')),
	('VentanaAdmin', (Select Id from norma.Modules where Name='DefaultModule')),
	('MenuA', (Select Id from norma.Modules where Name='DefaultModule')),
	('MenuB', (Select Id from norma.Modules where Name='DefaultModule')),
	('MenuAdmin', (Select Id from norma.Modules where Name='DefaultModule'))
)
AS Source (Name,[IdModule])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name],[IdModule])
	VALUES ([Name],[IdModule]);