using AppGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGeneratorTests;
internal class ModelWriterMock : IModelWriter
{
	private readonly Dictionary<String, String> _data = new();
	public void CreateDirectory(String path)
	{
		throw new NotImplementedException();
	}

	public void WriteFile(String content, String path, String fileName)
	{
		_data[$"{path}/{fileName}"] = content;
	}
}
