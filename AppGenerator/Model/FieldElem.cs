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

public enum FieldRole
{
	Ordinal,
	PrimaryKey,
	Void,
	Name,
	RowNumber,
	Parent
}

public class FieldElem : BaseElem
{
	public Int32 Length { get; init; }
	public FieldType Type { get; init; }
	public String? RefTable { get; init; }
	public Boolean Required { get; init; }
	public String? Default { get; init; }
	public FieldRole Role { get; init; }

	public Boolean IsReference => Type == FieldType.Reference;
	public Boolean IsNotNull => Required || IsPrimaryKey || IsVoid || IsParent;
	public Boolean IsName => Role == FieldRole.Name;
	public Boolean IsPrimaryKey => Role == FieldRole.PrimaryKey;
	public Boolean IsVoid => Role == FieldRole.Void;
	public Boolean IsParent => Role == FieldRole.Parent;
	public Boolean IsRowNumber => Role == FieldRole.RowNumber;
}
