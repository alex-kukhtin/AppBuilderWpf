using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGenerator;

public class ModelGenerator
{
	private readonly ILogger<ModelGenerator> _logger;
	public ModelGenerator(ILogger<ModelGenerator> logger)
	{
		_logger = logger;
	}
	public void Generate(TableDescriptor descr)
	{
		_logger.LogInformation("GenerateModel: {name}", descr.Table.Name);
		// model.json

	}
}
