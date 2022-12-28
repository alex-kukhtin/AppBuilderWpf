using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGenerator;

public class TableElem : BaseElem
{
	public List<FieldElem> Fields { get; init; } = new();

	public List<TableElem> Details { get; init; } = new();
}
