
-- !!!!TEST APPLICATION BUILDER!!!!
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'ui')
	exec sp_executesql N'create schema ui';
go
------------------------------------------------
begin
	set nocount on;
	grant execute on schema ::ui to public;
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'ui' and TABLE_NAME=N'Menu2')
create table ui.Menu2
(
	TenantId int not null,
	Id	uniqueidentifier not null,
	Parent uniqueidentifier null,
	[Name] nvarchar(255) null,
	[Url] nvarchar(255) null,
	Icon nvarchar(255) null,
	Model nvarchar(255) null,
	Help nvarchar(255) null,
	[Order] int not null constraint DF_Menu2_Order default(0),
	[Description] nvarchar(255) null,
	[ClassName] nvarchar(255) null,
	ModuleId uniqueidentifier,
	constraint PK_Menu2 primary key (TenantId, Id),
	constraint FK_Menu2_Parent_Menu foreign key (TenantId, Parent) references ui.Menu2(TenantId, Id)
);
go
-------------------------------------------------
drop procedure if exists ui.[MenuModule2.Merge];
drop type if exists ui.[MenuModule2.TableType];
go
-------------------------------------------------
create type ui.[MenuModule2.TableType] as table
(
	Id uniqueidentifier,
	Parent uniqueidentifier,
	[Name] nvarchar(255),
	[Url] nvarchar(255),
	Icon nvarchar(255),
	[Model] nvarchar(255),
	[Order] int,
	[Description] nvarchar(255),
	[Help] nvarchar(255),
	Module nvarchar(16),
	ClassName nvarchar(255)
);
go
------------------------------------------------
create procedure ui.[MenuModule2.Merge]
@TenantId int,
@Menu ui.[MenuModule2.TableType] readonly,
@ModuleId uniqueidentifier
as
begin
	with T as (
		select * from ui.Menu2 where TenantId = @TenantId
	)
	merge T as t
	using @Menu as s
	on t.Id = s.Id and t.TenantId = @TenantId
	when matched then
		update set
			t.Id = s.Id,
			t.Parent = s.Parent,
			t.[Name] = s.[Name],
			t.[Url] = s.[Url],
			t.[Icon] = s.Icon,
			t.[Order] = s.[Order],
			t.Model = s.Model,
			t.[Description] = s.[Description],
			t.Help = s.Help,
			t.ClassName = s.ClassName,
			t.ModuleId = @moduleId
	when not matched by target then
		insert(TenantId, Id, Parent, [Name], [Url], Icon, [Order], Model, [Description], Help, ClassName, ModuleId) values 
		(@TenantId, Id, Parent, [Name], [Url], Icon, [Order], Model, [Description], Help, ClassName, @moduleId)
	when not matched by source and t.TenantId = @TenantId then
		delete;
