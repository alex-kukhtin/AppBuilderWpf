using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.MsSqlServer.Extensions
{
	public static class NameExtensions
	{
		private readonly static HashSet<String> _keywords = new(StringComparer.OrdinalIgnoreCase)
		{
			"Name", "Date", "No"
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
}
