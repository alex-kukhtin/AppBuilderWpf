
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
	public Boolean System { get; set; }
	public Int32 Length { get; set; }
	public FieldType Type { get; set; }
	public String? RefTable { get; set; }

	public String SqlType(IdentifierType identType)
	{
		return Type switch
		{
			FieldType.String => $"nvarchar({Length})",
			FieldType.BigInt or FieldType.Money  or FieldType.Float or
			FieldType.Date or FieldType.DateTime
				=> Type.ToString().ToLowerInvariant(),
			FieldType.Guid => $"uniqueidentifier",
			FieldType.Boolean => $"bit",
			FieldType.Reference => identType.ToString().ToLowerInvariant(),
			FieldType.Identifier => $"{identType.ToString().ToLowerInvariant()} not null",
			_ => throw new NotImplementedException()
		};
	}
}
