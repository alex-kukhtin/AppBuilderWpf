------------------------------------------------
drop procedure if exists cat.[Agent.Map]
drop type if exists cat.[Agent.Map.TableType]
go
------------------------------------------------
create type cat.[Agent.Map.TableType] as table (
	_rowno int identity(1, 1),
	_rowcnt int,
	Id bigint
)
go
------------------------------------------------
create or alter procedure cat.[Agent.Index]
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

	declare @tmp cat.[Agent.Map.TableType]; 
	
	insert into @tmp(Id, _rowcnt)
	select a.Id,
		count(*) over()
	from cat.[Agents] a
	where a.Void = 0 and (@fr is null or a.[Name] like @fr or a.Memo like @fr or a.Code like @fr)
	order by 
		case when @Dir = N'asc' then
			case @Order
				when N'name' then a.[Name]
				when N'memo' then a.Memo
				when N'code' then a.Code
			end
		end asc,
		case when @Dir = N'desc' then
			case @Order
				when N'name' then a.[Name]
				when N'memo' then a.Memo
				when N'code' then a.Code
			end
		end desc,
		a.Id
	offset @Offset rows fetch next @PageSize rows only
	option (recompile);

	select [Agents!TAgent!Array] = null,
		[Id!!Id] = a.Id, [Name!!Name] = a.[Name], a.Memo, a.Code,
		[!!RowCount] = _t._rowcnt			
	from @tmp _t inner join cat.[Agents] a on _t.Id = a.Id
	order by _t._rowno;

	select [!$System!] = null, [!Agents!Offset] = @Offset, [!Agents!PageSize] = @PageSize, 
		[!Agents!SortOrder] = @Order, [!Agents!SortDir] = @Dir;
end
go
------------------------------------------------
create or alter procedure cat.[Agent.Load]
@UserId bigint,
@Id bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @tmp cat.[Agent.Map.TableType]; 
	
	insert into @tmp(Id)
	select a.Id
	from cat.[Agents] a
	where a.Id = @Id;

	select [Agent!TAgent!Object] = null,
		[Id!!Id] = a.Id, [Name!!Name] = a.[Name], a.Memo, a.Code
	from @tmp _t inner join cat.[Agents] a on _t.Id = a.Id
	where a.Id = @Id;
end
go
------------------------------------------------
drop procedure if exists cat.[Agent.Update];
drop procedure if exists cat.[Agent.Metadata];
drop type if exists cat.[Agent.TableType];
go
------------------------------------------------
create type cat.[Agent.TableType] as table (
	Id bigint,
	[Name] nvarchar(255),
	Memo nvarchar(255),
	Code nvarchar(50)
)
go
------------------------------------------------
create or alter procedure cat.[Agent.Metadata]
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @Agent cat.[Agent.TableType];
	select [Agent!Agent!Metadata] = null, * from @Agent;
end
go
------------------------------------------------
create or alter procedure cat.[Agent.Update]
@UserId bigint,
@Agent cat.[Agent.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read committed;

	declare @rtable table(Id bigint);
	declare @Id bigint;

	merge cat.[Agents] as t
	using @Agent as s
	on t.Id = s.Id
	when matched then update set
		t.[Name] = s.[Name],
		t.Memo = s.Memo,
		t.Code = s.Code
	when not matched by target then insert
		([Name], Memo, Code)
	values
		(s.[Name], s.Memo, s.Code)
	output inserted.Id into @rtable(Id);

	select top (1) @Id = Id from @rtable;

	exec cat.[Agent.Load] @UserId = @UserId, @Id = @Id;
end
go
