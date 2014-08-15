/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionList.cs"
 * 
 *	This script stores, and handles the sequentual triggering of, actions.
 *	It is derived by Cutscene, Hotspot, Trigger, and DialogOption.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class ActionList : MonoBehaviour
{

	public float triggerTime = 0f;
	public bool autosaveAfter = false;
	public List<Action> actions = new List<Action>();
	public int nextActionNumber = 0; 	// -1 to stop running and end cutscene, -2 to stop running but not end cutscene (used when chaining another ActionList)
	public Conversation conversation = null;
	
	protected LayerMask LayerHotspot;
	protected LayerMask LayerOff;
	protected StateHandler stateHandler;
	
	
	private void Awake ()
	{
		LayerHotspot = LayerMask.NameToLayer("Default");
		LayerOff = LayerMask.NameToLayer("Ignore Raycast");
	}
	
	
	void Start ()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
		{
			stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
		}
	}
	

	public void Interact ()
	{
		StartCoroutine ("InteractCoroutine");
	}
	
	
	public IEnumerator InteractCoroutine ()
	{
		if (actions.Count > 0)
		{
			nextActionNumber = 0;
			
			if (triggerTime > 0f)
			{
				// Wait for both the triggerTime, and until the current cutscene is over
				yield return new WaitForSeconds (triggerTime);
				
				while (stateHandler.gameState != GameState.Normal)
				{
					yield return new WaitForFixedUpdate ();
				}
			}

			ProcessAction (0);
		}
	}

		
	public void ProcessAction (int thisActionNumber)
	{
		if (stateHandler == null)
		{
			Start ();
		}
		
		if (stateHandler)
		{
			if (nextActionNumber > -1 && nextActionNumber < actions.Count)
			{
				stateHandler.gameState = GameState.Cutscene;
				
				nextActionNumber = thisActionNumber + 1;
				StartCoroutine ("RunAction", actions [thisActionNumber]);
			}
			else
			{
				EndCutscene ();
			}
		}
	}

	
	private IEnumerator RunAction (Action action)
	{
		action.isRunning = false;
		
		float waitTime = action.Run ();		
		if (waitTime > 0f)
		{
			while (action.isRunning)
			{
				yield return new WaitForSeconds (waitTime);
				waitTime = action.Run ();
			}
		}

		int actionEnd = action.End ();		

		if (actionEnd != 0)
		{
			nextActionNumber = actionEnd;
		}
		
		// Fix bug where actionlist would continue even if game is paused
		while (stateHandler.gameState == GameState.Paused)
		{
			yield return new WaitForFixedUpdate ();
		}
		
		ProcessAction (nextActionNumber);
	}

	
	public void EndCutscene ()
	{
		if (stateHandler)
		{
			if (stateHandler.gameState == GameState.Cutscene && nextActionNumber > -2)
			{
				stateHandler.gameState = GameState.Normal;
				
				if (autosaveAfter)
				{
					SaveSystem saveSystem = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>();
					saveSystem.SaveGame (-1);
				}
			}
			else if (autosaveAfter)
			{
				Debug.LogWarning ("Cannot autosave because another cutscene has started.");
			}
			
			if (conversation)
			{
				stateHandler.gameState = GameState.Cutscene;
				conversation.Interact ();
				conversation = null;
			}
		}
	}
	

	private void TurnOn ()
	{
		gameObject.layer = LayerHotspot;
	}
	
	
	private void TurnOff ()
	{
		gameObject.layer = LayerOff;
	}
	
	
	public void Kill ()
	{
		nextActionNumber = -1;
		StopCoroutine ("RunAction");
		StopCoroutine ("InteractCoroutine");
	}
	
	
	public bool IsRunning ()
	{
		if (nextActionNumber > -1)
		{
			return true;
		}
		
		return false;
	}
	
	
	void OnDestroy()
	{
		stateHandler = null;
	}
}
