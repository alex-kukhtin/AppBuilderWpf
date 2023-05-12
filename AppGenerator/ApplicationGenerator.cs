
using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

using AppGenerator.Interfaces;

namespace AppGenerator;

public class ApplicationGenerator
{
	private readonly ILogger<ApplicationGenerator> _logger;
	private readonly DirectoryStructureGenerator _dirGenerator;
	private readonly ModelGenerator _modelGenerator;
	private readonly ISqlGenerator _sqlGenerator;
	private readonly IModelWriter _modelWriter;

	public ApplicationGenerator(DirectoryStructureGenerator dirGenerator, 
		ModelGenerator modelGenerator, ISqlGenerator sqlGenerator,
		IModelWriter modelWriter, ILogger<ApplicationGenerator> logger) 
	{
		_logger = logger;
		_dirGenerator = dirGenerator;
		_modelGenerator = modelGenerator;
		_sqlGenerator = sqlGenerator;
		_modelWriter = modelWriter;
	}

	private String? _solutionFile;

	public void GenerateAppliction(String solutionFile)
	{
		_logger.LogInformation("SolutionFile: {solutionFile}", solutionFile);
		_solutionFile = solutionFile;
		var json = File.ReadAllText(_solutionFile);
		var appElem = JsonConvert.DeserializeObject<AppElem>(json, JsonHelpers.DefaultSettings) 
			?? throw new InvalidOperationException("Invalid appliction element");
		var list = _dirGenerator.Generate(appElem);
		var sqlJsonElem = new SqlJson()
		{
			Version = "1.0.0",
			Schema = "@schemas/sql-json-schema.json#",
			OutputFile = "dst/application.sql"
		};
		sqlJsonElem.InputFiles.Add("_sql/_struct.sql");
		sqlJsonElem.InputFiles.Add("_sql/_ui.sql");
		_modelGenerator.Start(appElem);
		_sqlGenerator.Start(appElem);
		foreach ( var item in list)
		{
			if (item.HasEndpoint)
			{
				_modelGenerator.Generate(item);
				var fileName = _sqlGenerator.GenerateEndpoint(item);
				_sqlGenerator.GenerateUi(item);
				sqlJsonElem.InputFiles.Add(fileName);
			}
			_sqlGenerator.GenerateStruct(item);
		}
		_sqlGenerator.Finish();
		var sqlJsonArray = new List<SqlJson>()
		{
			sqlJsonElem
		};
		_modelWriter.WriteFile(JsonConvert.SerializeObject(sqlJsonArray, JsonHelpers.DefaultSettings), "", "sql.json");
	}
}