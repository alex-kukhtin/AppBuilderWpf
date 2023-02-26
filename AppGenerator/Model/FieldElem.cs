// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

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
	public String? Default { get; init; }
	public Boolean Visible { get; init; }
	public Boolean Sort { get; init; }

	public Boolean IsId => Name == "Id";
}
