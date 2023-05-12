// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Linq;

using AppGenerator.Interfaces;

namespace AppGenerator;

public class XamlGenerator
{
	private readonly IModelWriter _modelWriter;
	private readonly AppElem _root;
	public XamlGenerator(IModelWriter modelWriter, AppElem root)
	{
		_modelWriter = modelWriter;
		_root = root;
	}

	public void Generate(TableDescriptor descr)
	{
		GenerateIndex(descr);

		if (descr.Schema == "Catalog")
		{
			GenerateEditCatalog(descr);
			GenerateBrowseCatalog(descr);
		}
		else if (descr.Schema == "Document")
		{
			var fileName = "edit.view.xaml";
			var fileView = AppHelpers.GetResource($"AppGenerator.Resources.{descr.Schema}.{fileName}");
			descr.ReplaceMainMacros(fileView);
			fileView.Replace("$(ElementName)", descr.Table.Name);
			fileView.Replace("$(ElementTitle)", descr.Table.Title ?? descr.Table.Name);
			_modelWriter.WriteFile(fileView.ToString(), descr.Path, fileName);
		}
	}

	private void GenerateIndex(TableDescriptor descr)
	{
		var fileName = "index.view.xaml";
		var indexView = AppHelpers.GetResource($"AppGenerator.Resources.{descr.Schema}.{fileName}");
		descr.ReplaceMainMacros(indexView);
		indexView.Replace("$(CollectionName)", descr.Table.Name!.Pluralize());
		indexView.Replace("$(ElementName)", descr.Table.Name);
		indexView.Replace("$(EditUrl)", $"/{descr.Path.ToLowerInvariant()}/edit");
		var columns = descr.Table.Ui?.List?.Fields.Select(u => GetDataGridColumn(descr.FindField(u.Field), descr))
			?? descr.Table.Fields.Where(f => f.IsName).Select(f => GetDataGridColumn(f, descr));
		indexView.Replace("$(Columns)", String.Join("\n", columns));
		_modelWriter.WriteFile(indexView.ToString(), descr.Path, fileName);
	}
	private void GenerateEditCatalog(TableDescriptor descr)
	{
		var fileName = "edit.dialog.xaml";
		var fileDialog = AppHelpers.GetResource($"AppGenerator.Resources.{descr.Schema}.{fileName}");
		descr.ReplaceMainMacros(fileDialog);
		fileDialog.Replace("$(ElementName)", descr.Table.Name);
		fileDialog.Replace("$(ElementTitle)", descr.Table.Title ?? descr.Table.Name);

		var controls = descr.Table.Ui?.Edit?.Fields.Select(u => GetEditControl(u, descr.FindField(u.Field), descr))
				?? Enumerable.Empty<String>();

		fileDialog.Replace("$(Controls)", String.Join("\n", controls));


		_modelWriter.WriteFile(fileDialog.ToString(), descr.Path, fileName);
	}

	private void GenerateBrowseCatalog(TableDescriptor descr)
	{
		var fileName = "browse.dialog.xaml";
		var browseDialog = AppHelpers.GetResource($"AppGenerator.Resources.{descr.Schema}.{fileName}");
		descr.ReplaceMainMacros(browseDialog);
		browseDialog.Replace("$(ElementName)", descr.Table.Name);
		browseDialog.Replace("$(ElementTitle)", descr.Table.Title ?? descr.Table.Name);

		browseDialog.Replace("$(CollectionName)", descr.Table.TableName);
		browseDialog.Replace("$(EditUrl)", $"/{descr.Path.ToLowerInvariant()}/edit");

		_modelWriter.WriteFile(browseDialog.ToString(), descr.Path, fileName);
	}

	String GetEditControl(UIField ui, FieldElem fieldElem, TableDescriptor descr)
	{
		var title = fieldElem.Title ?? fieldElem.Name;
		var value = $"{descr.Table.Name}.{fieldElem.Name}";
		var tabIndex = ui.TabIndex != 0 ? @$" TabIndex=""{ui.TabIndex}""" : String.Empty;
		var requred = ui.Required ? @" Required=""True""" : String.Empty;
		if (fieldElem.IsReference)
		{
			var refTable = _root.FindTableByReference(fieldElem.RefTable);
			return "\t\t" + $$"""<SelectorSimple Label="{{title}}" Value="{Bind {{value}}}" Url="/catalog/{{refTable.Table.Name.ToLowerInvariant()}}"{{tabIndex}}{{requred}}/>""";
		}
		else
		{
			var multiline = ui.Multiline ? @" Multiline=""True""" : String.Empty;
			return "\t\t" + $$"""<TextBox Label="{{title}}" Value="{Bind {{value}}}"{{tabIndex}}{{multiline}}{{requred}}/>""";
		}
	}
	String GetDataGridColumn(FieldElem fieldElem, TableDescriptor descr)
	{
		var title = fieldElem.Title ?? fieldElem.Name;
		var value = fieldElem.Name;
		var sort = String.Empty;
		var role = String.Empty;
		if (fieldElem.IsReference)
		{
			var refTable = _root.FindTableByReference(fieldElem.RefTable);
			var refName = refTable.NameField();
			value = $"{fieldElem.Name}.{refName}";
		}
		if (fieldElem.IsReference || !fieldElem.Sort)
			sort = @" Sort=""False""";
		if (fieldElem.PrimaryKey)
			role = @" Role=""Id""";
		String column = $$"""<DataGridColumn Header="{{title}}" Content="{Bind {{value}}}"{{sort}}{{role}}/>""";
		return "\t\t\t" + column;
	}
}
