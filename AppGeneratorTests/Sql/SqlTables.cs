// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

using AppGenerator.Interfaces;
using AppGenerator;

namespace AppGeneratorTests;

[TestClass]
[TestCategory("Sql.Tables")]
public class SqlTables
{
	[TestMethod]
	public void TableCatalog()
	{
		var sp = TestEngine.ServiceProvider();
		var sqlGen = sp.GetRequiredService<ISqlGenerator>();
		var writer = sp.GetService<IModelWriter>();
		var td = new TableDescriptor("/catalog1", "Catalog", new TableElem()
		{
			Name = "Unit",
			Fields = new List<FieldElem>()
			{
				new FieldElem()
				{
					Name = "Id",
					Type = FieldType.Identifier

				}
			}
		});
		var root = new AppElem()
		{
			IdentifierType = IdentifierType.BigInt
		};
		sqlGen.Generate(td, root);
		var result = sqlGen.Finish();

		Assert.IsNotNull(result);
	}

}
