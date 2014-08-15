/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionSendMessage.cs"
 * 
 *	This action calls "SendMessage" on a GameObject.
 *	Both standard messages, and custom ones with paremeters, can be sent.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionSendMessage : Action
{
	
	public bool isSelf = false;
	public GameObject linkedObject;
	
	public MessageToSend messageToSend;
	public enum MessageToSend { TurnOn, TurnOff, Interact, Kill, Custom };
	
	public string customMessage;
	public bool sendValue;
	public int customValue;

	
	public ActionSendMessage ()
	{
		this.isDisplayed = true;
		title = "Object: Send message";
	}
	
	
	override public float Run ()
	{
		if (linkedObject)
		{
			if (messageToSend == MessageToSend.TurnOn)
			{
				linkedObject.SendMessage ("TurnOn");
			}
			else if (messageToSend == MessageToSend.TurnOff)
			{
				linkedObject.SendMessage ("TurnOff");
			}
			else if (messageToSend == MessageToSend.Interact)
			{
				linkedObject.SendMessage ("Interact");
			}
			else if (messageToSend == MessageToSend.Kill)
			{
				linkedObject.SendMessage ("Kill");
			}
			else
			{
				if (!sendValue)
				{
					linkedObject.SendMessage (customMessage);
				}
				else
				{
					linkedObject.SendMessage (customMessage, customValue);
				}
			}
		}
		
		return 0f;
	}
	
	
	override public int End ()
	{
		// If the linkedObject is an immediately-starting ActionList, don't end the cutscene
		if (linkedObject && messageToSend == MessageToSend.Interact && linkedObject.GetComponent <Cutscene>())
		{
			Cutscene tempAction = linkedObject.GetComponent<Cutscene>();
			
			if (tempAction.triggerTime == 0f)
			{
				return -2;
			}
		}
		
		return (base.End());
	}
	
	
	#if UNITY_EDITOR

	public void ShowGUI (GameObject ownSelf)
	{
		isSelf = EditorGUILayout.Toggle ("Is self?", isSelf);
		if (isSelf)
		{
			linkedObject = ownSelf;
		}
		
		ShowGUI ();
	}
	
	
	public override void ShowGUI ()
	{
		if (!isSelf)
		{
			linkedObject = (GameObject) EditorGUILayout.ObjectField ("Object to affect:", linkedObject, typeof(GameObject), true);
		}

		messageToSend = (MessageToSend) EditorGUILayout.EnumPopup ("Message to send:", messageToSend);
		if (messageToSend == MessageToSend.Custom)
		{
			customMessage = EditorGUILayout.TextField ("Method name:", customMessage);
			
			sendValue = EditorGUILayout.Toggle ("Pass integer to method?", sendValue);
			if (sendValue)
			{
				customValue = EditorGUILayout.IntField ("Integer to send:", customValue);
			}
		}
		
		AfterRunningOption ();
	}
	
	
	public override string SetLabel ()
	{
		string labelAdd = "";
		
		if (linkedObject)
		{
			if (messageToSend == MessageToSend.TurnOn)
			{
				labelAdd += " ('Turn on' ";
			}
			else if (messageToSend == MessageToSend.TurnOff)
			{
				labelAdd += " ('Turn off' ";
			}
			else if (messageToSend == MessageToSend.Interact)
			{
				labelAdd += " ('Interact' ";
			}
			else if (messageToSend == MessageToSend.Kill)
			{
				labelAdd += " ('Kill' ";
			}
			else
			{
				labelAdd += " ('" + customMessage + "' ";
			}

			labelAdd += " to " + linkedObject.name + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}