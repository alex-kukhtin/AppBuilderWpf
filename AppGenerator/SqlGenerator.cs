// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

using AppGenerator.Interfaces;

namespace AppGenerator;

public class SqlGenerator : ISqlGenerator
{
	private readonly IModelWriter _modelWriter;
	private readonly List<String> _tables = new();
	private readonly ILogger<ISqlGenerator> _logger;
	public SqlGenerator(ILogger<ISqlGenerator> logger, IModelWriter modelWriter)
	{
		_logger = logger;
		_modelWriter = modelWriter;
	}

	public void Generate(TableDescriptor descr, AppElem appElem)
	{
		GenerateTable(descr, appElem);
	}

	public void Finish()
	{
		var sb = new StringBuilder();
		foreach (var table in _tables)
			sb.Append(table);
		_modelWriter.WriteFile(sb.ToString(), "_sql", "struct.sql");
	}

	private void GenerateTable(TableDescriptor descr, AppElem appElem)
	{
		const String sequence = @"
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'app' and SEQUENCE_NAME = N'SQ_$(TableName)')
	create sequence app.SQ_$(TableName) as bigint start with 100 increment by 1;
go";
		const String header = @"
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'$(TableSchema)' and TABLE_NAME=N'$(TableName)')
create table $(TableSchema).$(TableName)
(
";
		void ReplaceTableMacros(StringBuilder sb)
		{
			sb.Replace("$(TableSchema)", descr.Schema);
			sb.Replace("$(TableName)", descr.Table.Name.Pluralize());
		}

		var sb = new StringBuilder();
		if (appElem.IdentifierType == IdentifierType.Integer || appElem.IdentifierType == IdentifierType.BigInt)
			sb.Append(sequence);
		sb.Append(header);
		ReplaceTableMacros(sb);
		var fields = new List<String>();
		foreach (var field in descr.Table.Fields)
		{
			fields.Add(DefineStructField(field, appElem.IdentifierType));
		}
		sb.AppendLine(String.Join(",\n", fields));
		sb.AppendLine(");\ngo");

		var tableDef = sb.ToString();
		_tables.Add(tableDef);
	}

	private String DefineStructField(FieldElem field, IdentifierType identType)
	{
		return $"\t{field.Name} {field.SqlType(identType)}";
	}
}

