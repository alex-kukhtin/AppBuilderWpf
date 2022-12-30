// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.Logging;

using AppGenerator.Interfaces;

namespace AppGenerator;

public class ModelGenerator
{
	private readonly ILogger<ModelGenerator> _logger;
	private readonly IModelWriter _modelWriter;
	public ModelGenerator(IModelWriter modelWriter, ILogger<ModelGenerator> logger)
	{
		_logger = logger;
		_modelWriter = modelWriter;
	}
	public void Generate(TableDescriptor descr)
	{
		_logger.LogInformation("Generate model: {name}", descr.Table.Name);
		GenerateModelJson(descr);
		GenerateTypeScript(descr);
		GenerateXaml(descr);
	}

	public void GenerateModelJson(TableDescriptor descr)
	{
		var modelJson = GetResource($"AppGenerator.Resources.{descr.Schema}.model.json");
		ReplaceMainMacros(modelJson, descr);
		_modelWriter.WriteFile(modelJson.ToString(), descr.Path, "model.json");
	}

	private void GenerateTypeScript(TableDescriptor descr)
	{
		var indexTemplate = GetResource($"AppGenerator.Resources.{descr.Schema}.index.template.ts");
		ReplaceMainMacros(indexTemplate, descr);
		_modelWriter.WriteFile(indexTemplate.ToString(), descr.Path, "index.template.ts");

		var editTemplate = GetResource($"AppGenerator.Resources.{descr.Schema}.edit.template.ts");
		ReplaceMainMacros(editTemplate, descr);
		_modelWriter.WriteFile(editTemplate.ToString(), descr.Path, "edit.template.ts");
	}
	private void GenerateXaml(TableDescriptor descr)
	{
		var fileName = "index.view.xaml";
		var indexView = GetResource($"AppGenerator.Resources.{descr.Schema}.{fileName}");
		ReplaceMainMacros(indexView, descr);
		indexView.Replace("$(CollectionName)", descr.Table.Name!.Pluralize());
		indexView.Replace("$(EditUrl)", $"/{descr.Path.ToLowerInvariant()}/edit");
		_modelWriter.WriteFile(indexView.ToString(), descr.Path, fileName);

		fileName = "edit.dialog.xaml";
		var fileDialog = GetResource($"AppGenerator.Resources.{descr.Schema}.{fileName}");
		ReplaceMainMacros(fileDialog, descr);
		fileDialog.Replace("$(ElementName)", descr.Table.Name);
		fileDialog.Replace("$(ElementTitle)", descr.Table.Title ?? descr.Table.Name);
		_modelWriter.WriteFile(fileDialog.ToString(), descr.Path, fileName);
	}

	private void ReplaceMainMacros(StringBuilder sb, TableDescriptor descr)
	{
		sb.Replace("$(SchemaName)", descr.Schema);
		sb.Replace("$(ModelName)", descr.Table.Name);
	}

	private StringBuilder GetResource(String name)
	{
		var ass = Assembly.GetAssembly(typeof(ApplicationGenerator));
		var stream = ass?.GetManifestResourceStream(name);
		if (stream == null)
			throw new InvalidOperationException($"Resource not found: {name}");
		using var sr = new StreamReader(stream);
		return new StringBuilder(sr.ReadToEnd());	
	}
}
