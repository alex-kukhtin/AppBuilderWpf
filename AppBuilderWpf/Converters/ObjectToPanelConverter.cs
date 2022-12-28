using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Automation.Provider;

namespace AppBuilder;

public class ObjectToPanelConverter : IMultiValueConverter
{
	public Object? Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
	{
		var vm = values[1] as ViewModel ?? throw new InvalidOperationException("Invalid Converter parameter");
		return values[0] switch
		{
			AppNode appNode => new AppPanel(appNode),
			CatalogNode catalogNode => new TablePanel(catalogNode, vm),
			DocumentNode documentNode => new TablePanel(documentNode, vm),
			TableNode tableNode => new TablePanel(tableNode, vm),
			_ => null,
		}; ;
	}

	public Object[] ConvertBack(Object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
