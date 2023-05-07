// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;

using AppGenerator;
using AppGenerator.Interfaces;

namespace SqlGenerator.MsSqlServer;

public record FieldResult
{
	public String Definition { get; init; }
	public ForeignKey? ForeignKey { get; init; }
	public FieldResult(String def, ForeignKey? foreignKey)
	{
		Definition = def;
		ForeignKey = foreignKey;
	}
}

public class ForeignKey
{
	private readonly String _name;
	private readonly String _text;
	private readonly TableDescriptor _descr;
	public ForeignKey(String name, String text, TableDescriptor descr)
	{
		_name = name;
		_text = text;
		_descr = descr;
	}
	public String GenerateText()
	{
		return $@"{MsSqlServerSqlGenerator.DIVIDER}
if not exists(select * from INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE where TABLE_SCHEMA = N'{_descr.Schema.SchemaName()}' and TABLE_NAME = N'{_descr.Table.TableName}' and CONSTRAINT_NAME = N'{_name}')
	alter table {_descr.Schema.SchemaName()}.{_descr.Table.TableName} add
		{_text};
go
";
	}
}

public class MsSqlServerSqlGenerator : ISqlGenerator
{
	private readonly IModelWriter _modelWriter;
	private readonly List<String> _tables = new();
	private readonly List<ForeignKey> _foreignKeys = new();
	private readonly List<TableDescriptor> _uiElements = new();
	private readonly ILogger<ISqlGenerator> _logger;
	private AppElem _appElem = new();
	public const String DIVIDER = "------------------------------------------------";
	public MsSqlServerSqlGenerator(ILogger<ISqlGenerator> logger, IModelWriter modelWriter)
	{
		_logger = logger;
		_modelWriter = modelWriter;
	}

	public void Start(AppElem appElem)
	{
		_appElem = appElem;
	}

	public void GenerateStruct(TableDescriptor descr)
	{
		GenerateTable(descr);
	}

	public void GenerateUi(TableDescriptor descr)
	{
		_uiElements.Add(descr);
	}

	public String GenerateEndpoint(TableDescriptor descr)
	{
		if (String.IsNullOrEmpty(descr.Path))
			return String.Empty;
		_logger.LogInformation("Generate endpoint: {tableName}", descr.Table.Name);
		return new EndpointGenerator(_modelWriter, descr, _appElem).Generate();
	}

	public static String CreateSchemas()
	{
		return @$"
{DIVIDER}
-- schemas
{DIVIDER}
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'cat')
	exec sp_executesql N'create schema cat';
go
{DIVIDER}
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'doc')
	exec sp_executesql N'create schema doc';
go
{DIVIDER}
begin
	set nocount on;
	grant execute on schema ::cat to public;
	grant execute on schema ::doc to public;
end
go
";
	}
	public void Finish()
	{
		var sb = new StringBuilder();
		sb.Append(CreateSchemas());
		sb.AppendLine($"{DIVIDER}\n-- Tables");
		foreach (var table in _tables)
			sb.Append(table);
		if (_foreignKeys.Count > 0)
		{
			sb.AppendLine($"{DIVIDER}\n-- Foreign keys");
			foreach (var key in _foreignKeys)
			{
				sb.Append(key.GenerateText());
			}
		}
		var content = sb.ToString();
		_logger.LogInformation("Write file: _struct.sql");
		_modelWriter.WriteFile(content, "_sql", "_struct.sql");


		_logger.LogInformation("Write file: _ui.sql");
		_modelWriter.WriteFile(GenerateUIScript(), "_sql", "_ui.sql");
	}

	private static StringBuilder GetResource(String name)
	{
		var ass = Assembly.GetAssembly(typeof(MsSqlServerSqlGenerator));
		var stream = (ass?.GetManifestResourceStream(name)) 
			?? throw new InvalidOperationException($"Resource not found: {name}");
		using var sr = new StreamReader(stream);
		return new StringBuilder(sr.ReadToEnd());
	}

	private String GenerateUIScript()
	{
		var sqlRes = GetResource("SqlGenerator.MsSqlServer.Resources.ui.sql");
		sqlRes.Replace("$(AppTitle)", _appElem.Title ?? _appElem.Name);
		var fieldList = new List<String>();
		int id_catalog  = 100;
		int id_document = 200;
		int order_catalog = 10;
		int order_document = 10;
		foreach (var ui in _uiElements)
		{
			if (ui.Schema == "Catalog")
				fieldList.Add($"\t\t({++id_catalog},\t10,\tN'{ui.Table.Title ?? ui.Table.Name}',\tN'{ui.Table.Name?.ToLowerInvariant()!}',\t{order_catalog += 10})");
			else if (ui.Schema == "Document")
				fieldList.Add($"\t\t({++id_document},\t20,\tN'{ui.Table.Title ?? ui.Table.Name}',\tN'{ui.Table.Name?.ToLowerInvariant()!}',\t{order_document += 10})");
		}
		var fields = String.Join(",\n", fieldList) + ";";
		sqlRes.Replace("$(MenuItems)", fields);
		return sqlRes.ToString();
	}

	private void GenerateTable(TableDescriptor descr)
	{
		String sequence = $@"{DIVIDER}
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'{descr.Schema.SchemaName()}' and SEQUENCE_NAME = N'SQ_{descr.Table.TableName}')
	create sequence {descr.Schema.SchemaName()}.SQ_{descr.Table.TableName} as {_appElem.IdentifierType.SqlType()} start with 100 increment by 1;
