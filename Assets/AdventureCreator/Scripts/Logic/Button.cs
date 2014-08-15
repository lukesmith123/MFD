/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Button.cs"
 * 
 *	This script is a container class for interactions
 *	that are linked to Hotspots and NPCs.
 * 
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Button
{
	
	public Interaction interaction = null;
	public int invID = 0;

	public PlayerAction playerAction = PlayerAction.DoNothing;
	
	public bool setProximity = false;
	public float proximity = 1f;
	
	
	public Button ()
	{ }
	
}
