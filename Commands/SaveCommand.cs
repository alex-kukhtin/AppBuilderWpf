using System;
using System.IO;
using System.Windows.Input;

using Newtonsoft.Json;

namespace AppBuilder;

public class SaveCommand : ICommand
{
	private readonly ViewModel _viewModel;
	public SaveCommand(ViewModel viewModel)
	{
		_viewModel = viewModel;
	}

	public event EventHandler? CanExecuteChanged;

	public void OnCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool CanExecute(object? parameter) => true;

	public void Execute(object? parameter)
	{
		var node = _viewModel.Root[0];
		var json = JsonConvert.SerializeObject(node, Formatting.Indented, JsonHelpers.DefaultSettings);

		var path = Path.Combine("C:/Temp", "result.json");
		File.WriteAllText(path, json);
	}
}
