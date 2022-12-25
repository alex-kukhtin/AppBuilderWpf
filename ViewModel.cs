using AppBuilder.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppBuilder;

public class ViewModel : INotifyPropertyChanged
{

	public ObservableCollection<AppNode> Root { get; } = new ObservableCollection<AppNode>();

	public String SolutionPath { get; private set; } = String.Empty;

	public event PropertyChangedEventHandler? PropertyChanged;
	public void SetContext(AppNode? node, String fileName)
	{
		Root.Clear();
		if (node != null)
			Root.Add(node);
		SolutionPath = Path.GetDirectoryName(fileName) ?? String.Empty;
		node.IsSelected = true;
		OnPropertyChanged(String.Empty);
	}

	public void OnPropertyChanged([CallerMemberName] String prop = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}

	public ICommand OpenCommand => new OpenCommand(this);
	public ICommand GenerateCommand => new GenerateCommand(this);
	public ICommand AddCommand => new AddCommand(this);
}
