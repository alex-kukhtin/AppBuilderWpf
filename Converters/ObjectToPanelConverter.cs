using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;

namespace AppBuilder;

public class ObjectToPanelConverter : IValueConverter
{
	public Object? Convert(Object? value, Type targetType, Object? parameter, CultureInfo culture)
	{
		return value switch
		{
			AppNode appNode => new AppPanel(appNode),
			TableNode tableNode => new TablePanel(tableNode),
			CatalogNode catalogNode => new CatalogPanel(catalogNode),
			_ => null,
		}; ;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
