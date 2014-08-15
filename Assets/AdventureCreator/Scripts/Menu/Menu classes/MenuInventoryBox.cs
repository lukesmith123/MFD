/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuInventoryBox.cs"
 * 
 *	This MenuElement lists all inventory items held by the player.
 * 
 */

using UnityEngine;

public class MenuInventoryBox : MenuElement
{
	
	private RuntimeInventory runtimeInventory;
	private RuntimeActionList runtimeActionList;
	
	
	public MenuInventoryBox ()
	{
		numSlots = 0;
		isVisible = true;
		isClickable = true;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		if (GetTexture (_slot))
		{
			GUI.DrawTexture (GetSlotRectRelative (_slot), GetTexture (_slot), ScaleMode.StretchToFill, true, 0f);
		}
		
		// Draw a blank label so that we can highlight the rect
		GUI.Label (GetSlotRectRelative (_slot), GetCount (_slot), _style);

		base.Display (_style, _slot);
	}
	
	
	public override void RecalculateSize ()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>())
		{
			runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();
		
			numSlots = runtimeInventory.localItems.Count;
			runtimeInventory = null;
		}
		
		base.RecalculateSize ();
	}
	
	
	private string GetCount (int slot)
	{
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>())
		{
			runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();

			int count = runtimeInventory.localItems [slot].count;
			runtimeInventory = null;
		
			if (count > 1)
			{
				return count.ToString ();
			}
		}
		
		return "";
	}
	
	
	private Texture2D GetTexture (int slot)
	{
		runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();
		
		if (runtimeInventory)
		{
			Texture2D texture = runtimeInventory.localItems [slot].tex;
			runtimeInventory = null;
			return texture;
		}

		return null;
	}
	
	
	public string GetLabel (int slot)
	{
		runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();
		string label = "";
		
		if (runtimeInventory)
		{
			label = runtimeInventory.localItems [slot].label;
			runtimeInventory = null;
		}
		
		return label;
	}
	
}