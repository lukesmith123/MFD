/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuToggle.cs"
 * 
 *	This MenuElement toggles between On and Off when clicked on.
 *	It can be used for changing boolean options.
 * 
 */

using UnityEngine;

public class MenuToggle : MenuElement
{

	public bool isOn;

	private string label;
	
	
	public MenuToggle (string _label)
	{
		label = _label;
		isOn = false;
		isVisible = true;
		isClickable = true;
		numSlots = 1;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		_style.alignment = TextAnchor.MiddleLeft;
		string toggleText = label + " : ";
		
		if (isOn)
		{
			toggleText += "On";
		}
		else
		{
			toggleText += "Off";
		}
		
		GUI.Label (relativeRect, toggleText, _style);
		
		base.Display (_style, _slot);
	}
	
	
	public void Toggle ()
	{
		if (isOn)
		{
			isOn = false;
		}
		else
		{
			isOn = true;
		}
	}

}