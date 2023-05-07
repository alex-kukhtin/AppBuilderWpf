------------------------------------------------
create or alter procedure cat.[Product.Index]
@UserId bigint,
@Id bigint = null,
@Offset int = 0,
@PageSize int = 20,
@Order nvarchar(255) = N'date', -- TODO: initial sort field
@Dir nvarchar(20) = N'asc' -- TODO: initial sort direction
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Products!TProduct!Array] = null,
		[Id!!Id] = p.Id, [Name!!Name] = p.[Name], p.SKU, [Unit!TUnit!RefId] = p.Unit, p.Memo
	from cat.[Products] p
	where p.Void = 0
	order by p.Id;

	select [!$System!] = null, [!Products!Offset] = @Offset, [!Products!PageSize] = @PageSize, 
		[!Products!SortOrder] = @Order, [!Products!SortDir] = @Dir;
end
go
------------------------------------------------
create or alter procedure cat.[Product.Load]
@UserId bigint,
@Id bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Product!TProduct!Object] = null,
		[Id!!Id] = p.Id, [Name!!Name] = p.[Name], p.SKU, [Unit!TUnit!RefId] = p.Unit, p.Memo
	from cat.[Products] p
	where Id = @Id;-- GENERATE MAPS HERE
end
go
------------------------------------------------
drop procedure if exists cat.[Product.Update];
drop procedure if exists cat.[Product.Metadata];
drop type if exists cat.[Product.TableType];
go
------------------------------------------------
create type cat.[Product.TableType] as table (
	Id bigint,
	[Name] nvarchar(255),
	SKU nvarchar(50),
	Unit bigint,
	Memo nvarchar(255)
)
go
------------------------------------------------
create or alter procedure cat.[Product.Metadata]
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @Product cat.[Product.TableType];
	select [Product!Product!Metadata] = null, * from @Product;
end
go
------------------------------------------------
create or alter procedure cat.[Product.Update]
@UserId bigint,
@Product cat.[Product.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read committed;

	declare @rtable table(Id bigint);
	declare @Id bigint;

	merge cat.[Products] as t
	using @Product as s
	on t.Id = s.Id
	when matched then update set
		t.[Name] = s.[Name],
		t.SKU = s.SKU,
		t.Unit = s.Unit,
		t.Memo = s.Memo
	when not matched by target then insert
		([Name], SKU, Unit, Memo)
	values
		(s.[Name], s.SKU, s.Unit, s.Memo)
	output inserted.Id into @rtable(Id);

	select top (1) @Id = Id from @rtable;

	exec cat.[Product.Load] @UserId = @UserId, @Id = @Id;
end
go
