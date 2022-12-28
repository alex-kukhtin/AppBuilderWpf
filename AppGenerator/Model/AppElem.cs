using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGenerator;

public enum IdentifierType
{
	BigInt,
	Integer,
	Guid
}

public class AppElem : BaseElem
{
	public Boolean MultiTenant { get; init; }
	public IdentifierType IdentifierType { get; init; }
	public List<CatalogElem> Catalogs { get; init; } = new();
	public List<DocumentElem> Documents { get; init; } = new();
}
