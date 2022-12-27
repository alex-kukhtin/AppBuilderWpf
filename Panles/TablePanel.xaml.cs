using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
		if (e.Source is not Button btnObj)
			return;
		_table.Fields.First(x => x == btnObj.CommandParameter);
		if (btnObj.CommandParameter is not FieldNode fn)
			return;
		_table.Fields.Remove(fn);
	}
}

