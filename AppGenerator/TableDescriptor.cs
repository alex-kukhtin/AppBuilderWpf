// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;

namespace AppGenerator;

public class TableDescriptor
{
	public String Path { get; init; }
	public TableElem Table { get; init; }
	public String Schema { get; init; }

	public Boolean Child { get; init; }
	public TableDescriptor(String path, String schema, TableElem table, Boolean child = false)
	{
		Path = path;
		Schema = schema;
		Table = table;
		Child = child;
	}

	public Boolean HasEndpoint => !Child;
}
