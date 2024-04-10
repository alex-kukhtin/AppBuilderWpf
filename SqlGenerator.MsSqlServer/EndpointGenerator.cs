// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using AppGenerator;
using AppGenerator.Interfaces;

namespace SqlGenerator.MsSqlServer;

internal class EndpointGenerator
{
	private readonly TableDescriptor _descr;
	private readonly AppElem _root;
	private readonly IModelWriter _writer;
	public EndpointGenerator(IModelWriter writer, TableDescriptor descr, AppElem root)
	{
		_writer = writer;
		_descr = descr;
		_root = root;	
	}

	String InvokeTenantParam()
	{
		if (_root.MultiTenant)
			throw new NotImplementedException();
		return String.Empty;
	}

	String TenantParam()
	{
		if (_root.MultiTenant)
			throw new NotImplementedException();
		return String.Empty;
	}
	String TenantWhere()
	{
		if (_root.MultiTenant)
			throw new NotImplementedException();
		return String.Empty;
	}

	String OrderByGroup(IGrouping<String, FieldElem> group, String alias) 
	{
		var whenFields = group.Select(f => $"\t\t\t\twhen N'{f.Name.ToLowerInvariant()}' then {alias}.{f.Name.Escape()}");
		var groupString = $@"
		case when @Dir = N'asc' then
			case @Order
{String.Join("\n", whenFields)}
			end
		end asc,
		case when @Dir = N'desc' then
			case @Order
{String.Join("\n", whenFields)}
			end
		end desc,";
		return groupString;
	}

	String GenerateOrderByIndex(TableDescriptor descr, UIElemForm uiForm, String alias)
	{
		var groups = uiForm.Fields.Where(f => f.Sort).Select(f => descr.FindField(f.Field)).GroupBy(f => f.SqlTypeGroup(_root.IdentifierType));
		if (!groups.Any())
			return $"{alias}.{_descr.PrimaryKeyName()}";
		var sb = new StringBuilder();
		foreach (var group in groups)
		{
			var grp = OrderByGroup(group, alias);
			sb.Append(grp);
		}
		sb.Append($"\n\t\t{alias}.{_descr.PrimaryKeyName()}");
		return sb.ToString();
	}

