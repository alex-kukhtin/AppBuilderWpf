using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public class AppNode : BaseNode
{
	public List<TableNode>? Tables { get; set; }
	public List<CatalogNode>? Catalogs { get; set; }

	[JsonIgnore]
	public override IEnumerable<BaseNode> Children 
	{
		get
		{
			yield return new TablesNode(Tables);
		}
	}
}

