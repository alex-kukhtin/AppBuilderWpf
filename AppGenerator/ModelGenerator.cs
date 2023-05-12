// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using Microsoft.Extensions.Logging;

using AppGenerator.Interfaces;

namespace AppGenerator;

public class ModelGenerator
{
	private readonly ILogger<ModelGenerator> _logger;
	private readonly IModelWriter _modelWriter;
	private XamlGenerator? _xamlGenerator;
	public ModelGenerator(IModelWriter modelWriter, ILogger<ModelGenerator> logger)
	{
		_logger = logger;
		_modelWriter = modelWriter;
	}

	public void Start(AppElem elem)
	{
		_xamlGenerator = new XamlGenerator(_modelWriter, elem);
	}
	public void Generate(TableDescriptor descr)
	{
		_logger.LogInformation("Generate model: {name}", descr.Table.Name);
		GenerateModelJson(descr);
		GenerateTypeScript(descr);
		_xamlGenerator?.Generate(descr);
	}

	public void GenerateModelJson(TableDescriptor descr)
	{
		var modelJson = AppHelpers.GetResource($"AppGenerator.Resources.{descr.Schema}.model.json");
		descr.ReplaceMainMacros(modelJson);
		_modelWriter.WriteFile(modelJson.ToString(), descr.Path, "model.json");
	}

	private void GenerateTypeScript(TableDescriptor descr)
	{
		var indexTemplate = AppHelpers.GetResource($"AppGenerator.Resources.{descr.Schema}.index.template.ts");
		descr.ReplaceMainMacros(indexTemplate);
		_modelWriter.WriteFile(indexTemplate.ToString(), descr.Path, "index.template.ts");

		var editTemplate = AppHelpers.GetResource($"AppGenerator.Resources.{descr.Schema}.edit.template.ts");
		descr.ReplaceMainMacros(editTemplate);
		_modelWriter.WriteFile(editTemplate.ToString(), descr.Path, "edit.template.ts");
	}
}
