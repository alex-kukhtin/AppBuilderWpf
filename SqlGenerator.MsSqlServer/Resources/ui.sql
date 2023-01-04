-- app title
------------------------------------------------
begin
if not exists(select * from a2sys.SysParams where [Name] = N'AppTitle')
	insert into a2sys.SysParams ([Name], StringValue) values (N'AppTitle', N'$(AppTitle)');
else
	update a2sys.SysParams set StringValue = N'$(AppTitle)' where [Name] = N'AppTitle';
end
go
------------------------------------------------
if not exists (select * from a2security.Acl where [Object] = 'std:menu' and [ObjectId] = 1 and GroupId = 1)
	insert into a2security.Acl ([Object], ObjectId, GroupId, CanView) values (N'std:menu', 1, 1, 1);
go
------------------------------------------------
-- menu
begin
	set nocount on;
	declare @menu a2ui.[Menu2.TableType];
	insert into @menu(Id, Parent, [Name], [Url], [Order]) values
		(1,		null,	N'Root', null, 0),
		(10,	1,	N'@[Catalogs]' ,  N'catalog', 10),
		(20,	1,	N'@[Documents]' , N'document', 10),
$(MenuItems)
		

	exec a2ui.[Menu.Merge] @menu, 1, 1000;
	exec a2security.[Permission.UpdateAcl.Menu];
end
go