using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private AppNode? _appNode;
	public MainWindow()
	{
		InitializeComponent();
	}

	private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
	{
		var of = new OpenFileDialog()
		{
			Filter = "Solution files|*.solution.json",
			CheckFileExists = true
		};
		var res = of.ShowDialog();
		if (res.HasValue && res.Value)
		{
			var json = File.ReadAllText(of.FileName);
			_appNode = JsonConvert.DeserializeObject<AppNode>(json, JsonHelpers.DefaultSettings);
			if (_appNode == null)
				return;
			this.DataContext = new ObservableCollection<BaseNode>() {
				_appNode
			};
		}
	}

	private void Test_Executed(object sender, ExecutedRoutedEventArgs e)
	{
		if (_appNode == null)
			return;
		_appNode.Tables.Add(new TableNode()
		{
			Name = "NewTable"
		});
	}
}