go
";

		String header = $@"{DIVIDER}
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'{descr.Schema.SchemaName()}' and TABLE_NAME=N'{descr.Table.TableName}')
create table {descr.Schema.SchemaName()}.{descr.Table.TableName}
(
";

		var sb = new StringBuilder();
		if (_appElem.IdentifierType.HasSequence())
			sb.Append(sequence);
		sb.Append(header);
		var fields = new List<String>();
		foreach (var field in descr.Table.Fields)
		{
			var fr = FieldDefinition(field, descr);
			fields.Add(fr.Definition);
			if (fr.ForeignKey != null)
				_foreignKeys.Add(fr.ForeignKey);

		}
		// primary key
		String pkFields = _appElem.MultiTenant ? "TenantId, Id" : "Id";
		fields.Add($"\tconstraint PK_{descr.Table.TableName} primary key ({pkFields})");
		sb.AppendLine(String.Join(",\n", fields));
		sb.AppendLine(");\ngo");

		var tableDef = sb.ToString();
		_tables.Add(tableDef);
	}
	public FieldResult FieldDefinition(FieldElem field, TableDescriptor tableDescr)
	{
		var nullable = field.Required ? " not null" : String.Empty;
		String? foreignKey = null;
		var defaultConstraint = String.Empty;
		var tableName = tableDescr.Table.TableName;
		ForeignKey? foreignKeyResult = null;
		if (field.Type == FieldType.Reference)
		{
			if (field.RefTable == null)
				throw new InvalidOperationException($"The reference '{field.Name}' is empty");
			var refTable = _appElem.FindTableByReference(field.RefTable);
			var foreignKeyName = $"FK_{tableName}_{field.Name}_{refTable.Table.TableName}";
			String pkName = _appElem.MultiTenant ? $"TenantId, {refTable.Table.PrimaryKey.Name}" : $"{refTable.Table.PrimaryKey.Name}";
			foreignKey = $"constraint {foreignKeyName} foreign key ({field.Name}) references {refTable.Schema.SchemaName()}.{refTable.Table.TableName}({pkName})";
			foreignKeyResult = new ForeignKey(foreignKeyName, foreignKey, tableDescr);
		}
		String fkText = String.Empty;
		if (foreignKey != null)
			fkText = $"\n\t\t/* {foreignKey} */";
		if (field.IsId && _appElem.IdentifierType.HasSequence())
			defaultConstraint = $"\n\t\tconstraint DF_{tableName}_{field.Name} default (next value for {tableDescr.Schema.SchemaName()}.SQ_{tableName})";
		else if (field.Required && field.Name == "Void")
			defaultConstraint = $"\n\t\tconstraint DF_{tableName}_{field.Name} default (0)";
		return new FieldResult($"\t{field.Name.Escape()} {field.SqlType(_appElem.IdentifierType)}{nullable}{defaultConstraint}{fkText}", foreignKeyResult);
	}
}