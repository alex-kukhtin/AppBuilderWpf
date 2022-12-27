using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
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

		if (String.IsNullOrEmpty(_viewModel.SolutionFileName))
		{
			var fd = new SaveFileDialog()
			{
				Filter = "Solution files|*.solution.json",
				OverwritePrompt = true,
				FileName = node.Name
			};

			var res = fd.ShowDialog();
			if (!res.HasValue || !res.Value)
				return;
			_viewModel.SolutionFileName = fd.FileName ?? throw new InvalidOperationException("Empty file name");

		}
		var json = JsonConvert.SerializeObject(node, Formatting.Indented, JsonHelpers.DefaultSettings);

		File.WriteAllText(_viewModel.SolutionFileName, json);
	}
}
