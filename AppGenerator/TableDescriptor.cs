using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGenerator;

public class TableDescriptor
{
	public String Path { get; init; }
	public TableElem Table { get; init; }
	public String Schema { get; init; }
	public TableDescriptor(String path, String schema, TableElem table)
	{
		Path = path;
		Schema = schema;
		Table = table;
	}
}
