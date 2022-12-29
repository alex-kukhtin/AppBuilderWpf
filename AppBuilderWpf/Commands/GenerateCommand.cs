using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using AppGenerator;
using Newtonsoft.Json;

namespace AppBuilder;

public class GenerateCommand : ICommand
{
	private readonly ViewModel _viewModel;
	public GenerateCommand(ViewModel viewModel)
	{
		_viewModel = viewModel;
	}

	public event EventHandler? CanExecuteChanged;

	public void OnCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
	public bool CanExecute(object? parameter) => _viewModel.Root.Count > 0;

	public void Execute(object? parameter)
	{
		if (!CanExecute(parameter))
			return;
		MessageBox.Show("GenerateCommand. Yet not implemtnted");
		/*
		var gen = new ApplicationGenerator(_viewModel.SolutionFileName);
		gen.GenerateAppliction();
		var node = _viewModel.Root[0];
		// ensure saved
		_viewModel.SaveCommand.Execute(parameter);
		*/
	}
}
