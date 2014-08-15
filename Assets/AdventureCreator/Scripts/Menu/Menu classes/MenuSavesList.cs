/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuSavesList.cs"
 * 
 *	This MenuElement handles the display of any saved games recorded.
 * 
 */

using UnityEngine;

public class MenuSavesList : MenuElement
{
	
	public MenuSavesList ()
	{
		numSlots = 0;
		isVisible = true;
		isClickable = true;
	}
	

	public override void Display (GUIStyle _style, int _slot)
	{
		_style.alignment = TextAnchor.MiddleCenter;
		
		#if !UNITY_WEBPLAYER
		GUI.Label (GetSlotRectRelative (_slot), SaveSystem.GetSaveSlotName (_slot), _style);
		#endif
		
		base.Display (_style, _slot);
	}
	
	
	public override void RecalculateSize ()
	{
		#if !UNITY_WEBPLAYER
		numSlots = SaveSystem.GetNumSlots ();
		#endif
		
		base.RecalculateSize ();
	}
	
}