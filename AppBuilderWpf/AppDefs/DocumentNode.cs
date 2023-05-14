
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace AppBuilder;

public class DocumentsNode : BaseNode
{
	private readonly ObservableCollection<DocumentNode>? _documents;
	public DocumentsNode(ObservableCollection<DocumentNode>? documents)
	{
		Name = "Documents";
		_documents = documents;
	}
	public override IEnumerable<TableNode>? Children => _documents;

}
public class DocumentNode : TableNode
{
	[JsonIgnore]
	public override String Image => "/Images/Document.png";
	[JsonIgnore]
	public override String NameWithSchema => $"Document.{Name}";

	public void ApplyDefaults()
	{
		AddField(true, "Id", FieldType.Identifier);
		AddField(true, "Void", FieldType.Boolean);
		AddField(false, "Date", FieldType.Date, "@[Date]");
		AddField(false, "No", FieldType.String, "@[Number]", 50);
		AddField(false, "Memo", FieldType.String, "@[Memo]", 255);
	}
}
