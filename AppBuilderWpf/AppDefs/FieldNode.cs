
using System;
using System.Windows;
using Newtonsoft.Json;

namespace AppBuilder;

public enum FieldType
{
	String,
	Identifier,
	BigInt,
	Integer,
	Boolean,
	Date,
	DateTime,
	Float,
	Money,
	Guid,
	Reference,
}

public class FieldNode : BaseNode
{
	[JsonProperty(Order = 1)]
	public Boolean System { get; set; }

	private Int32 _length;
	[JsonProperty(Order = 2)]
	public Int32 Length { 
		get => _length; set { _length = value; OnPropertyChanged(); } 
	}

	private FieldType _type;
	[JsonProperty(Order = 3)]
	public FieldType Type
	{
		get => _type;
		set
		{
			_type = value;
			if (_type != FieldType.Reference)
				RefTable = null;
			if (Type == FieldType.String)
				Length = 50;
			else
				Length = 0;
			OnPropertyChanged();
		}
	}

	private String? _refTable;
	[JsonProperty(Order = 3)]
	public String? RefTable
	{
		get => _refTable;
		set { _refTable = value; OnPropertyChanged(); }
	}

	[JsonProperty(Order = 4)]
	public Boolean Required { get; set; }

	[JsonIgnore]
	public Boolean IsEnabled => System ? false : true;
	[JsonIgnore]
	public Visibility VisibleIsNotSystem => System ? Visibility.Hidden : Visibility.Visible;
}
