
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace AppGenerator;

public class DirectoryStructureGenerator
{
	private readonly ILogger<DirectoryStructureGenerator> _logger;
	public DirectoryStructureGenerator(ILogger<DirectoryStructureGenerator> logger)
	{
		_logger = logger;
	}

	public IEnumerable<TableDescriptor> Generate(AppElem elem, String baseDirectory)
	{
		var tdlist = new List<TableDescriptor>();
		// generate catalogs
		foreach (var c in elem.Catalogs) {
			if (!String.IsNullOrEmpty(c.Name))
			{
				String path = Path.Combine(baseDirectory, "catalog", c.Name.ToLowerInvariant());
				var td = new TableDescriptor(path, Constants.Schemas.Catalog,  c);
				tdlist.Add(td);
				CreateDirectoryIfNeeded(path);
			}
			else
				_logger.LogError("Catalog name is empty");
		}
		// generate elements
		foreach (var d in elem.Documents)
		{
			if (!String.IsNullOrEmpty(d.Name)) {

				String path = Path.Combine(baseDirectory, "document", d.Name.ToLowerInvariant());
				var td = new TableDescriptor(path, Constants.Schemas.Document, d);
				tdlist.Add(td);
				CreateDirectoryIfNeeded(path);
			}
			else
				_logger.LogError("Document name is empty");
		}
		return tdlist;
	}

	private void CreateDirectoryIfNeeded(String dir) 
	{
		if (!Directory.Exists(dir))
		{
			_logger.LogInformation("Create directory {dir}", dir);
			Directory.CreateDirectory(dir);
		}
	}
}
