// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AppBuilder;

public class PointerToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (parameter != null && parameter.ToString() == "Invert")
			return value != null ? Visibility.Hidden : Visibility.Visible;
		return value != null ? Visibility.Visible : Visibility.Hidden;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
