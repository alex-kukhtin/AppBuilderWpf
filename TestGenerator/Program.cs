using AppGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
				.AddSingleton<ModelGenerator>();
			});
}

