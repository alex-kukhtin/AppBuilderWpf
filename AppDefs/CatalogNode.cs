using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public class CatalogsNode : BaseNode
{
	private readonly ObservableCollection<CatalogNode>? _catalogs;
	public CatalogsNode(ObservableCollection<CatalogNode>? catalogs)
	{
		Name = "Catalogs";
		_catalogs = catalogs;
	}
	public override IEnumerable<BaseNode>? Children => _catalogs;

}
public class CatalogNode : BaseNode
{
}
