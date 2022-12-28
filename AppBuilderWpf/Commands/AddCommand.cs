
using System;
using System.Windows.Input;

namespace AppBuilder;

public class AddCommand : ICommand
{
	private readonly ViewModel _viewModel;
	public AddCommand(ViewModel viewModel)
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
		var root = _viewModel.Root[0];
		switch (parameter?.ToString())
		{
			case "Catalog":
				root.AddCatalog();
				break;
			case "Document":
				root.AddDocument();
				break;
		}
	}
}
