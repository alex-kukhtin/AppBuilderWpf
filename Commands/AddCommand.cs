using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppBuilder.Commands;

public class AddCommand : ICommand
{
	private readonly ViewModel _viewModel;
	public AddCommand(ViewModel viewModel)
	{
		_viewModel = viewModel;
	}
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute(object? parameter) => true;

	public void Execute(object? parameter)
	{
		var root = _viewModel.Root[0];
		switch (parameter?.ToString())
		{
			case "Table":
				var t = new TableNode() { Name = "NewTable" };
				root.Tables.Add(t);
				t.IsSelected = true;
				break;
		}
	}
}
