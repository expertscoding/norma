MERGE INTO [norma].[Resources] AS Target
USING ( VALUES 
	('A'),
	('B'),
	('VentanaAdmin'),
	('MenuA'),
	('MenuB'),
	('MenuAdmin')
)
AS Source (Name)
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name])
	VALUES ([Name]);