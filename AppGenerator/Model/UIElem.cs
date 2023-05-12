// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;

namespace AppGenerator;

public enum SearchMode
{
	Default,
	Exact,
	Like
}

public record UIField
{
	public String? Field { get; init; }
	public Boolean Sort { get; init; }
	public SearchMode Search { get; init; }
	public Int32 TabIndex { get; init; }
	public Boolean Multiline { get; init; }
	public Boolean Required { get; init; }
}

public record UIForm
{
	public List<UIField> Fields { get; init; } = new();
}


public record UIElem
{
	public UIForm? List { get; init; }
	public UIForm? Edit { get; init; }
}
