/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionTimescale.cs"
 * 
 *	This action modifies the speed at which the game runs at.
 *	It can be used for slow-motion effects during both cutscenes and gameplay.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionTimescale : Action
{
	
	public float timeScale;
	
	
	public ActionTimescale ()
	{
		this.isDisplayed = true;
		title = "Engine: Change timescale";
	}
	
	
	override public float Run ()
	{
		if (timeScale >= 0f)
		{
			PlayerInput playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
			playerInput.timeScale = timeScale;
		}
		
		return 0f;
	}
	
	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		timeScale = EditorGUILayout.Slider ("Timescale:", timeScale, 0f, 1f);
		
		AfterRunningOption ();
	}
	
	#endif

}