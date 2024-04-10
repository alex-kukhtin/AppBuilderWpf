
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AppBuilder;
/// <summary>
/// Interaction logic for TablePanel.xaml
/// </summary>
public partial class TablePanel : UserControl
{
	private readonly TableNode _table;
	private readonly ViewModel _vm;
	public TablePanel(TableNode table, ViewModel vm)
	{
		_table = table;
		_vm = vm;
		InitializeComponent();
		this.DataContext = table;
	}

	public IEnumerable<String> RefTables => _vm.RefTables;

	private void AddField_Click(object sender, RoutedEventArgs e)
	{
		_table.CreateField();
	}

	private void DeleteField_Click(object sender, RoutedEventArgs e)
	{
		if (e.Source is not Button btnObj || btnObj.CommandParameter is not FieldNode fn)
			return;
		_table.Fields.Remove(fn);
	}
	private void FieldUp_Click(object sender, RoutedEventArgs e)
	{
		if (e.Source is not Button btnObj || btnObj.CommandParameter is not FieldNode fn)
			return;
		int pos = _table.Fields.IndexOf(fn);
		if (pos < 1) 
			return;
		_table.Fields.Move(pos, pos - 1);
	}

	private void FieldDown_Click(object sender, RoutedEventArgs e)
	{
		if (e.Source is not Button btnObj || btnObj.CommandParameter is not FieldNode fn)
			return;
		int pos = _table.Fields.IndexOf(fn);
		if (pos >= _table.Fields.Count - 1)
			return;
		_table.Fields.Move(pos, pos + 1);
	}

	private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_table.SelectedField = e.AddedItems?[0] as FieldNode;
	}
}