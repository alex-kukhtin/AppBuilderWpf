
using System.Collections.Generic;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppBuilder;

public static class JsonHelpers
{
	public static JsonSerializerSettings DefaultSettings =>
		new JsonSerializerSettings()
		{
			ContractResolver = new DefaultContractResolver()
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			},
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			Converters = new List<JsonConverter>() { 
				new StringEnumConverter()
			}
		};
}
