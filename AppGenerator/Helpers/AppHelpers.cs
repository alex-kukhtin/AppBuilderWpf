// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace AppGenerator;

internal static class AppHelpers
{
	internal static StringBuilder GetResource(String name)
	{
		var ass = Assembly.GetAssembly(typeof(ApplicationGenerator));
		var stream = ass?.GetManifestResourceStream(name)
			?? throw new InvalidOperationException($"Resource not found: {name}");
		using var sr = new StreamReader(stream);
		return new StringBuilder(sr.ReadToEnd());
	}
}
