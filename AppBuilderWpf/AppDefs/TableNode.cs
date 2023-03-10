using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

	public FieldNode AddField(Boolean system, String name, FieldType type, String? Title = null, Int32 length = 0)
	{
		var f = new FieldNode() { System = system, Name = name, Type = type, Title = Title, Length = length };
		if (system)
			f.Required = true;
		Fields.Add(f);
		return f;
	}

	private FieldNode? _selectedField;
	[JsonIgnore]
	public FieldNode? SelectedField
	{
		get { return _selectedField; }
		set
		{
			_selectedField = value;
			OnPropertyChanged();
		}
	}

	[JsonIgnore]
	public override IEnumerable<BaseNode>? Children => Details;

	public void AddDetails()
	{
		var t = new TableNode() { Name = $"Details{Details.Count + 1}" };
		var parent = t.AddField(true, Name!, FieldType.Reference);
		parent.RefTable = NameWithSchema;
		parent.Required = true;
		parent.Parent = true;
		Details.Add(t);
		t.IsSelected = true;
	}

	public override void OnNameChanged()
	{
		foreach (var d in Details)
		{
			foreach (var f in d.Fields.Where(f => f.Parent))
			{
				f.RefTable = NameWithSchema;
				f.Name = Name;
			}
		}
	}
}
