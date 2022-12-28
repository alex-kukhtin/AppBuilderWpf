using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace AppBuilder;

public class TablesNode : BaseNode
{
	private readonly ObservableCollection<TableNode>? _tables; 
	public TablesNode(ObservableCollection<TableNode>? tables)
	{
		Name = "Tables";
		_tables = tables;
	}
	public override IEnumerable<BaseNode>? Children => _tables;
}

public class TableNode : BaseNode
{
	[JsonProperty(Order = 2)]
	public ObservableCollection<FieldNode> Fields { get; set; } = new();

	[JsonProperty(Order = 3)]
	public ObservableCollection<TableNode> Details { get; set; } = new();

	public Boolean ShouldSerializeDetails() => Details != null && Details.Count > 0;

	[JsonIgnore]
	public override String Image => "/Images/table.png";

	[JsonIgnore]
	public virtual String NameWithSchema => throw new NotImplementedException();

	public void CreateField()
	{
		var f = new FieldNode() { Name = $"Field{Fields.Count + 1}", Type = FieldType.String, Length = 50 };
		Fields.Add(f);
	}

	public void AddField(Boolean system, String name, FieldType type, String? Title = null, Int32 length = 0)
	{
		var f = new FieldNode() { System = system, Name = name, Type = type, Title = Title, Length = length };
		Fields.Add(f);
	}

	[JsonIgnore]
	public override IEnumerable<BaseNode>? Children => Details;

	public void AddDetails()
	{
		var t = new TableNode() { Name = $"Details{Details.Count + 1}" };
		Details.Add(t);
		t.IsSelected = true;
	}
}
