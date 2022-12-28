
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace AppGenerator;

public class ApplicationGenerator
{
	private readonly String _solutionFile;
	public ApplicationGenerator(String solutionFile) 
	{ 
		_solutionFile = solutionFile;
	}

	public Boolean GenerateAppliction()
	{
		var json = File.ReadAllText(_solutionFile);
		var settings = new JsonSerializerSettings()
		{
			ContractResolver = new DefaultContractResolver()
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			}
		};
		var appElem = JsonConvert.DeserializeObject<AppElem>(json, settings);
		return true;
	}
}