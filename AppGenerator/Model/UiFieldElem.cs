using System;

namespace AppGenerator;

public enum SearchType
{
	None,
	Exact,
	Like
}

public class UIFieldElem : BaseElem
{
	public String? Field { get; init; }
	public Boolean Sort { get; init; }
	public SearchType Search { get; init; }
	public Int32 TabIndex { get; init; }
	public Boolean Multiline { get; init; }
	public Boolean Required { get; init; }
	public Boolean IsSearch => Search != SearchType.None;	
}
