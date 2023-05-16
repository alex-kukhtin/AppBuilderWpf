------------------------------------------------
drop procedure if exists doc.[Document.Map]
drop type if exists doc.[Document.Map.TableType]
go
------------------------------------------------
create type doc.[Document.Map.TableType] as table (
	_rowno int identity(1, 1),
	_rowcnt int,
	Id bigint,
	Agent bigint
)
go
------------------------------------------------
create or alter procedure doc.[Document.Map]
@UserId bigint,
@Map doc.[Document.Map.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	with T as (select Agent from @Map group by Agent)
	select [!TAgent!Map] = null, [Id!!Id] = a.Id, [Name!!Name] = a.[Name]
	from T inner join cat.[Agents] a on a.Id = T.Agent;
end
go
------------------------------------------------
create or alter procedure doc.[Document.Load]
@UserId bigint,
@Id bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @tmp doc.[Document.Map.TableType]; 
	
	insert into @tmp(Id, Agent)
	select d.Id, d.Agent
	from doc.[Documents] d
	where d.Id = @Id;

	select [Document!TDocument!Object] = null,
		[Id!!Id] = d.Id, d.[Date], d.[No], d.[Sum], [Agent!TAgent!RefId] = d.Agent, d.Memo, [DocDetails!TDocDetails!Array] = null
	from @tmp _t inner join doc.[Documents] d on _t.Id = d.Id
	where d.Id = @Id;

-- GENERATE DETAILS HERE --

	exec doc.[Document.Map] @UserId = @UserId, @Map = @tmp;

end
go
------------------------------------------------
drop procedure if exists doc.[Document.Update];
drop procedure if exists doc.[Document.Metadata];
drop type if exists doc.[Document.TableType];
go
------------------------------------------------
create type doc.[Document.TableType] as table (
	Id bigint,
	[Date] date,
	[No] nvarchar(50),
	[Sum] money,
	Agent bigint,
	Memo nvarchar(255)
)
go
------------------------------------------------
create or alter procedure doc.[Document.Metadata]
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @Document doc.[Document.TableType];
	select [Document!Document!Metadata] = null, * from @Document;
end
go
------------------------------------------------
create or alter procedure doc.[Document.Update]
@UserId bigint,
@Document doc.[Document.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read committed;

	declare @rtable table(Id bigint);
	declare @Id bigint;

	merge doc.[Documents] as t
	using @Document as s
	on t.Id = s.Id
	when matched then update set
		t.[Date] = s.[Date],
		t.[No] = s.[No],
		t.[Sum] = s.[Sum],
		t.Agent = s.Agent,
		t.Memo = s.Memo
	when not matched by target then insert
		([Date], [No], [Sum], Agent, Memo)
	values
		(s.[Date], s.[No], s.[Sum], s.Agent, s.Memo)
	output inserted.Id into @rtable(Id);

	select top (1) @Id = Id from @rtable;

	exec doc.[Document.Load] @UserId = @UserId, @Id = @Id;
end
go
