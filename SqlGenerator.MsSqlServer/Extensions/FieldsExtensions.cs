// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using AppGenerator;

namespace SqlGenerator.MsSqlServer;
internal static class FieldsExtensions
{
	public static Boolean HasSequence(this IdentifierType identifierType)
	{
		return identifierType == IdentifierType.Integer || identifierType == IdentifierType.BigInt;
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
}
