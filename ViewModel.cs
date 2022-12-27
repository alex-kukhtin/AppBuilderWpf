
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AppBuilder;

public class ViewModel : INotifyPropertyChanged
{
	private AppNode _appNode = new() { Name = "Application" };
	public ViewModel()
	{
		Root.Add(_appNode);
		_appNode.IsSelected = true;
	}

	public ObservableCollection<AppNode> Root { get; } = new ObservableCollection<AppNode>();

	public String SolutionFileName { get; set; } = String.Empty;

	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] String prop = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}

	public void Open(AppNode? node, String fileName)
	{
		Root.Clear();
		if (node != null)
			_appNode = node;
		Root.Add(_appNode);
		_appNode.IsSelected = true;
		SolutionFileName = fileName;
		OnPropertyChanged(String.Empty);
	}

	public IEnumerable<String> RefTables =>
		_appNode.Catalogs.Cast<TableNode>()
				.Union(_appNode.Documents.Cast<TableNode>())
				.Select(t => t.NameWithSchema).ToList();

	private Object? _selectedItem;
	public Object? SelectedItem
	{
		get 
		{ 
			return _selectedItem; 
		}
		set 
		{ 
			_selectedItem = value; 
			OnPropertyChanged(); 
			_addDetailCommand?.OnCanExecuteChanged();
		}
	}

	AddDetailsCommand? _addDetailCommand;
	public ICommand NewCommand => new NewCommand(this);
	public ICommand OpenCommand => new OpenCommand(this);
	public ICommand GenerateCommand => new GenerateCommand(this);
	public ICommand AddCommand => new AddCommand(this);
	public ICommand SaveCommand => new SaveCommand(this);
	public ICommand AddDetailsCommand => _addDetailCommand ??= new AddDetailsCommand(this);
}

