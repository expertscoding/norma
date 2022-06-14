MERGE INTO [norma].[Permissions] AS Target
USING ( VALUES 
    -- For Applicaiton 1
	('ListarA', (Select Id from norma.Actions where Name='Listar'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 1')),
	('ConsultarA', (Select Id from norma.Actions where Name='Consultar'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 1')),
	('EditA', (Select Id from norma.Actions where Name='Edit'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 1')),
	('DeleteA', (Select Id from norma.Actions where Name='Delete'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 1')),
	('ProtectB', (Select Id from norma.Actions where Name='Protect'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='B' and m.Name = 'DefaultModule 1')),
	('ManageA', (Select Id from norma.Actions where Name='Manage'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 1')),
    -- For Applicaiton 2
	('ListarA|App2', (Select Id from norma.Actions where Name='Listar'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 2')),
	('ConsultarA|App2', (Select Id from norma.Actions where Name='Consultar'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 2')),
	('EditA|App2', (Select Id from norma.Actions where Name='Edit'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 2')),
	('DeleteA|App2', (Select Id from norma.Actions where Name='Delete'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 2')),
	('VerySpecialPermission', (Select Id from norma.Actions where Name='Protect'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='B' and m.Name = 'DefaultModule 2')),
	('ManageA|App2', (Select Id from norma.Actions where Name='Manage'), (Select r.Id from norma.Resources r inner join norma.Modules m on r.IdModule=m.id where r.Name='A' and m.Name = 'DefaultModule 2'))
)
AS Source ([Name], [IdAction], [IdResource])
ON Target.[IdAction] = Source.[IdAction] 
	and Target.[IdResource] = Source.[IdResource]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Name], [IdAction], [IdResource])
	VALUES ([Name], [IdAction], [IdResource]);