/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuSlider.cs"
 * 
 *	This MenuElement creates a slider for eg. volume control.
 * 
 */

using UnityEngine;

public class MenuSlider : MenuElement
{
	
	public int amount;
	
	private string label;
	private Texture2D sliderTexture;
	
	
	public MenuSlider (string _label)
	{
		label = _label;
		isVisible = true;
		isClickable = true;
		numSlots = 1;
	}
	
	
	public void SetSliderTexture (Texture2D _sliderTexture)
	{
		sliderTexture = _sliderTexture;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		_style.alignment = TextAnchor.MiddleLeft;
		GUI.Label (relativeRect, label, _style);
		
		Rect sliderRect = relativeRect;
		sliderRect.x = relativeRect.x + (relativeRect.width / 2);
		sliderRect.width = slotSize.x * Screen.width * (float) amount / 10 * 0.5f;
		GUI.DrawTexture (sliderRect, sliderTexture, ScaleMode.StretchToFill, true, 0f);
		
		base.Display (_style, _slot);
	}
	
	
	public void Change ()
	{
		amount ++; 
		
		if (amount > 10)
		{
			amount = 0;
		}
	}

}