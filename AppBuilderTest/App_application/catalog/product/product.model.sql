------------------------------------------------
drop procedure if exists cat.[Product.Map]
drop type if exists cat.[Product.Map.TableType]
go
------------------------------------------------
create type cat.[Product.Map.TableType] as table (
	_rowno int identity(1, 1),
	_rowcnt int,
	Id bigint,
	Unit bigint
)
go
------------------------------------------------
create or alter procedure cat.[Product.Map]
@UserId bigint,
@Map cat.[Product.Map.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	with T as (select Unit from @Map group by Unit)
	select [!TUnit!Map] = null, [Id!!Id] = u.Id, [Name!!Name] = u.[Name]
	from T inner join cat.[Units] u on u.Id = T.Unit;
end
go
------------------------------------------------
create or alter procedure cat.[Product.Index]
@UserId bigint,
@Id bigint = null,
@Offset int = 0,
@PageSize int = 20, -- TODO: PageSize?
@Order nvarchar(255) = N'name',
@Dir nvarchar(4) = N'asc',
@Fragment nvarchar(255) = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;
	
	set @Order = lower(@Order);
	set @Dir = lower(@Dir);

	declare @fr nvarchar(255);
	set @fr = N'%' + @Fragment + N'%'

	declare @tmp cat.[Product.Map.TableType]; 
	
	insert into @tmp(Id, Unit, _rowcnt)
	select p.Id, p.Unit,
		count(*) over()
	from cat.[Products] p
	where p.Void = 0 and (@fr is null or p.Id = @Fragment or p.[Name] like @fr or p.SKU = @Fragment or p.Memo like @fr)
	order by 
		case when @Dir = N'asc' then
			case @Order
				when N'id' then p.Id
			end
		end asc,
		case when @Dir = N'desc' then
			case @Order
				when N'id' then p.Id
			end
		end desc,
		case when @Dir = N'asc' then
			case @Order
				when N'name' then p.[Name]
				when N'sku' then p.SKU
				when N'memo' then p.Memo
			end
		end asc,
		case when @Dir = N'desc' then
			case @Order
				when N'name' then p.[Name]
				when N'sku' then p.SKU
				when N'memo' then p.Memo
			end
		end desc,
		p.Id
	offset @Offset rows fetch next @PageSize rows only
	option (recompile);

	select [Products!TProduct!Array] = null,
		[Id!!Id] = p.Id, [Name!!Name] = p.[Name], p.SKU, [Unit!TUnit!RefId] = p.Unit, p.Memo,
		[!!RowCount] = _t._rowcnt			
	from @tmp _t inner join cat.[Products] p on _t.Id = p.Id
	order by _t._rowno;

	exec cat.[Product.Map] @UserId = @UserId, @Map = @tmp;

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

	declare @tmp cat.[Product.Map.TableType]; 
	
	insert into @tmp(Id, Unit)
	select p.Id, p.Unit
	from cat.[Products] p
	where p.Id = @Id;

	select [Product!TProduct!Object] = null,
		[Id!!Id] = p.Id, [Name!!Name] = p.[Name], p.SKU, [Unit!TUnit!RefId] = p.Unit, p.Memo
	from @tmp _t inner join cat.[Products] p on _t.Id = p.Id
	where p.Id = @Id;

	exec cat.[Product.Map] @UserId = @UserId, @Map = @tmp;

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
