// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using AppGenerator;

namespace SqlGenerator.MsSqlServer;

internal static class TableExtensions
{
	public static String TableTypeName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.Name}.TableType]";
	}
	public static String MapTableTypeName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.Name}.Map.TableType]";
	}
	public static String MetadataProcName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.Name}.Metadata]";
	}
	public static String LoadProcName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.Name}.Load]";
	}
	public static String IndexProcName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.Name}.Index]";
	}
	public static String UpdateProcName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.Name}.Update]";
	}
	public static String MapProcName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.Name}.Map]";
	}
	public static String SqlTableName(this TableDescriptor descr)
	{
		return $"{descr.Schema.SchemaName()}.[{descr.Table.TableName}]";
	}
	public static String SqlTableAlias(this TableDescriptor descr, IEnumerable<String> used)
	{
		String a = descr.Table.Name.ToLowerInvariant()[0..1];
		Int32 index = 1;
		while (used.Contains(a))
		{
			a += index.ToString();
			index++;
		}
		return a;
	}

	public static Boolean HasMaps(this TableDescriptor descr)
	{
		return descr.Table.Fields.Any(f => f.IsReference);
	}

	public static SortDescription RealSort(this TableDescriptor descr)
	{
		var table = descr.Table;
		var sd = new SortDescription()
		{
			Dir = table.InitialSort?.Dir ?? SortDirection.Asc,
			Field = (table.InitialSort?.Field
				?? table.Fields.FirstOrDefault(x => x.IsName)?.Name ?? String.Empty).ToLowerInvariant()
		};
		return sd;
	}
}
