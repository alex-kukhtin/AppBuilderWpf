// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

namespace SqlGenerator.MsSqlServer;

public static class NameExtensions
{
	private readonly static HashSet<String> _keywords = new(StringComparer.OrdinalIgnoreCase)
	{
		"Name", "Date", "No", "Contract", "Sum", "To", "From", "Key", "Plan", "End", "Start",
		"State", "Map", "Match", "New", "Time", "Data", "Value", "First"
	};

	public static String Escape(this String? name)
	{
		if (name == null)
			return String.Empty;
		if (_keywords.Contains(name))
			return $"[{name}]";
		return name;
	}

	public static String SchemaName(this String? schema) {
		return schema switch
		{
			"Catalog" => "cat",
			"Document" => "doc",
			_ => throw new NotImplementedException($"Undefined schema for '{schema}'")
		};
	}
}
