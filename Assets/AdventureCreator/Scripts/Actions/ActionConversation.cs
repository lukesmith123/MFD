/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionConversation.cs"
 * 
 *	This action turns on a conversation.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionConversation : Action
{
	
	public Conversation conversation;
	
	
	public ActionConversation ()
	{
		this.isDisplayed = true;
		title = "Dialogue: Start conversation";
	}
	
	
	override public float Run ()
	{
		conversation.Interact ();
		
		return 0f;
	}
	
	
	override public int End ()
	{
		return -2;
	}
	
	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		conversation = (Conversation) EditorGUILayout.ObjectField ("Conversation:", conversation, typeof (Conversation), true);
	}
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (conversation)
		{
			labelAdd = " (" + conversation + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}