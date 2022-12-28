using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGenerator; 
internal class BaseElem
{ 
	public String? Name { get; init; }
	public String? Title { get; set; }
	public String? Description { get; set; }
}
