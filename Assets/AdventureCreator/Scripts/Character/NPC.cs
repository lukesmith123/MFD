/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"NPC.cs"
 * 
 *	This is attached to all non-Player characters.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : Char
{
	
	LayerMask LayerOn;
	LayerMask LayerOff;
	
	
	void Awake ()
	{
		ResetBaseClips ();
		
		LayerOn = LayerMask.NameToLayer ("Default");
		LayerOff = LayerMask.NameToLayer ("Ignore Raycast");
	}
	
	
	new private void FixedUpdate ()
	{
		if (activePath && !pausePath)
		{
			charState = CharState.Move;
			CheckIfStuck ();
		}
		
		base.FixedUpdate ();
	}
	
	
	private void TurnOn ()
	{
		gameObject.layer = LayerOn;
	}
	

	private void TurnOff ()
	{
		gameObject.layer = LayerOff;
	}
	
}
