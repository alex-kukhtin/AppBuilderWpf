
using System;

using Newtonsoft.Json;

namespace AppBuilder;

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

public class FieldNode : BaseNode
{
	[JsonProperty(Order = 1)]
	public Boolean System { get; set; }

	[JsonProperty(Order = 2)]
	public Int32 Length { get; set; }

	[JsonProperty(Order = 3)]
	public FieldType Type { get; set; }

	[JsonProperty(Order = 3)]
	public String? RefTable { get; set; }
}
