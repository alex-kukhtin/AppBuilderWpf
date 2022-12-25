using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public class TablesNode : BaseNode
{
	private readonly IEnumerable<TableNode>? _tables; 
	public TablesNode(List<TableNode>? tables)
	{
		_tables = tables;
	}
	public override string? DisplayName => "Tables";
	public override IEnumerable<BaseNode>? Children => _tables;
}

public class TableNode : BaseNode
{
	public override string? DisplayName => Name;
}
