﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace AppBuilder;

public class AppNode : BaseNode
{
	[JsonProperty(Order = 1)]
	public Boolean MultiTenant { get; set; }

	[JsonProperty(Order = 2)]
	public String DefaultSchema { get; set; } = String.Empty;

	[JsonProperty(Order = 11)]
	public ObservableCollection<CatalogNode> Catalogs { get; init; } = new();
	[JsonProperty(Order = 12)]
	public ObservableCollection<DocumentNode> Documents { get; init; } = new();

	[JsonIgnore]
	public override String Image => "/Images/AppNode.png";

	[JsonIgnore]
	public override IEnumerable<BaseNode>? Children
	{
		get
		{
			yield return new CatalogsNode(Catalogs);
			yield return new DocumentsNode(Documents);
		}
	}


	public void AddCatalog() {
		var c = new CatalogNode() { Name = $"Catalog{Catalogs.Count + 1}" };
		c.ApplyDefaults();
		Catalogs.Add(c);
		c.IsSelected = true;
	}
	public void AddDocument()
	{
		var d = new DocumentNode() { Name =  $"Document{Documents.Count + 1}" };
		d.ApplyDefaults();
		Documents.Add(d);
		d.IsSelected = true;
	}
}

