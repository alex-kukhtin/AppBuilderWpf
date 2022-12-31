// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

using AppGenerator;
using AppGenerator.Interfaces;
using Microsoft.Extensions.Logging;
using SqlGenerator.MsSqlServer;

namespace AppGeneratorTests;

internal class TestEngine
{
	private static IServiceProvider? _provider;

	public static IServiceProvider ServiceProvider()
	{
		if (_provider != null)
			return _provider;

		var collection = new ServiceCollection();

		ILoggerFactory loggerFactory = 
			LoggerFactory.Create(builder => builder.AddConsole());
		collection
			.AddLogging()
			.AddSingleton(sp => loggerFactory.CreateLogger<ISqlGenerator>())
			.AddSingleton<ISqlGenerator, MsSqlServerSqlGenerator>()
			.AddSingleton<IModelWriter, ModelWriterMock>();

		_provider = collection.BuildServiceProvider();

		var log = _provider.GetService<ILogger<ISqlGenerator>>();
		return _provider;
	}
}
