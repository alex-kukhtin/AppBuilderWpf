
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AppBuilder;

public class AppNode : BaseNode
{
	public ObservableCollection<TableNode> Tables { get; init; } = new();
	public ObservableCollection<CatalogNode> Catalogs { get; init; } = new();
	public ObservableCollection<DocumentNode> Documents { get; init; } = new();

	public override IEnumerable<BaseNode>? Children
	{
		get
		{
			yield return new TablesNode(Tables);
			yield return new CatalogsNode(Catalogs);
			yield return new DocumentsNode(Documents);
		}
	}
}

