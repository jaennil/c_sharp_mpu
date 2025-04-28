namespace cnsRoadEditor;

public enum Road
{
	[View("─")]
	Horizontal = 1,

	[View("│")]
	Vertical,
	
	[View("\u2510")]
	RightTopCorner,
	
	[View("\u250c")]
	LeftTopCorner,
	
	[View("\u2518")]
	RightBottomCorner,
	
	[View("\u2514")]
	LeftBottomCorner,
	
	[View("\u251c")]
	LeftT,
	
	[View("\u2524")]
	RightT,
	
	[View("\u252c")]
	BottomT,
	
	[View("\u2534")]
	TopT,

    [View("\u22c5")]
    None,
}
