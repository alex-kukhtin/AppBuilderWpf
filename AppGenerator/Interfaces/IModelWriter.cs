// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;

namespace AppGenerator.Interfaces;

public interface IModelWriter
{
	void CreateDirectory(String path);
	void WriteFile(String content, String path, String fileName);
}
