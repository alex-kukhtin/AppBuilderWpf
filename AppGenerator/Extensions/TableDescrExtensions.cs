// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Linq;
using System.Text;

namespace AppGenerator;

internal static class TableDescrExtensions
{
	internal static StringBuilder ReplaceMainMacros(this TableDescriptor descr, StringBuilder sb)
	{
		sb.Replace("$(SchemaName)", descr.Schema.SchemaName());
		sb.Replace("$(Schema)", descr.Schema);
		sb.Replace("$(ModelName)", descr.Table.Name);
		return sb;
	}
	internal static String NameField(this TableDescriptor descr)
	{
		return descr.Table.Fields.FirstOrDefault(f => f.IsName)?.Name
			?? throw new InvalidOperationException("Name field not found");
	}

	internal static FieldElem FindField(this TableDescriptor descr, String? name)
	{
		if (String.IsNullOrEmpty(name))
			throw new ArgumentException("name is empty");
		return descr.Table.Fields.FirstOrDefault(f => f.Name == name)
			?? throw new InvalidOperationException("Name field not found");
	}
}