	String GenerateIndex(UIElemForm uiForm)
	{
		var sb = new StringBuilder();

		//var procName = _descr.IndexProcName();

		List<String> aliases = new();
		var alias = _descr.SqlTableAlias(aliases);
		aliases.Add(alias);

		var fields = _descr.Table.Fields.Where(f => !f.IsVoid)
			.Select(f => f.FieldNameForSelect(alias));

		var propName = _descr.Table.TableName;
		var iderType = _root.IdentifierType.SqlType();
		var sortDescr = uiForm.RealSort();
		var pkName = _descr.PrimaryKeyName();
		var voidName = _descr.VoidName();
		var mapFields = _descr.Table.Fields.Where(f => f.IsPrimaryKey || f.IsReference)
			.Select(f => f.Name.Escape());
		var mapFieldsAlias = _descr.Table.Fields.Where(f => f.IsPrimaryKey || f.IsReference)
			.Select(f => $"{alias}.{f.Name.Escape()}");

		String searchWhere = String.Empty;
		String frText = String.Empty;
		if (uiForm.HasSearch())
		{
			var searchFields = uiForm.Fields.Where(f => f.IsSearch)
				.Select(f => _descr.FindField(f.Field).LikeDeclaration(f, alias));
			searchWhere = $" and (@fr is null or {String.Join( " or ", searchFields)})";
			frText = "\n\n\tdeclare @fr nvarchar(255);\n\tset @fr = N'%' + @Fragment + N'%'";
		}

		String index = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create or alter procedure {_descr.IndexProcName()}
			{TenantParam()}@UserId bigint,
			@Id {iderType} = null,
			@Offset int = 0,
			@PageSize int = 20, -- TODO: PageSize?
			@Order nvarchar(255) = N'{sortDescr.OrderBy}',
			@Dir nvarchar(4) = N'{sortDescr.Direction}'{(uiForm.HasSearch() ? ",\n@Fragment nvarchar(255) = null": "")}
			as
			begin
				set nocount on;
				set transaction isolation level read uncommitted;
				
				set @Order = lower(@Order);
				set @Dir = lower(@Dir);{frText}

				declare @tmp {_descr.MapTableTypeName()}; 
				
				insert into @tmp({String.Join(", ", mapFields)}, _rowcnt)
				select {String.Join(", ", mapFieldsAlias)},
					count(*) over()
				from {_descr.SqlTableName()} {alias}
				where {TenantWhere()}{alias}.{voidName} = 0{searchWhere}
				order by {GenerateOrderByIndex(_descr, uiForm, alias)}
				offset @Offset rows fetch next @PageSize rows only
				option (recompile);

				select [{propName}!T{_descr.Table.Name}!Array] = null,
					{String.Join(", ", fields)},
					[!!RowCount] = _t._rowcnt			
				from @tmp _t inner join {_descr.SqlTableName()} {alias} on _t.{pkName} = {alias}.{pkName}
				order by _t._rowno;
			""";
		String indexEnd = $"""
				select [!$System!] = null, [!{propName}!Offset] = @Offset, [!{propName}!PageSize] = @PageSize, 
					[!{propName}!SortOrder] = @Order, [!{propName}!SortDir] = @Dir;
			end
			go
			""";
		sb.AppendLine(index);
		if (_descr.HasMaps())
			sb.AppendLine($"\n\texec {_descr.MapProcName()} {InvokeTenantParam()}@UserId = @UserId, @Map = @tmp;\n");
		else
			sb.AppendLine();
		sb.Append(indexEnd);
		return sb.ToString();
	}

	String GenerateLoad()
	{
		var sb = new StringBuilder();
		var procName = _descr.LoadProcName();

		List<String> aliases = new();
		var alias = _descr.SqlTableAlias(aliases);
		aliases.Add(alias);

		var hasDetails = _descr.Table.Details.Any();
		var details = _descr.Table.Details.Select(d => $"[{d.Name}!T{d.Name}!Array] = null");

		var fields = _descr.Table.Fields.Where(f => !f.IsVoid)
			.Select(f => f.FieldNameForSelect(alias)).Union(details);
		var hasRefs = _descr.Table.Fields.Any(f => f.IsReference);

		var mapFields = _descr.Table.Fields.Where(f => f.IsPrimaryKey || f.IsReference)
			.Select(f => f.Name.Escape());
		var mapFieldsAlias = _descr.Table.Fields.Where(f => f.IsPrimaryKey || f.IsReference)
			.Select(f => $"{alias}.{f.Name.Escape()}");

		var pkName = _descr.PrimaryKeyName();
		String load = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create or alter procedure {procName}
			{TenantParam()}@UserId bigint,
			@Id {_root.IdentifierType.SqlType()} = null
			as
			begin
				set nocount on;
				set transaction isolation level read uncommitted;

				declare @tmp {_descr.MapTableTypeName()}; 
				
				insert into @tmp({String.Join(", ", mapFields)})
				select {String.Join(", ", mapFieldsAlias)}
				from {_descr.SqlTableName()} {alias}
				where {TenantWhere()}{alias}.{pkName} = @Id;

				select [{_descr.Table.Name}!T{_descr.Table.Name}!Object] = null,
					{String.Join(", ", fields)}
				from @tmp _t inner join {_descr.SqlTableName()} {alias} on _t.{pkName} = {alias}.{pkName}
				where {TenantWhere()}{alias}.{pkName} = @Id;
			""";
		sb.AppendLine(load);
		if (hasDetails)
		{
			sb.AppendLine();
			sb.AppendLine("-- GENERATE DETAILS HERE --");
		}
		if (hasRefs)
		{
			sb.AppendLine($"\n\texec {_descr.MapProcName()} {InvokeTenantParam()}@UserId = @UserId, @Map = @tmp;\n");
		}
		sb.Append("end\ngo");
		return sb.ToString();
	}

