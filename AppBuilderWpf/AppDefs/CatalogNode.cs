
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
	public override IEnumerable<TableNode>? Children => _catalogs;

}
public class CatalogNode : TableNode
{
	[JsonIgnore]
	public override String Image => "/Images/Catalog.png";

	[JsonIgnore]
	public override String NameWithSchema => $"Catalog.{Name}";

	public void ApplyDefaults()
	{
		AddField("Id", FieldType.Identifier, FieldRole.PrimaryKey);
		AddField("Void", FieldType.Boolean, FieldRole.Void);
		AddStringField("Name", "@[Name]", 255);
		AddStringField("Memo", "@[Memo]", 255);
	}
}
