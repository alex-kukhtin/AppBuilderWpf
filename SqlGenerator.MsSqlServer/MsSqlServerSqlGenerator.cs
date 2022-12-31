using AppGenerator;
using AppGenerator.Interfaces;
using Microsoft.Extensions.Logging;
using SqlGenerator.MsSqlServer.Extensions;
using System.Text;

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
	String _name;
	String _text;
	TableDescriptor _descr;
	public ForeignKey(String name, String text, TableDescriptor descr)
	{
		_name = name;
		_text = text;
		_descr = descr;
	}
	public String GenerateText()
	{
		return $@"------------------------------------------------
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
	private readonly ILogger<ISqlGenerator> _logger;
	public MsSqlServerSqlGenerator(ILogger<ISqlGenerator> logger, IModelWriter modelWriter)
	{
		_logger = logger;
		_modelWriter = modelWriter;
	}

	public void Generate(TableDescriptor descr, AppElem appElem)
	{
		GenerateTable(descr, appElem);
	}

	public String Finish()
	{
		var sb = new StringBuilder();
		foreach (var table in _tables)
			sb.Append(table);
		if (_foreignKeys.Count > 0)
		{
			sb.AppendLine("------------------------------------------------\n-- Foreign keys");
			foreach (var key in _foreignKeys)
			{
				sb.Append(key.GenerateText());
			}
		}
		var content = sb.ToString();
		_modelWriter.WriteFile(content, "_sql", "struct.sql");
		return content;
	}

	private void GenerateTable(TableDescriptor descr, AppElem appElem)
	{
		String sequence = $@"------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'{descr.Schema.SchemaName()}' and SEQUENCE_NAME = N'SQ_{descr.Table.TableName}')
	create sequence {descr.Schema.SchemaName()}.SQ_{descr.Table.TableName} as bigint start with 100 increment by 1;
go
";

		String header = $@"------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'{descr.Schema.SchemaName()}' and TABLE_NAME=N'{descr.Table.TableName}')
create table {descr.Schema.SchemaName()}.{descr.Table.TableName}
(
";

		var sb = new StringBuilder();
		if (appElem.IdentifierType == IdentifierType.Integer || appElem.IdentifierType == IdentifierType.BigInt)
			sb.Append(sequence);
		sb.Append(header);
		var fields = new List<String>();
		foreach (var field in descr.Table.Fields)
		{
			var fr = FieldDefinition(field, descr, appElem);
			fields.Add(fr.Definition);
			if (fr.ForeignKey != null)
				_foreignKeys.Add(fr.ForeignKey);

		}
		sb.AppendLine(String.Join(",\n", fields));
		sb.AppendLine(");\ngo");

		var tableDef = sb.ToString();
		_tables.Add(tableDef);
	}
	public static FieldResult FieldDefinition(FieldElem field, TableDescriptor tableDescr, AppElem root)
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
			var refTable = root.FindTableByReference(field.RefTable);
			var foreignKeyName = $"FK_{tableName}_{field.Name}_{refTable.Table.TableName}";
			foreignKey = $"constraint {foreignKeyName} foreign key references {refTable.Schema.SchemaName()}.{refTable.Table.TableName}({refTable.Table.PrimaryKey.Name})";
			foreignKeyResult = new ForeignKey(foreignKeyName, foreignKey, tableDescr);
		}
		String fkText = String.Empty;
		if (foreignKey != null)
			fkText = $"\n\t\t/* {foreignKey} */";
		if (field.IsId && root.IdentifierType.HasSequence())
			defaultConstraint = $"\n\t\tconstraint DF_{tableName}_{field.Name} default (next value for {tableDescr.Schema.SchemaName()}.SQ_{tableName})";
		return new FieldResult($"\t{field.Name.Escape()} {field.SqlType(root.IdentifierType)}{nullable}{defaultConstraint}{fkText}", foreignKeyResult);
	}
}