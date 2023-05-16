// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;

namespace AppGenerator;

public class TableElem : BaseElem
{
	public List<FieldElem> Fields { get; init; } = new();

	public List<TableElem> Details { get; init; } = new();

	public FieldElem PrimaryKey => Fields.First(x => x.IsPrimaryKey);
	public String TableName => Name.Pluralize();
	public UiTableElem? Ui { get; init; }
}
