// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.


using System;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace AppBuilder;

public enum Direction
{
	Asc,
	Desc
}

public enum UiView
{
	None,
	Dialog,
	Page
}

public class UiInitialOrder : ObservableNode
{
	private String? _field;
	[JsonProperty(Order = 3)]
	public String? Field
	{
		get => _field;
		set { _field = value; OnPropertyChanged(); }
	}

	private Direction? _dir;
	[JsonProperty(Order = 4)]
	public Direction? Dir
	{
		get => _dir;
		set { _dir = value; OnPropertyChanged(); }
	}
}

public class UiElementNode : ObservableNode
{
	private UiView _view;
	[JsonProperty(Order = 2)]
	public UiView View { get => _view; set { _view = value; OnPropertyChanged(); } }

	UiInitialOrder? _initialOrder;
	[JsonProperty(Order = 3)]
	public UiInitialOrder? InitialOrder { get => _initialOrder; set { _initialOrder = value; OnPropertyChanged(); } }

	[JsonProperty(Order = 4)]
	public ObservableCollection<UiFieldNode> Fields { get; set; } = new();
}

public class UiNode : ObservableNode
{
	[JsonIgnore]
	internal TableNode? _parent;

	UiElementNode? _index;
	[JsonProperty(Order = 5)]
	public UiElementNode? Index { get => _index; set { _index = value; OnPropertyChanged(); } }

	UiElementNode? _browseItem;
	[JsonProperty(Order = 6)]
	public UiElementNode? BrowseItem { get => _browseItem; set { _browseItem = value; OnPropertyChanged(); } }

	UiElementNode? _editItem;
	[JsonProperty(Order = 7)]
	public UiElementNode? EditItem { get => _editItem; set { _editItem = value; OnPropertyChanged(); } }

	UiElementNode? _editFolder;
	[JsonProperty(Order = 8)]
	public UiElementNode? EditFolder { get => _editFolder; set { _editFolder = value; OnPropertyChanged(); } }

	[JsonIgnore] 
	public Boolean IsEmpty => _index == null && _editItem == null && _editFolder == null && _browseItem == null;

	internal void SetParent(TableNode parent)
	{
		_parent = parent;	
	}
}
