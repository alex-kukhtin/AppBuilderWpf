// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using AppGenerator.Interfaces;

namespace AppGenerator;

public class DirectoryStructureGenerator
{
	private readonly ILogger<DirectoryStructureGenerator> _logger;
	private readonly IModelWriter _modelWriter;
	public DirectoryStructureGenerator(IModelWriter modelWriter, ILogger<DirectoryStructureGenerator> logger)
	{
		_logger = logger;
		_modelWriter = modelWriter;
	}

	public IEnumerable<TableDescriptor> Generate(AppElem elem)
	{
		_modelWriter.CreateDirectory("_sql");
		var tdlist = new List<TableDescriptor>();
		// generate catalogs
		foreach (var c in elem.Catalogs) {
			if (!String.IsNullOrEmpty(c.Name))
			{
				String path = $"catalog/{c.Name.ToLowerInvariant()}";
				var td = new TableDescriptor(path, Constants.Schemas.Catalog,  c);
				tdlist.Add(td);
				_modelWriter.CreateDirectory(path);
			}
			else
				_logger.LogError("Catalog name is empty");
		}
		// generate elements
		foreach (var d in elem.Documents)
		{
			if (!String.IsNullOrEmpty(d.Name)) {

				String path = $"document/{d.Name.ToLowerInvariant()}";
				var td = new TableDescriptor(path, Constants.Schemas.Document, d);
				tdlist.Add(td);
				foreach (var dd in d.Details)
					tdlist.Add(new TableDescriptor(String.Empty, Constants.Schemas.Document, dd, true));
				_modelWriter.CreateDirectory(path);
			}
			else
				_logger.LogError("Document name is empty");
		}
		return tdlist;
	}
}
