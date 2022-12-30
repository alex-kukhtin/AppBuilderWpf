// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;

namespace AppGenerator;

public class TableDescriptor
{
	public String Path { get; init; }
	public TableElem Table { get; init; }
	public String Schema { get; init; }
	public TableDescriptor(String path, String schema, TableElem table)
	{
		Path = path;
		Schema = schema;
		Table = table;
	}
}
