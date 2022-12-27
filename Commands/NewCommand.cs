using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AppBuilder;

public class NewCommand : ICommand
{
	private readonly ViewModel _viewModel;
	public NewCommand(ViewModel viewModel)
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
		MessageBox.Show("New Command");
	}
}
