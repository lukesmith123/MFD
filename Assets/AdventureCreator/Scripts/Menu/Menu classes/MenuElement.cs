/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuElement.cs"
 * 
 *	This is the base class for all menu elements.  It should never
 *	be added itself to a menu, as it is only a container of shared data.
 * 
 */

using UnityEngine;

public class MenuElement
{
	
	public bool isVisible;
	public bool isClickable;
	public Orientation orientation = Orientation.Horizontal;
	
	private Texture2D backgroundTexture;
	protected Rect relativeRect;
	protected Vector2 slotSize;
	protected int numSlots;
	
	
	public void SetBackground (Texture2D _backgroundTexture)
	{
		backgroundTexture = _backgroundTexture;
	}
	
	
	public virtual void Display (GUIStyle _style, int _slot)
	{
		if (backgroundTexture && _slot == 0)
		{
			GUI.DrawTexture (relativeRect, backgroundTexture, ScaleMode.StretchToFill, true, 0f);
		}
	}
	
	
	public Vector2 GetSize ()
	{
		Vector2 size = new Vector2 (relativeRect.width, relativeRect.height);
		return (size);
	}
	
	
	public void SetSize (Vector2 _size)
	{
		slotSize = new Vector2 (_size.x, _size.y);
	}
	
	
	public void SetAbsoluteSize (Vector2 _size)
	{
		slotSize = new Vector2 (_size.x / Screen.width, _size.y / Screen.height);
	}
	
	
	public int GetNumSlots ()
	{
		return numSlots;
	}
	
	
	public Rect GetSlotRectRelative (int _slot)
	{
		Rect positionRect = relativeRect;
		positionRect.width = slotSize.x * Screen.width;
		positionRect.height = slotSize.y * Screen.height;
		
		if (_slot > numSlots)
		{
			_slot = numSlots;
		}

		if (orientation == Orientation.Horizontal)
		{
			positionRect.x += slotSize.x * _slot * Screen.width;
		}
		else
		{
			positionRect.y += slotSize.y * _slot * Screen.height;
		}
		
		return (positionRect);
	}
	
	
	public virtual void RecalculateSize ()
	{
		if (orientation == Orientation.Horizontal)
		{
			relativeRect.width = slotSize.x * Screen.width * numSlots;
			relativeRect.height = slotSize.y * Screen.height;
		}
		else
		{
			relativeRect.width = slotSize.x * Screen.width;
			relativeRect.height = slotSize.y * Screen.height * numSlots;
		}
	}
	
	
	public void SetPosition (Vector2 _position)
	{
		relativeRect.x = _position.x;
		relativeRect.y = _position.y;
	}

}