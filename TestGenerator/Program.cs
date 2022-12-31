using AppGenerator;
using AppGenerator.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlGenerator.MsSqlServer;
using System;
using System.IO;
using System.Reflection;

namespace TestGenerator;

internal class Program
{
	static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			Console.WriteLine("Usage: TestGenerator.exe <soultionfile>");
			return;
		}

		var host = CreateHostBuilder(args).Build();
		host.Services.GetRequiredService<ApplicationGenerator>().GenerateAppliction(args[0]);
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureServices((hostContext, services) =>
			{
				services.AddLogging()
				.AddSingleton<ApplicationGenerator>()
				.AddSingleton<DirectoryStructureGenerator>()
				.AddSingleton<ModelGenerator>()
				.AddSingleton<ISqlGenerator, MsSqlServerSqlGenerator>()
				.AddSingleton<IModelWriter>(x => new ModelWriter(Path.GetDirectoryName(args[0])!));
			});
}

