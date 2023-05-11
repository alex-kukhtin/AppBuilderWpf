------------------------------------------------
drop procedure if exists doc.[Document.Map]
drop type if exists doc.[Document.Map.TableType]
go
------------------------------------------------
create type doc.[Document.Map.TableType] as table (
	_rowno int identity(1, 1),
	_rowcnt int
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
end
go
------------------------------------------------
create or alter procedure doc.[Document.Index]
@UserId bigint,
@Id bigint = null,
@Offset int = 0,
@PageSize int = 20, -- TODO: PageSize?
@Order nvarchar(255) = N'date',
@Dir nvarchar(4) = N'desc'
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @tmp doc.[Document.Map.TableType]; 
	
	select [Documents!TDocument!Array] = null,
		[Id!!Id] = d.Id, d.[Date], d.[No], d.Memo, [Agent!TAgent!RefId] = d.Agent
	from doc.[Documents] d
	where d.Void = 0
	order by d.Id;

	exec doc.[Document.Map] @UserId = @UserId, @Map = @tmp;

	select [!$System!] = null, [!Documents!Offset] = @Offset, [!Documents!PageSize] = @PageSize, 
		[!Documents!SortOrder] = @Order, [!Documents!SortDir] = @Dir;
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

	select [Document!TDocument!Object] = null,
		[Id!!Id] = d.Id, d.[Date], d.[No], d.Memo, [Agent!TAgent!RefId] = d.Agent, [DocDetails!TDocDetails!Array] = null
	from doc.[Documents] d
	where Id = @Id;

-- GENERATE DETAILS HERE --
-- GENERATE MAPS HERE --
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
	Memo nvarchar(255),
	Agent bigint
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
		t.Memo = s.Memo,
		t.Agent = s.Agent
	when not matched by target then insert
		([Date], [No], Memo, Agent)
	values
		(s.[Date], s.[No], s.Memo, s.Agent)
	output inserted.Id into @rtable(Id);

	select top (1) @Id = Id from @rtable;

	exec doc.[Document.Load] @UserId = @UserId, @Id = @Id;
end
go
