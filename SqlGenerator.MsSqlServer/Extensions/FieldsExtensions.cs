// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using AppGenerator;

namespace SqlGenerator.MsSqlServer;
internal static class FieldsExtensions
{
	public static Boolean HasSequence(this IdentifierType identifierType)
	{
		return identifierType == IdentifierType.Integer || identifierType == IdentifierType.BigInt;
	}

	public static String SqlType(this IdentifierType identifierType)
	{
		return identifierType switch
		{
			IdentifierType.Integer => "int",
			IdentifierType.BigInt => "bigint",
			IdentifierType.Guid => "uniqueidentifier",
			_ =>
			throw new InvalidOperationException($"Unknown identifier type: {identifierType}")
		};
	}

	public static String SqlType(this FieldElem elem, IdentifierType identType)
	{
		return elem.Type switch
		{
			FieldType.String => $"nvarchar({elem.Length})",
			FieldType.Integer => "int",
			FieldType.BigInt or FieldType.Money or FieldType.Float or
			FieldType.Date or FieldType.DateTime
				=> elem.Type.ToString().ToLowerInvariant(),
			FieldType.Guid => "uniqueidentifier",
			FieldType.Boolean => $"bit",
			FieldType.Reference => identType.ToString().ToLowerInvariant(),
			FieldType.Identifier => $"{identType.ToString().ToLowerInvariant()}",
			_ => throw new NotImplementedException()
		};
	}

	public static String FieldTextForTableType(this FieldElem elem, IdentifierType identType)
	{
		return $"{elem.Name.Escape()} {elem.SqlType(identType)}";
	}
	public static String ReferenceItem(this FieldElem field)
	{
		if (!field.IsReference || field.RefTable == null)
			throw new InvalidOperationException("Field is not reference");
		return field.RefTable.Split(".")[1];
	}

	public static String FieldNameForSelect(this FieldElem field, String alias)
	{
		var escname = $"{alias}.{field.Name.Escape()}";
		return field switch
		{
			{ IsId: true } => $"[Id!!Id] = {escname}",
			{ IsName: true} => $"[Name!!Name] = {escname}",
			{ IsReference: true} => $"[{field.Name}!T{field.ReferenceItem()}!RefId] = {escname}",
			_ => escname
		};
	}
}
