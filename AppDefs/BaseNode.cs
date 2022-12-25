using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public class BaseNode
{
	public String? Name { get; set; }

	[JsonIgnore]
	public virtual String? DisplayName => Name;

	[JsonIgnore]
	public virtual IEnumerable<BaseNode>? Children => null;
}
