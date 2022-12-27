
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace AppBuilder;

public class CatalogsNode : BaseNode
{
	private readonly ObservableCollection<CatalogNode>? _catalogs;
	public CatalogsNode(ObservableCollection<CatalogNode>? catalogs)
	{
		Name = "Catalogs";
		_catalogs = catalogs;
	}
	public override IEnumerable<BaseNode>? Children => _catalogs;

}
public class CatalogNode : TableNode
{
	[JsonIgnore]
	public override String Image => "/Images/Catalog.png";

	[JsonIgnore]
	public override String NameWithSchema => $"Catalog.{Name}";

	public void ApplyDefaults()
	{
		AddField(true, "Id", FieldType.Identifier);
		AddField(true, "Void", FieldType.Boolean);
		AddField(false, "Name", FieldType.String, "@[Name]", 255);
		AddField(false, "Memo", FieldType.String, "@[Memo]", 255);
	}
}
