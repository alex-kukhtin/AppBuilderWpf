// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Windows.Input;

namespace AppBuilder;

public class AddDetailsCommand : ICommand
{
	private readonly ViewModel _viewModel;
	public AddDetailsCommand(ViewModel viewModel)
	{
		_viewModel = viewModel;
	}
	public event EventHandler? CanExecuteChanged;

	public void OnCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool CanExecute(object? parameter)
	{
		return (_viewModel?.SelectedItem as TableNode) != null;
	}

	public void Execute(object? parameter)
	{
		var selTable = _viewModel.SelectedItem as TableNode;
		if (selTable == null) 
			return;
		selTable.AddDetails();
	}
}
