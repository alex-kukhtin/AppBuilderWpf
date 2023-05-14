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

	private String? _nameInParent;
	[JsonProperty(Order = 3)]
	public String? NameInParent
	{
		get => _nameInParent;
		set { _nameInParent = value; OnPropertyChanged(); }
	}

	[JsonProperty(Order = 4)]
	public ObservableCollection<TableNode> Details { get; set; } = new();
	public Boolean ShouldSerializeDetails() => Details != null && Details.Count > 0;

	[JsonProperty(Order = 5)]
	public UiNode Ui { get; set; } = new();

	public Boolean ShouldSerializeUi() => Ui != null && !Ui.IsEmpty;

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

		var id = t.AddField(true, "Id", FieldType.Identifier);
		id.Role = FieldRole.PrimaryKey;

		var parent = t.AddField(true, Name!, FieldType.Reference);
		parent.RefTable = NameWithSchema;
		parent.Required = true;

		var rowNo = t.AddField(true, "RowNumber", FieldType.Integer);
		rowNo.Required = true;
		rowNo.Role = FieldRole.RowNumber;
		rowNo.Default = "1";

		Details.Add(t);
		t.IsSelected = true;
	}

	public override void OnNameChanged()
	{
		foreach (var d in Details)
		{
			foreach (var f in d.Fields.Where(f => f.Role == FieldRole.Parent))
			{
				f.RefTable = NameWithSchema;
				f.Name = Name;
			}
		}
	}
	internal void SetParent(AppNode parent)
	{

	}

	internal override void OnInit()
	{
		base.OnInit();
		foreach (var f in Fields)
		{
			f.SetParent(this);
			f.OnInit();
		}
		Ui?.SetParent(this);
	}
}
