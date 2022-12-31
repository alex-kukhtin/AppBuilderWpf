// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System.Linq;
using System.Collections.Generic;

namespace AppGenerator;

public class TableElem : BaseElem
{
	public List<FieldElem> Fields { get; init; } = new();

	public List<TableElem> Details { get; init; } = new();

	public FieldElem PrimaryKey => Fields.First(x => x.Name == "Id");
	public string TableName => Name.Pluralize();
}
