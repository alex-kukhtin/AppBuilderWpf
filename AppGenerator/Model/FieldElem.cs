
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

internal class FieldElem : BaseElem
{
	public Boolean System { get; set; }
	public Int32 Length { get; set; }
	public FieldType Type { get; set; }
	public String? RefTable { get; set; }
}
