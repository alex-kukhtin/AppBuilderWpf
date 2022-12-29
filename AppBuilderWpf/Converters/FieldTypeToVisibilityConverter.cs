
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AppBuilder;

public class FieldTypeToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not FieldType fieldType)
			throw new InvalidCastException("Invalid FieldType");
		return fieldType.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Hidden;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
