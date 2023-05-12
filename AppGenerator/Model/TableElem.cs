// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;

namespace AppGenerator;

public enum SortDirection
{
	Asc,
	Desc
}
public record SortDescription
{
	public String? Field { get; init; }
	public SortDirection Dir { get; init; }
	public String Direction => Dir.ToString().ToLowerInvariant();
}

public class TableElem : BaseElem
{
	public List<FieldElem> Fields { get; init; } = new();

	public List<TableElem> Details { get; init; } = new();

	public FieldElem PrimaryKey => Fields.First(x => x.PrimaryKey);
	public String TableName => Name.Pluralize();

	public SortDescription? InitialSort { get; init; }

	public UIElem? Ui { get; init; }
}
