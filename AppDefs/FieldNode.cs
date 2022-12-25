
using System;
using Newtonsoft.Json;

namespace AppBuilder;

public class FieldNode : BaseNode
{
	[JsonProperty(Order = 2)]
	public Int32 Length { get; set; }
}
