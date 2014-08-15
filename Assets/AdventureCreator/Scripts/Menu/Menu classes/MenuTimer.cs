/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuTimer.cs"
 * 
 *	This MenuElement can be used in conjunction with MenuDialogList to create
 *	timed conversations, "Walking Dead"-style.
 * 
 */

using UnityEngine;

public class MenuTimer : MenuElement
{
	
	private Texture2D timerTexture;
	private PlayerInput playerInput;
	
	
	public MenuTimer ()
	{
		isVisible = true;
		isClickable = false;
		numSlots = 1;
	}
	
	
	public void SetTimerTexture (Texture2D _timerTexture)
	{
		timerTexture = _timerTexture;
	}
	
	
	public override void Display (GUIStyle _style, int _slot)
	{
		if (!playerInput)
		{
			playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
		}
		
		if (playerInput && playerInput.activeConversation && playerInput.activeConversation.isTimed)
		{
			Rect timerRect = relativeRect;
			timerRect.width = slotSize.x * Screen.width * playerInput.activeConversation.GetTimeRemaining ();
			GUI.DrawTexture (timerRect, timerTexture, ScaleMode.StretchToFill, true, 0f);
		}
		
		base.Display (_style, _slot);
	}

}