/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuDialogList.cs"
 * 
 *	This MenuElement lists the available options of the active conversation,
 *	and runs them when clicked on.
 * 
 */

using UnityEngine;

public class MenuDialogList : MenuElement
{
	
	private string[] labels;
	
	public MenuDialogList ()
	{
		isVisible = true;
		isClickable = true;
		numSlots = 0;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		_style.alignment = TextAnchor.MiddleLeft;
		
		PlayerInput playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
		
		if (playerInput && playerInput.activeConversation)
		{
			GUI.Label (GetSlotRectRelative (_slot), labels [_slot], _style);
		}
		
		base.Display (_style, _slot);
	}
	
	
	public override void RecalculateSize ()
	{
		if (GameObject.FindWithTag (Tags.gameEngine) && GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>())
		{
			PlayerInput playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
			
			if (playerInput && playerInput.activeConversation)
			{
				numSlots = playerInput.activeConversation.GetCount ();
				
				labels = new string[numSlots];
				for (int i=0; i<numSlots; i++)
				{
					labels[i] = playerInput.activeConversation.GetOptionName (i);
				}
			}
			else
			{
				numSlots = 0;
			}
		}
		
		base.RecalculateSize ();
	}
	
	
	public void RunOption (int slot)
	{
		PlayerInput playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
		
		if (playerInput && playerInput.activeConversation)
		{
			playerInput.activeConversation.RunOption (slot);
		}
	}
	
}