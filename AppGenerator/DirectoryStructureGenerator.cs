// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using AppGenerator.Interfaces;
using Microsoft.Extensions.Logging;

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
		var tdlist = new List<TableDescriptor>();
		// generate catalogs
		foreach (var c in elem.Catalogs) {
			if (!String.IsNullOrEmpty(c.Name))
			{
				String path = Path.Combine("catalog", c.Name.ToLowerInvariant()).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
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

				String path = Path.Combine("document", d.Name.ToLowerInvariant());
				var td = new TableDescriptor(path, Constants.Schemas.Document, d);
				tdlist.Add(td);
				_modelWriter.CreateDirectory(path);
			}
			else
				_logger.LogError("Document name is empty");
		}
		return tdlist;
	}
}
