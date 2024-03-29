﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

namespace AppBuilder;

public class ObservableNode : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] String prop = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}

}
public class BaseNode : ObservableNode
{
	private String? _name;

	[JsonProperty(Order = -5)]
	public String? Name { 
		get => _name; 
		set { 
			_name = value; 
			OnPropertyChanged();
			OnNameChanged();
		} 
	}

	[JsonProperty(Order = -4)]
	public String? Title { get; set; }

	[JsonProperty(Order = -3)]
	public String? Description { get; set; }

	[JsonIgnore]
	public virtual IEnumerable<BaseNode>? Children => null;

	[JsonIgnore]
	public static Boolean IsExpanded => true;

	private Boolean _isSelected;
	[JsonIgnore]
	public Boolean IsSelected { 
		get {
			return _isSelected;
		}
		set {
			if (_isSelected == value)
				return;
			_isSelected = value;
			OnPropertyChanged();
		}
	}

	[JsonIgnore]
	public virtual String Image => "/Images/FolderClosed.png";

	public virtual void OnNameChanged() 
	{ 
	}

	internal virtual void OnInit() 
	{ 
	}
}
