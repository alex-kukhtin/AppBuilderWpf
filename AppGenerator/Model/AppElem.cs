// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace AppGenerator;

public enum IdentifierType
{
	BigInt,
	Integer,
	Guid
}

public class AppElem : BaseElem
{
	public Boolean MultiTenant { get; init; }
	public IdentifierType IdentifierType { get; init; }
	public List<CatalogElem> Catalogs { get; init; } = new();
	public List<DocumentElem> Documents { get; init; } = new();

	public TableDescriptor FindTableByReference(String? refTable)
	{
		if (String.IsNullOrEmpty(refTable))
			throw new InvalidOperationException($"Invalid empty reference");
		var name = refTable.Split('.');
		if (name.Length != 2)
			throw new InvalidOperationException($"Invalid reference '{refTable}'");
		TableElem tableDef = name[0] switch
		{
			"Catalog" => Catalogs.First(c => c.Name == name[1]),
			"Document" => Documents.First(d => d.Name == name[1]),
			_ => throw new InvalidOperationException($"Invalid schema '{name[0]}'")
		};
		return new TableDescriptor(String.Empty, name[0], tableDef);
	}
}
