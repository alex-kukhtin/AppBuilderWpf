// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;

namespace AppGenerator.Interfaces;

public interface ISqlGenerator
{
	void GenerateStruct(TableDescriptor descr, AppElem appElem);
	String GenerateEndpoint(TableDescriptor descr, AppElem appElem);
	void Finish();
}
