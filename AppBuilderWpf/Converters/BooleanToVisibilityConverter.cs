// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AppBuilder;

public class BooleanToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not Boolean boolVal)
			throw new InvalidCastException("Expected 'boolean'");
		if (parameter != null && parameter.ToString() == "Invert")
			return boolVal ? Visibility.Hidden : Visibility.Visible;
		return boolVal ? Visibility.Visible : Visibility.Hidden;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
