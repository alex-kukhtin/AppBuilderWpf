// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;

namespace AppGenerator.Interfaces;

public interface ISqlGenerator
{
	void Start(AppElem appElem);
	void GenerateStruct(TableDescriptor descr);
	String GenerateEndpoint(TableDescriptor descr);
	void GenerateUi(TableDescriptor descr);
	void Finish();
}
