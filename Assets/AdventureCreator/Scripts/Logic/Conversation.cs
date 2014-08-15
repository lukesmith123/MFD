/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Conversation.cs"
 * 
 *	This script is handles character conversations.
 *	It generates instances of DialogOption for each line
 *	that the player can choose to say.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Conversation : MonoBehaviour
{
	
	public bool isTimed = false;
	public float timer = 5f;
	public ButtonDialog defaultOption;
	
	private float startTime;
	private bool isRunning;
	public List<ButtonDialog> options = new List<ButtonDialog>();
	
	private PlayerInput playerInput;
	
	
	void Awake ()
	{
		if (GameObject.FindWithTag (Tags.gameEngine) && GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>())
		{
			playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
		}
	}
	
	
	public void Interact ()
	{
		// End the conversation if no options are present
		
		bool onePresent = false;
		foreach (ButtonDialog _option in options)
		{
			if (_option.isOn)
			{
				onePresent = true;
			}
		}

		if (playerInput)
		{
			if (onePresent)
			{
				playerInput.activeConversation = this;
			}
			else
			{
				playerInput.activeConversation = null;
			}
		}
		
		if (isTimed)
		{
			startTime = Time.time;
			StartCoroutine (RunDefault ());
		}
	}

	
	public void TurnOn ()
	{
		Interact ();
	}

	
	public void TurnOff ()
	{
		if (playerInput)
		{
			playerInput.activeConversation = null;
		}
	}
	
	
	private IEnumerator RunDefault ()
	{
		yield return new WaitForSeconds (timer);
		
		if (playerInput && playerInput.activeConversation != null && defaultOption != null)
		{
			playerInput.activeConversation = null;
			
			if (defaultOption.returnToConversation)
			{
				defaultOption.dialogueOption.conversation = this;
			}
			else
			{
				defaultOption.dialogueOption.conversation = null;
			}
			
			defaultOption.dialogueOption.Interact ();
		}
	}
	
	
	private IEnumerator RunOptionCo (int i)
	{
		yield return new WaitForSeconds (0.3f);
		
		if (options[i].returnToConversation)
		{
			options[i].dialogueOption.conversation = this;
		}
		else
		{
			options[i].dialogueOption.conversation = null;
		}
	
		options[i].dialogueOption.Interact ();
	}
	
	
	public void RunOption (int slot)
	{
		int i = ConvertSlotToOption (slot);
		
		if (playerInput)
		{
			playerInput.activeConversation = null;
		}
		
		StartCoroutine (RunOptionCo (i));
	}
	
	
	public float GetTimeRemaining ()
	{
		return ((startTime + timer - Time.time) / timer);
	}

	
	private int ConvertSlotToOption (int slot)
	{
		int numberOff = 0;
		for (int j=0; j<=slot; j++)
		{
			if (!options[j].isOn)
			{
				numberOff ++;
			}
		}
		
		int i = slot + numberOff;
		
		while (!options[i].isOn && i < options.Count)
		{
			numberOff++;
			i = slot + numberOff;
		}
		
		return i;
	}
	
	public string GetOptionName (int slot)
	{
		int i = ConvertSlotToOption (slot);
		return options[i].label;
	}
	
	
	public void SetOption (int i, bool flag, bool isLocked)
	{
		if (!options[i].isLocked)
		{
			options[i].isLocked = isLocked;
			options[i].isOn = flag;
		}
	}
	
	
	public int GetCount ()
	{
		int numberOn = 0;
		foreach (ButtonDialog _option in options)
		{
			if (_option.isOn)
			{
				numberOn ++;
			}
		}
		return numberOn;
	}
	
	
	public List<bool> GetOptionStates ()
	{
		List<bool> states = new List<bool>();
		foreach (ButtonDialog _option in options)
		{
			states.Add (_option.isOn);
		}
	
		return states;
	}
	
	
	public List<bool> GetOptionLocks ()
	{
		List<bool> locks = new List<bool>();
		foreach (ButtonDialog _option in options)
		{
			locks.Add (_option.isLocked);
		}
	
		return locks;
	}
	
	
	public void SetOptionStates (List<bool> states)
	{
		int i=0;
		foreach (ButtonDialog _option in options)
		{
			_option.isOn = states[i];
			i++;
		}
	}
	
	
	public void SetOptionLocks (List<bool> locks)
	{
		int i=0;
		foreach (ButtonDialog _option in options)
		{
			_option.isLocked = locks[i];
			i++;
		}
	}
	
	
	private void OnDestroy ()
	{
		playerInput = null;
	}

}
