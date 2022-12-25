using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBuilder;

public class DocumentsNode : BaseNode
{
	private readonly ObservableCollection<DocumentNode>? _documents;
	public DocumentsNode(ObservableCollection<DocumentNode>? documents)
	{
		Name = "Documents";
		_documents = documents;
	}
	public override IEnumerable<BaseNode>? Children => _documents;

}
public class DocumentNode : BaseNode
{
}
