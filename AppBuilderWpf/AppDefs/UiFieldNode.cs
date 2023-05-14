// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;

using Newtonsoft.Json;

namespace AppBuilder;

public enum SearchType
{
	None,
	Exact,
	Like
}

public class UiFieldNode : BaseNode
{
	private String? _field;
	[JsonProperty(Order = 1)]
	public String? Field
	{
		get => _field;
		set { _field = value; OnPropertyChanged(); }
	}

	private Boolean _sort;
	[JsonProperty(Order = 2)]
	public Boolean Sort { 
		get => _sort;
		set { _sort = value; OnPropertyChanged(); }
	}

	private SearchType _search;
	[JsonProperty(Order = 3)]
	public SearchType Search
	{
		get => _search;
		set { _search = value; OnPropertyChanged(); }
	}

	private Boolean _required;
	[JsonProperty(Order = 4)]
	public Boolean Required
	{
		get => _required; set { _required = value; OnPropertyChanged(); }
	}

	private Boolean _multiline;
	[JsonProperty(Order = 5)]
	public Boolean Multiline
	{
		get => _multiline; set { _multiline = value; OnPropertyChanged(); }
	}

	private Int32? _tabIndex;
	[JsonProperty(Order = 5)]
	public Int32? TabIndex
	{
		get => _tabIndex; set { _tabIndex = value; OnPropertyChanged(); }
	}
}
