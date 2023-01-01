// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using AppGenerator;
using AppGenerator.Interfaces;

namespace SqlGenerator.MsSqlServer;

internal class EndpointGenerator
{
	private readonly TableDescriptor _descr;
	private readonly AppElem _root;
	private readonly IModelWriter _writer;
	public EndpointGenerator(IModelWriter writer, TableDescriptor descr, AppElem root)
	{
		_writer = writer;
		_descr = descr;
		_root = root;	
	}

	public void Generate()
	{
		String fileName = $"{_descr.Table.Name!.ToLowerInvariant()}.model.sql";
		_writer.WriteFile("", _descr.Path, fileName);
		int z = 55;
	}
}
