
using Newtonsoft.Json;
using System;
using System.IO;

using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace AppGenerator;

public class ApplicationGenerator
{
	private readonly ILogger<ApplicationGenerator> _logger;
	private readonly DirectoryStructureGenerator _dirGenerator;
	private readonly ModelGenerator _modelGenerator;
	public ApplicationGenerator(DirectoryStructureGenerator dirGenerator, 
		ModelGenerator modelGenerator,
		ILogger<ApplicationGenerator> logger) 
	{
		_logger = logger;
		_dirGenerator = dirGenerator;
		_modelGenerator = modelGenerator;
	}

	private String? _solutionFile;

	public void GenerateAppliction(String solutionFile)
	{
		_logger.LogInformation("SolutionFile: {solutionFile}", solutionFile);
		_solutionFile = solutionFile;
		var json = File.ReadAllText(_solutionFile);
		var settings = new JsonSerializerSettings()
		{
			ContractResolver = new DefaultContractResolver()
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			}
		};
		var appElem = JsonConvert.DeserializeObject<AppElem>(json, settings);
		if (appElem == null)
			throw new InvalidOperationException("Invalid appliction element");

		var list = _dirGenerator.Generate(appElem);
		foreach ( var item in list )
		{
			_modelGenerator.Generate(item);
		}
	}
}