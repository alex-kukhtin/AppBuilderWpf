using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public class TablesNode : BaseNode
{
	private readonly ObservableCollection<TableNode>? _tables; 
	public TablesNode(ObservableCollection<TableNode>? tables)
	{
		Name = "Tables";
		_tables = tables;
	}
	public override IEnumerable<BaseNode>? Children => _tables;
}

public class TableNode : BaseNode
{
}
