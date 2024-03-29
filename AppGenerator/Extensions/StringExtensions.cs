﻿using System;

namespace AppGenerator;

public static class StringExtensions
{
	public static String Pluralize(this String? text)
	{
		if (text == null)
			return String.Empty;
		if (text.Length == 0)
			return text;
		if (text.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
			!text.EndsWith("ay", StringComparison.OrdinalIgnoreCase) &&
			!text.EndsWith("ey", StringComparison.OrdinalIgnoreCase) &&
			!text.EndsWith("iy", StringComparison.OrdinalIgnoreCase) &&
			!text.EndsWith("oy", StringComparison.OrdinalIgnoreCase) &&
			!text.EndsWith("uy", StringComparison.OrdinalIgnoreCase))
		{
			return text[..^1] + "ies";
		}
		else if (text.EndsWith("us", StringComparison.InvariantCultureIgnoreCase))
		{
			return text + "es";
		}
		else if (text.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
		{
			return text + "es";
		}
		else if (text.EndsWith("s", StringComparison.InvariantCultureIgnoreCase))
		{
			return text;
		}
		else if (text.EndsWith("x", StringComparison.InvariantCultureIgnoreCase) ||
			text.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase) ||
			text.EndsWith("sh", StringComparison.InvariantCultureIgnoreCase))
		{
			return text + "es";
		}
		else if (text.EndsWith("f", StringComparison.InvariantCultureIgnoreCase) && text.Length > 1)
		{
			return text[..^1] + "ves";
		}
		else if (text.EndsWith("fe", StringComparison.InvariantCultureIgnoreCase) && text.Length > 2)
		{
			return text[..^2] + "ves";
		}
		else
		{
			return text + "s";
		}
	}
	public static String SchemaName(this String? schema)
	{
		return schema switch
		{
			"Catalog" => "cat",
			"Document" => "doc",
			_ => throw new NotImplementedException($"Undefined schema for '{schema}'")
		};
	}
}
