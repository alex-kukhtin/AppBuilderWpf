// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

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
	public Int32 Length { get; init; }
	public FieldType Type { get; init; }
	public String? RefTable { get; init; }
	public Boolean Required { get; init; }
	public String? Default { get; init; }
	public Boolean Sort { get; init; }
	public Boolean Search { get; init; }
	public Boolean Parent { get; init; }
	public Boolean IsName { get; init; }
	public Boolean IsVoid { get; init; }
	public Boolean PrimaryKey { get; init; }
	public Boolean RowNumber { get; init; }
	public Boolean IsReference => Type == FieldType.Reference;
	public Boolean IsNotNull => Required || PrimaryKey || IsVoid;
}
