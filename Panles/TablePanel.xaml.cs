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
	public TablePanel(TableNode table)
	{
		InitializeComponent();
		this.DataContext = table;
	}
}

