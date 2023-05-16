------------------------------------------------
drop procedure if exists cat.[Unit.Map]
drop type if exists cat.[Unit.Map.TableType]
go
------------------------------------------------
create type cat.[Unit.Map.TableType] as table (
	_rowno int identity(1, 1),
	_rowcnt int,
	Id bigint
)
go
------------------------------------------------
create or alter procedure cat.[Unit.Load]
@UserId bigint,
@Id bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @tmp cat.[Unit.Map.TableType]; 
	
	insert into @tmp(Id)
	select u.Id
	from cat.[Units] u
	where u.Id = @Id;

	select [Unit!TUnit!Object] = null,
		[Id!!Id] = u.Id, [Name!!Name] = u.[Name], u.Memo
	from @tmp _t inner join cat.[Units] u on _t.Id = u.Id
	where u.Id = @Id;
end
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
