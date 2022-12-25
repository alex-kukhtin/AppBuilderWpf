using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public class BaseNode
{
	public String? Name { get; set; }

	[JsonIgnore]
	public virtual IEnumerable<BaseNode>? Children => null;
}
