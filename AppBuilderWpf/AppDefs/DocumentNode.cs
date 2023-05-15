
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
		AddField("Id", FieldType.Identifier, FieldRole.PrimaryKey);
		AddField("Void", FieldType.Boolean, FieldRole.Void);
		AddField("Date", FieldType.Date, FieldRole.Ordinal, "@[Date]");
		AddStringField("No", "@[Number]", 50);
		AddStringField("Memo", "@[Memo]", 255);
	}
}
