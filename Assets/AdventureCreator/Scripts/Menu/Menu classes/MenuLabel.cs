/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuLabel.cs"
 * 
 *	This MenuElement provides a basic label.
 * 
 */

using UnityEngine;

public class MenuLabel : MenuElement
{
	
	public string label;
	
	private TextAnchor anchor;
	private bool doOutline;
		
	
	public MenuLabel (string _label, bool _doOutline)
	{
		label = _label;
		doOutline = _doOutline;
		isVisible = true;
		isClickable = false;
		numSlots = 1;
		anchor = TextAnchor.MiddleCenter;		
	}
	
	
	public void SetAlignment (TextAnchor _anchor)
	{
		anchor = _anchor;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		_style.wordWrap = true;
		_style.alignment = anchor;
		
		if (doOutline)
		{
			AdvGame.DrawOutline (relativeRect, label, _style, Color.black, Color.white, 1);
		}
		else
		{
			GUI.Label (relativeRect, label, _style);
		}
		
		base.Display (_style, _slot);
	}

}