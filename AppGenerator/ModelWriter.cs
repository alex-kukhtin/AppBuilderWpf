using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AppGenerator;

public class ModelWriter
{
	private readonly String _solutionPath;
	public ModelWriter(String solutionPath)
	{
		_solutionPath = solutionPath;
	}

	public void WriteFile(String content, String path, String fileName)
	{
		var writeFileName = Path.Combine(_solutionPath, path, fileName);
		File.WriteAllText(writeFileName, content, Encoding.UTF8);
	}

	public void CreateDirectory(String path)
	{
		var dirPath = Path.Combine(_solutionPath, path);
		if (Directory.Exists(dirPath))
			return;
		Directory.CreateDirectory(dirPath);
	}
}
