using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AppBuilder;
public class EnumBindingSource : MarkupExtension
{
	public Type? EnumType;

	public EnumBindingSource() { }

	public EnumBindingSource(Type enumType)
	{
		EnumType = enumType;
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		if (EnumType == null)
			throw new InvalidOperationException("The EnumType must be specified.");

		return Enum.GetValues(EnumType);
	}
}

