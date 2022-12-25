using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public static class JsonHelpers
{
	public static JsonSerializerSettings DefaultSettings =>
		new JsonSerializerSettings()
		{
			ContractResolver = new DefaultContractResolver()
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			}
		};
}
