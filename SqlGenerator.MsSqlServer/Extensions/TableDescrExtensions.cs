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

	public static String PrimaryKeyName(this TableDescriptor descr)
	{
		return descr.Table.Fields.FirstOrDefault(f => f.IsPrimaryKey)?.Name?.Escape()
			?? throw new InvalidOperationException($"There is no primary key in {descr.Table.Name}");
	}
	public static String NameFieldName(this TableDescriptor descr)
	{
		return descr.Table.Fields.FirstOrDefault(f => f.IsName)?.Name?.Escape()
			?? throw new InvalidOperationException($"There is no name field in {descr.Table.Name}");
	}

	public static String? VoidName(this TableDescriptor descr)
	{
		return descr.Table.Fields.FirstOrDefault(f => f.IsVoid)?.Name?.Escape();
	}

	public static Boolean HasMaps(this TableDescriptor descr)
	{
		return descr.Table.Fields.Any(f => f.IsReference);
	}
	public static Boolean HasSearch(this UIElemForm descr)
	{
		return descr.Fields.Any(f => f.Search != SearchType.None);
	}

	public static SortDescription RealSort(this UIElemForm descr)
	{
		return new SortDescription()
		{
			Dir = descr.InitialOrder?.Dir ?? SortDirection.Asc,
			OrderBy = (descr.InitialOrder?.OrderBy ?? String.Empty).ToLowerInvariant()
		};
	}

	internal static FieldElem FindField(this TableDescriptor descr, String? name)
	{
		if (String.IsNullOrEmpty(name))
			throw new ArgumentException("name is empty");
		return descr.Table.Fields.FirstOrDefault(f => f.Name == name)
			?? throw new InvalidOperationException("Name field not found");
	}
}
