// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;

namespace AppGenerator.Interfaces;

public interface ISqlGenerator
{
	void Generate(TableDescriptor descr, AppElem appElem);
	String Finish();
}
