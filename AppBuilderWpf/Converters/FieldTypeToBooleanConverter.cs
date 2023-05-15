// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;

namespace AppBuilder;

public class FieldTypeToBooleanConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not FieldType fieldType)
			throw new InvalidCastException("Invalid FieldType");
		return fieldType.ToString() == parameter.ToString(); ;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
