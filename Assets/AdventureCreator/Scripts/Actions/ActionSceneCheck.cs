/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionSceneCheck.cs"
 * 
 *	This action checks the player's last-visited scene,
 *	useful for running specific "player enters the room" cutscenes.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionSceneCheck : Action
{
	
	public int sceneNumber;
	public enum IntCondition { EqualTo, NotEqualTo };
	public IntCondition intCondition;
	
	public ResultAction resultActionTrue;
	public ResultAction resultActionFail;

	public int skipActionTrue;
	public Cutscene linkedCutsceneTrue;
	
	public int skipActionFail;
	public Cutscene linkedCutsceneFail;
	
	
	public ActionSceneCheck ()
	{
		this.isDisplayed = true;
		title = "Engine: Check previous scene";
	}

	
	override public int End ()
	{
		bool result = false;
		result = CheckCondition ();

		if (result)
		{
			if (resultActionTrue == ResultAction.Continue)
			{
				return 0;
			}
			
			else if (resultActionTrue == ResultAction.Stop)
			{
				return -1;
			}
			
			else if (resultActionTrue == ResultAction.Skip)
			{
				return (skipActionTrue);
			}
			
			else if (resultActionTrue == ResultAction.RunCutscene)
			{
				if (linkedCutsceneTrue)
				{
					linkedCutsceneTrue.SendMessage ("Interact");
				}
				
				return -2;
			}
		}
		else
		{
			if (resultActionFail == ResultAction.Continue)
			{
				return 0;
			}
			
			else if (resultActionFail == ResultAction.Stop)
			{
				return -1;
			}
			
			else if  (resultActionFail == ResultAction.Skip)
			{
				return (skipActionFail);						
			}
			
			else if (resultActionFail == ResultAction.RunCutscene)
			{
				if (linkedCutsceneFail)
				{
					linkedCutsceneFail.SendMessage ("Interact");
				}
				
				return -2;
			}
		}
		
		return 0;
	}
	
	
	private bool CheckCondition ()
	{
		SceneChanger sceneChanger = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SceneChanger>();

		int actualSceneNumber = sceneChanger.previousScene;
	
		if (intCondition == IntCondition.EqualTo)
		{
			if (actualSceneNumber == sceneNumber)
			{
				return true;
			}
		}
		
		else if (intCondition == IntCondition.NotEqualTo)
		{
			if (actualSceneNumber != sceneNumber)
			{
				return true;
			}
		}
		
		return false;
	}

	
	#if UNITY_EDITOR

	public void ShowGUI (int lowerValue, int upperValue)
	{
		
		EditorGUILayout.BeginHorizontal();
		
			EditorGUILayout.LabelField ("Previous scene:");
			intCondition = (IntCondition) EditorGUILayout.EnumPopup (intCondition);
			sceneNumber = EditorGUILayout.IntField (sceneNumber);
		
		EditorGUILayout.EndHorizontal();
		
		if (lowerValue > upperValue || lowerValue == upperValue)
		{
			lowerValue = upperValue;
		}

		resultActionTrue = (Action.ResultAction) EditorGUILayout.EnumPopup("If condition is met:", (Action.ResultAction) resultActionTrue);
		if (resultActionTrue == Action.ResultAction.RunCutscene)
		{
			linkedCutsceneTrue = (Cutscene) EditorGUILayout.ObjectField ("Cutscene to run:", linkedCutsceneTrue, typeof (Cutscene), true);
		}
		else if (resultActionTrue == Action.ResultAction.Skip)
		{
			skipActionTrue = EditorGUILayout.IntSlider ("Action # to skip to:", skipActionTrue, lowerValue, upperValue);
		}
		
		resultActionFail = (Action.ResultAction) EditorGUILayout.EnumPopup("If condition is not met:", (Action.ResultAction) resultActionFail);
		if (resultActionFail == Action.ResultAction.RunCutscene)
		{
			linkedCutsceneFail = (Cutscene) EditorGUILayout.ObjectField ("Cutscene to run:", linkedCutsceneFail, typeof (Cutscene), true);
		}
		else if (resultActionFail == Action.ResultAction.Skip)
		{
			skipActionFail = EditorGUILayout.IntSlider ("Action # to skip to:", skipActionFail, lowerValue, upperValue);
		}
	}
	
	#endif

}