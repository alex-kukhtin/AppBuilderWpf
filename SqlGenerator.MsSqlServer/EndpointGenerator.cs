// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
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

	String GenerateIndex()
	{
		var sb = new StringBuilder();

		var procName = $"{_descr.Schema.SchemaName()}.[{_descr.Table.Name}.Index]";
		List<String> aliases = new();
		var alias = _descr.SqlTableAlias(aliases);
		aliases.Add(alias);
		var fields = _descr.Table.Fields.Where(f => !f.IsVoid)
			.Select(f => f.FieldNameForSelect(alias));

		var propName = _descr.Table.TableName;
		String index = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create or alter procedure {procName}
			{TenantParam()}@UserId bigint,
			@Id {_root.IdentifierType.SqlType()} = null,
			@Offset int = 0,
			@PageSize int = 20,
			@Order nvarchar(255) = N'date', -- TODO: initial sort field
			@Dir nvarchar(20) = N'asc' -- TODO: initial sort direction
			as
			begin
				set nocount on;
				set transaction isolation level read uncommitted;

				select [{propName}!T{_descr.Table.Name}!Array] = null,
					{String.Join(", ", fields)}
				from {_descr.SqlTableName()} {alias}
				where {alias}.Void = 0
				order by {alias}.Id;

				select [!$System!] = null, [!{propName}!Offset] = @Offset, [!{propName}!PageSize] = @PageSize, 
					[!{propName}!SortOrder] = @Order, [!{propName}!SortDir] = @Dir;
			end
			go
			""";
		sb.Append(index);
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


		String load = $"""
			{MsSqlServerSqlGenerator.DIVIDER}
			create or alter procedure {procName}
			{TenantParam()}@UserId bigint,
			@Id {_root.IdentifierType.SqlType()} = null
			as
			begin
				set nocount on;
				set transaction isolation level read uncommitted;

				select [{_descr.Table.Name}!T{_descr.Table.Name}!Object] = null,
					{String.Join(", ", fields)}
				from {_descr.SqlTableName()} {alias}
				where {TenantWhere()}Id = @Id;
			""";
		sb.Append(load);
		if (hasDetails)
		{
			sb.AppendLine();
			sb.AppendLine("-- GENERATE DETAILS HERE");
		}
		if (hasRefs)
		{
			sb.AppendLine("-- GENERATE MAPS HERE");
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
		var fields = _descr.Table.Fields.Where(f => !f.IsId && !f.IsVoid);
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

	public String Generate()
	{
		String fileName = $"{_descr.Table.Name!.ToLowerInvariant()}.model.sql";
		var sb = new StringBuilder();	
		sb.AppendLine(GenerateIndex());
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
