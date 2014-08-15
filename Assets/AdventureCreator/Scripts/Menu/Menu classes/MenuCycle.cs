/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuCycle.cs"
 * 
 *	This MenuElement is like a label, only it's text cycles through an array when clicked on.
 * 
 */

using UnityEngine;

public class MenuCycle : MenuElement
{

	public int selected;
	
	private string label;
	private string[] options;
	
	
	public MenuCycle (string _label, string[] _options)
	{
		label = _label;
		options = _options;
		selected = 0;
		isVisible = true;
		isClickable = true;
		numSlots = 1;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		_style.alignment = TextAnchor.MiddleLeft;
		string toggleText = label + " : ";//
		if (options.Length > selected)
		{
			toggleText += options[selected];
		}
		else
		{
			Debug.Log ("Could not gather options options for MenuCycle " + label);
			selected = 0;
			toggleText += options[selected];
		}
		
		GUI.Label (relativeRect, toggleText, _style);
		
		base.Display (_style, _slot);
	}
	
	
	public void Cycle ()
	{
		selected ++;
		if (selected > options.Length-1)
		{
			selected = 0;
		}
	}
	
}