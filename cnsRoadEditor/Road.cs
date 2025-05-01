namespace cnsRoadEditor;

public enum Road
{
	[View("─")]
	Horizontal = 1,

	[View("│")]
	Vertical,
	
	[View("┐")]
	RightTopCorner,
	
	[View("┌")]
	LeftTopCorner,
	
	[View("┘")]
	RightBottomCorner,
	
	[View("└")]
	LeftBottomCorner,
	
	[View("├")]
	LeftT,
	
	[View("┤")]
	RightT,
	
	[View("┬")]
	BottomT,
	
	[View("┴")]
	TopT,

    [View("┼")]
    Cross,

    [View("\u22c5")]
    None,
}
