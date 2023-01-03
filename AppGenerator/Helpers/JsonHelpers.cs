// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace AppGenerator;

internal static class JsonHelpers
{
	public static JsonSerializerSettings DefaultSettings = new()
	{
		Formatting = Formatting.Indented,
		ContractResolver = new DefaultContractResolver()
		{
			NamingStrategy = new CamelCaseNamingStrategy()
		}
	};
}