	public String GenerateDrop()
	{
		var sb = new StringBuilder();
		String meta = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			drop procedure if exists {_descr.UpdateProcName()};
			drop procedure if exists {_descr.MetadataProcName()};
			drop type if exists {_descr.TableTypeName()};
			go
			""";
		sb.Append(meta);
		return sb.ToString();
	}
	public String GenerateCreateType()
	{
		var sb = new StringBuilder();
		var fields = _descr.Table.Fields
			.Where(f => f.Name != "Void")
			.Select(f => f.FieldTextForTableType(_root.IdentifierType));
		String createType = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create type {_descr.TableTypeName()} as table (
				{String.Join(",\n\t", fields)}
			)
			go
			""";
		sb.Append(createType);
		return sb.ToString();
	}
	public String GenerateMetadata()
	{
		var sb = new StringBuilder();
		var procName = _descr.MetadataProcName();
		var elem = _descr.Table.Name;
		String meta = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create or alter procedure {procName}
			as
			begin
				set nocount on;
				set transaction isolation level read uncommitted;

				declare @{elem} {_descr.TableTypeName()};
				select [{elem}!{elem}!Metadata] = null, * from @{elem};
			end
			go
			""";
		sb.Append(meta);
		return sb.ToString();
	}
	public String GenerateUpdate()
	{
		var sb = new StringBuilder();
		var procName = _descr.UpdateProcName();
		var elem = _descr.Table.Name;
		var iderType = _root.IdentifierType.SqlType();
		var fields = _descr.Table.Fields.Where(f => !f.IsPrimaryKey && !f.IsVoid);
		var updateFields = fields.Select(f => $"t.{f.Name.Escape()} = s.{f.Name.Escape()}");
		var insertFieldsSource = fields.Select(f => $"{f.Name.Escape()}");
		var insertFieldsTarget = fields.Select(f => $"s.{f.Name.Escape()}");

		String update = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create or alter procedure {procName}
			{TenantParam()}@UserId bigint,
			@{elem} {_descr.TableTypeName()} readonly
			as
			begin
				set nocount on;
				set transaction isolation level read committed;

				declare @rtable table(Id {iderType});
				declare @Id {iderType};

				merge {_descr.SqlTableName()} as t
				using @{elem} as s
				on {TenantWhere()}t.Id = s.Id
				when matched then update set
					{String.Join(",\n\t\t",updateFields)}
				when not matched by target then insert
					({String.Join(", ", insertFieldsSource)})
				values
					({String.Join(", ", insertFieldsTarget)})
				output inserted.Id into @rtable(Id);

				select top (1) @Id = Id from @rtable;

				exec {_descr.LoadProcName()} {InvokeTenantParam()}@UserId = @UserId, @Id = @Id;
			end
			go
			""";
		sb.Append(update);
		return sb.ToString();
	}

	public String GenerateMapSelect(FieldElem field, TableDescriptor refTable, String alias)
	{
		var fn = field.Name.Escape();
		var primaryKey = refTable.PrimaryKeyName();
		var nameField = refTable.NameFieldName();
		var template = $"""			
			with T as (select {fn} from @Map group by {field.Name.Escape()})
			select [!T{field.Name}!Map] = null, [Id!!Id] = {alias}.{primaryKey}, [Name!!Name] = {alias}.{nameField}
			from T inner join {refTable.SqlTableName()} {alias} on {alias}.{primaryKey} = T.{fn};
		""";
		return template;
	}
	public String GenerateIndexMap()
	{
		var sb = new StringBuilder();

		var mapFields = _descr.Table.Fields.Where(f => f.IsPrimaryKey || f.IsReference)
			.Select(f => $"{f.Name.Escape()} {f.SqlType(_root.IdentifierType)}");
		String start = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			drop procedure if exists {_descr.MapProcName()}
			drop type if exists {_descr.MapTableTypeName()}
			go
			{MsSqlServerSqlGenerator.DIVIDER}
			create type {_descr.MapTableTypeName()} as table (
				_rowno int identity(1, 1),
				_rowcnt int,
				{String.Join(",\n\t", mapFields)}
			)
			go
			""";
		String mapStart = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create or alter procedure {_descr.MapProcName()}
			{TenantParam()}@UserId bigint,
			@Map {_descr.MapTableTypeName()} readonly
			as
			begin
				set nocount on;
				set transaction isolation level read uncommitted;

			""";
		String mapEnd = """
			end
			go
			""";
		sb.Append(start);

		List<String> aliases = new();

		if (_descr.HasMaps())
		{
			sb.AppendLine();
			sb.Append(mapStart);
			foreach (var f in _descr.Table.Fields.Where(f => f.IsReference))
			{
				if (f.RefTable == null)
					throw new InvalidOperationException("RefTable is null");
				var refTable = _root.FindTableByReference(f.RefTable);
				var alias = refTable.SqlTableAlias(aliases);
				aliases.Add(alias);
				sb.AppendLine();
				sb.AppendLine(GenerateMapSelect(f, refTable, alias));
			}
			sb.Append(mapEnd);
		}
		return sb.ToString();
	}
	public String Generate()
	{
		String fileName = $"{_descr.Table.Name!.ToLowerInvariant()}.model.sql";
		var sb = new StringBuilder();
		sb.AppendLine(GenerateIndexMap());
		var ix = _descr.Table.Ui?.Index;
		if (ix != null)
			sb.AppendLine(GenerateIndex(ix));
		sb.AppendLine(GenerateLoad());
		sb.AppendLine(GenerateDrop());
		sb.AppendLine(GenerateCreateType());
		sb.AppendLine(GenerateMetadata());
		sb.AppendLine(GenerateUpdate());
		sb.Replace("\r\n", "\n");
		_writer.WriteFile(sb.ToString(), _descr.Path, fileName);
		return $"{_descr.Path}/{fileName}";
	}
}
