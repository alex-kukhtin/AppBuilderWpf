// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Text;

using AppGenerator.Interfaces;

namespace AppGenerator;

public class ModelWriter : IModelWriter
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
