using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppBuilder;

public class OpenCommand : ICommand
{
	private readonly ViewModel _viewModel;
	public OpenCommand(ViewModel viewModel)
	{
		_viewModel = viewModel;
	}

	public event EventHandler? CanExecuteChanged;
	public bool CanExecute(object? parameter) => true;

	public void Execute(object? parameter)
	{
		var of = new OpenFileDialog()
		{
			Filter = "Solution files|*.solution.json",
			CheckFileExists = true
		};
		var res = of.ShowDialog();
		if (res.HasValue && res.Value)
		{
			var json = File.ReadAllText(of.FileName);
			var appNode = JsonConvert.DeserializeObject<AppNode>(json, JsonHelpers.DefaultSettings);
			_viewModel.SetContext(appNode, of.FileName);
		}
	}
}
