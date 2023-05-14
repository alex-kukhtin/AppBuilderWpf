// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Linq;
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

public enum FieldRole
{
	Ordinal,
	PrimaryKey,
	Void,
	Name,
	RowNumber,
	Parent
}

public class FieldNode : BaseNode
{
	[JsonIgnore]
	internal TableNode? _parent;

	[JsonProperty(Order = 1)]
	public Boolean System { get; set; }


	private Boolean _required;
	[JsonProperty(Order = 2)]
	public Boolean Required
	{
		get => _required; 
		set { 
			_required = value; 
			if (!_required)
				Default = String.Empty;
			OnPropertyChanged(); 
		}
	}

	private FieldType _type;
	[JsonProperty(Order = 4)]
	public FieldType Type
	{
		get {
			return Role switch
			{
				FieldRole.PrimaryKey => FieldType.Identifier,
				FieldRole.Void => FieldType.Boolean,
				FieldRole.RowNumber => FieldType.Integer,
				FieldRole.Parent => FieldType.Reference,
				FieldRole.Name => FieldType.String,
				_ => _type
			};
		}
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

	private Int32 _length;
	[JsonProperty(Order = 5)]
	public Int32 Length
	{
		get => _length; set { _length = value; OnPropertyChanged(); }
	}

	private String? _refTable;
	[JsonProperty(Order = 6)]
	public String? RefTable
	{
		get => _refTable;
		set { _refTable = value; OnPropertyChanged(); }
	}

	private String? _default;
	[JsonProperty(Order = 7)]
	public String? Default { get => _default; set { _default = value; OnPropertyChanged(); } }

	private FieldRole _role;
	[JsonProperty(Order = 9)]
	public FieldRole Role
	{
		get => _role; 
		set 
		{
			if (value != FieldRole.Ordinal && _parent != null)
			{
				foreach( var f in _parent.Fields.Where(f => f.Role == value))
					f.ClearRole();
			}
			_role = value; 
			OnPropertyChanged(); 
			OnPropertyChanged(nameof(Type));
		}
	}

	[JsonIgnore]
	public Boolean IsEnabled => Role == FieldRole.Ordinal;

	[JsonIgnore]
	public Visibility VisibleIsNotSystem => System ? Visibility.Hidden : Visibility.Visible;

	internal void SetParent(TableNode parent)
	{
		_parent = parent;
	}

	internal void ClearRole()
	{
		if (_role == FieldRole.Ordinal)
			return;
		_role = FieldRole.Ordinal;
		OnPropertyChanged(nameof(Role));
		OnPropertyChanged(nameof(Type));
	}
}
