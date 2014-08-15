/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ButtonDialog.cs"
 * 
 *	This script is a container class for dialogue options
 *	that are linked to Conversations.
 * 
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class ButtonDialog
{
	
	public string label = "(Not set)";
	public bool isOn;
	public bool isLocked;
	public bool returnToConversation;
	
	public DialogueOption dialogueOption;
	
	public ButtonDialog ()
	{ }

}
