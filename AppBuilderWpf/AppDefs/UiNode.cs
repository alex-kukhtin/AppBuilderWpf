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

public enum EditIn
{
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
	private EditIn? _editIn;
	[JsonProperty(Order = 2)]
	public EditIn? EditIn { get => _editIn; set { _editIn = value; OnPropertyChanged(); } }

	UiInitialOrder? _initialOrder;
	[JsonProperty(Order = 3)]
	public UiInitialOrder? InitialOrder { get => _initialOrder; set { _initialOrder = value; OnPropertyChanged(); } }

	[JsonProperty(Order = 4)]
	public ObservableCollection<UiFieldNode> Fields { get; set; } = new();
}

public class UiNode : ObservableNode
{
	UiElementNode? _index;
	[JsonProperty(Order = 5)]
	public UiElementNode? Index { get => _index; set { _index = value; OnPropertyChanged(); } }

	UiElementNode? _editItem;
	[JsonProperty(Order = 6)]
	public UiElementNode? EditItem { get => _editItem; set { _editItem = value; OnPropertyChanged(); } }

	UiElementNode? _editFolder;
	[JsonProperty(Order = 6)]
	public UiElementNode? EditFolder { get => _editFolder; set { _editFolder = value; OnPropertyChanged(); } }

	[JsonIgnore] public Boolean IsEmpty => _index == null && _editItem == null && _editFolder == null;

	internal void SetParent(TableNode parent)
	{

	}
}
