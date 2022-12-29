
using Microsoft.VisualBasic.FileIO;
using System;

namespace AppGenerator;

public enum FieldType
{
	String,
	Identifier,
	BigInt,
	Integer,
	Boolean,
	Date,
	DateTime,
	Float,
	Money,
	Guid,
	Reference,
}

public class FieldElem : BaseElem
{
	public Boolean System { get; init; }
	public Int32 Length { get; init; }
	public FieldType Type { get; init; }
	public String? RefTable { get; init; }
	public Boolean Required { get; init; }

	public String SqlType(IdentifierType identType)
	{
		return Type switch
		{
			FieldType.String => $"nvarchar({Length})",
			FieldType.Integer => "int",
			FieldType.BigInt or FieldType.Money  or FieldType.Float or
			FieldType.Date or FieldType.DateTime
				=> Type.ToString().ToLowerInvariant(),
			FieldType.Guid => "uniqueidentifier",
			FieldType.Boolean => $"bit",
			FieldType.Reference => identType.ToString().ToLowerInvariant(),
			FieldType.Identifier => $"{identType.ToString().ToLowerInvariant()} not null",
			_ => throw new NotImplementedException()
		};
	}
}
