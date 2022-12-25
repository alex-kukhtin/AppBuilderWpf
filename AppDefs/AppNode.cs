
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace AppBuilder;

public class AppNode : BaseNode
{
	[JsonProperty(Order = 1)]
	public Boolean MultiTenant { get; set; }

	[JsonProperty(Order = 2)]
	public String DefaultSchema { get; set; } = String.Empty;

	[JsonProperty(Order = 3)]
	public ObservableCollection<FieldNode> DefaultFields { get; init; } = new();

	[JsonProperty(Order = 10)]
	public ObservableCollection<TableNode> Tables { get; init; } = new();
	[JsonProperty(Order = 11)]
	public ObservableCollection<CatalogNode> Catalogs { get; init; } = new();
	[JsonProperty(Order = 12)]
	public ObservableCollection<DocumentNode> Documents { get; init; } = new();

	[JsonIgnore]
	public override IEnumerable<BaseNode>? Children
	{
		get
		{
			yield return new TablesNode(Tables);
			yield return new CatalogsNode(Catalogs);
			yield return new DocumentsNode(Documents);
		}
	}
}