end
go
------------------------------------------------
create or alter procedure ui.[Menu2.OnCreateTenant]
@TenantId int
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @menu ui.[MenuModule2.TableType];
	declare @moduleId uniqueidentifier = N'{71EAC435-0487-423F-8004-D488F8703092}';

	insert into @menu(Id, Parent, [Order], [Name], [Url], Icon, ClassName) 
	values
		(N'62E32954-060A-4959-9C23-8BEB600BADEC',  null,  0, N'Main',         null,         null, null),
		(N'DB40EF2E-1E11-421B-B1D5-36FAF4ADEB37', N'62E32954-060A-4959-9C23-8BEB600BADEC',  400, N'@[Sales]',         N'sales',       N'shopping', N'border-top'),
		(N'06997972-6AEA-4114-9E1E-A1537A70CD4F', N'62E32954-060A-4959-9C23-8BEB600BADEC',  500, N'@[Purchases]',     N'purchase',    N'cart', null),
		(N'537CEFA3-61B2-4844-B091-18CBF1CF8DC1', N'62E32954-060A-4959-9C23-8BEB600BADEC',  600, null, null,  null,   N'grow'),
		(N'7DE5512E-3DF8-4822-AB34-F06744D1A305', N'62E32954-060A-4959-9C23-8BEB600BADEC',  700, N'@[Settings]',      N'settings',    N'gear-outline', null),
		-- Sales
		(N'43897DEF-59FF-41C1-B93A-A6D183E1ED7A', N'DB40EF2E-1E11-421B-B1D5-36FAF4ADEB37', 11, N'@[Documents]',      null,  null, null),
		(N'36FE4813-1F57-474A-B97E-F716EDC1CDA1', N'43897DEF-59FF-41C1-B93A-A6D183E1ED7A', 2,  N'@[Orders]',         N'/document/document/index/0',     N'file', null),
		(N'5E6435F8-8D9C-48A4-A884-F87E565515D3', N'DB40EF2E-1E11-421B-B1D5-36FAF4ADEB37', 12, N'@[Catalogs]',      null,  null, null),
		(N'878CA15E-34D1-4FFE-910E-42215CBCE0A9', N'5E6435F8-8D9C-48A4-A884-F87E565515D3', 30, N'@[Agents]',         N'/catalog/agent/index/0',     N'file', null),
		(N'9AEA0E4A-3AFD-4476-9A99-A56B55A0C8CA', N'5E6435F8-8D9C-48A4-A884-F87E565515D3', 31, N'@[Products]',       N'/catalog/product/index/0',  N'file', null),
		(N'DA78C1A3-8FBD-4648-8C5F-CF99497FC158', N'5E6435F8-8D9C-48A4-A884-F87E565515D3', 32, N'@[Projects]',       N'/catalog/unit/index/0',   N'file', null),
		-- Purchase
		(N'85A094C7-FABE-4290-AEE8-8206BBB4684B', N'06997972-6AEA-4114-9E1E-A1537A70CD4F', 11, N'@[Documents]', null,  null, null),
		(N'7AA989B7-2B54-443D-8726-5E9400E2CCC9', N'06997972-6AEA-4114-9E1E-A1537A70CD4F', 14, N'@[Catalogs]',  null, null, null),
		(N'CE1107DD-9B72-41F1-B341-C8E2885EDBDA', N'7AA989B7-2B54-443D-8726-5E9400E2CCC9', 10, N'@[Agents]',    N'/catalog/agent/index/0',     N'users', null),
		(N'EE613815-AAD1-42C9-86D4-34A6E1AB54E2', N'7AA989B7-2B54-443D-8726-5E9400E2CCC9', 11, N'@[Units]',		N'/catalog/unit/index/0',  N'user-image', null),
		(N'E719BC4B-24F5-45DD-BD2A-47D7713D4087', N'7AA989B7-2B54-443D-8726-5E9400E2CCC9', 12, N'@[Items]',     N'/catalog/product/index/0',      N'package-outline', N'line-top');

	exec ui.[MenuModule2.Merge] @TenantId, @menu, @moduleId;
end
go

exec ui.[Menu2.OnCreateTenant] 1
go
------------------------------------------------
create or alter procedure ui.[Menu.Simple.User.Load]
@TenantId int = 1,
@UserId bigint = null,
@Mobile bit = 0,
@Root bigint = 1
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @RootId uniqueidentifier = N'62E32954-060A-4959-9C23-8BEB600BADEC';
	with RT as (
		select Id=m0.Id, ParentId = m0.Parent, [Level] = 0
			from ui.Menu2 m0
			where m0.TenantId = @TenantId and m0.Id = @RootId
		union all
		select m1.Id, m1.Parent, RT.[Level]+1
			from RT inner join ui.Menu2 m1 on m1.Parent = RT.Id and m1.TenantId = @TenantId
	)
	select [Menu!TMenu!Tree] = null, [Id!!Id]=RT.Id, [!TMenu.Menu!ParentId]=RT.ParentId,
		[Menu!TMenu!Array] = null,
		m.Name, m.Url, m.Icon, m.[Description], m.Help, m.ClassName
	from RT 
		inner join ui.Menu2 m on m.TenantId = @TenantId and RT.Id=m.Id
	order by RT.[Level], m.[Order], RT.[Id];

	-- system parameters
	select [SysParams!TParam!Object]= null, [AppTitle] = N'BuildApp', [AppSubTitle] = N'SubTitle', 
		[SideBarMode] = null, [NavBarMode] = null, [Pages] = null;
end
go

exec ui.[Menu.Simple.User.Load];
