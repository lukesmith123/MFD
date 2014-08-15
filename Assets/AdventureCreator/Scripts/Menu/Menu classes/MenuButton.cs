/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuButton.cs"
 * 
 *	This MenuElement can be clicked on to perform a function defined in MenuSystem.
 * 
 */

using UnityEngine;

public class MenuButton : MenuElement
{
	
	private string label;
	private TextAnchor anchor;
	
	
	public MenuButton (string _label)
	{
		label = _label;
		isVisible = true;
		isClickable = true;
		numSlots = 1;
		anchor = TextAnchor.MiddleCenter;
	}
	
	
	public void SetAlignment (TextAnchor _anchor)
	{
		anchor = _anchor;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		_style.alignment = anchor;
		GUI.Label (relativeRect, label, _style);
		
		base.Display (_style, _slot);
	}
	
}