
using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace AppGenerator;

internal class SqlJson
{
	[JsonProperty("$schema")]
	public String? Schema {get ; set; }
	public String? Version { get; set; }
	public String? OutputFile { get; set; }
	public List<String> InputFiles { get; set; } = new();
}
