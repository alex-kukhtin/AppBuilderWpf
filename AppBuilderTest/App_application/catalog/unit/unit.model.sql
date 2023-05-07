------------------------------------------------
create or alter procedure cat.[Unit.Index]
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

	select [Units!TUnit!Array] = null,
		[Id!!Id] = u.Id, [Name!!Name] = u.[Name], u.Memo
	from cat.[Units] u
	where u.Void = 0
	order by u.Id;

	select [!$System!] = null, [!Units!Offset] = @Offset, [!Units!PageSize] = @PageSize, 
		[!Units!SortOrder] = @Order, [!Units!SortDir] = @Dir;
end
go
------------------------------------------------
create or alter procedure cat.[Unit.Load]
@UserId bigint,
@Id bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Unit!TUnit!Object] = null,
		[Id!!Id] = u.Id, [Name!!Name] = u.[Name], u.Memo
	from cat.[Units] u
	where Id = @Id;end
go
------------------------------------------------
drop procedure if exists cat.[Unit.Update];
drop procedure if exists cat.[Unit.Metadata];
drop type if exists cat.[Unit.TableType];
go
------------------------------------------------
create type cat.[Unit.TableType] as table (
	Id bigint,
	[Name] nvarchar(255),
	Memo nvarchar(255)
)
go
------------------------------------------------
create or alter procedure cat.[Unit.Metadata]
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @Unit cat.[Unit.TableType];
	select [Unit!Unit!Metadata] = null, * from @Unit;
end
go
------------------------------------------------
create or alter procedure cat.[Unit.Update]
@UserId bigint,
@Unit cat.[Unit.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read committed;

	declare @rtable table(Id bigint);
	declare @Id bigint;

	merge cat.[Units] as t
	using @Unit as s
	on t.Id = s.Id
	when matched then update set
		t.[Name] = s.[Name],
		t.Memo = s.Memo
	when not matched by target then insert
		([Name], Memo)
	values
		(s.[Name], s.Memo)
	output inserted.Id into @rtable(Id);

	select top (1) @Id = Id from @rtable;

	exec cat.[Unit.Load] @UserId = @UserId, @Id = @Id;
end
go
