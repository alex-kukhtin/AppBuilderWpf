// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.


namespace AppGenerator.Interfaces;

public interface ISqlGenerator
{
	void Generate(TableDescriptor descr, AppElem appElem);
	void Finish();
}
