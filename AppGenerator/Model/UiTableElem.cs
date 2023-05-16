
using System;
using System.Collections.Generic;

namespace AppGenerator;

public enum UiView
{
	None,
	Dialog,
	Page
}

public enum SortDirection
{
	Asc,
	Desc
}
public record SortDescription
{
	public String? OrderBy { get; init; }
	public SortDirection Dir { get; init; }
	public String Direction => Dir.ToString().ToLowerInvariant();
}

public class UIElemForm
{
	public UiView View { get; init; }
	public SortDescription? InitialOrder { get; init; }
	public List<UIFieldElem> Fields { get; init; } = new();
}

public class UiTableElem
{
	public UIElemForm? Index { get; set; }
	public UIElemForm? EditItem { get; set; }
}
