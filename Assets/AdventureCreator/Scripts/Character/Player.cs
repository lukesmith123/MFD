/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Player.cs"
 * 
 *	This is attached to the Player GameObject, which must be tagged as Player.
 * 
 */

using UnityEngine;
using System.Collections;

public class Player : Char
{
	
	[HideInInspector] public bool lockedPath;
		
	private SettingsManager settingsManager;
	
	void Awake ()
	{
		settingsManager = AdvGame.GetReferences ().settingsManager;
		DontDestroyOnLoad (this);
	}
	
	
	new private void FixedUpdate ()
	{
		if (activePath && !pausePath)
		{
			if (stateHandler.gameState == GameState.Cutscene || settingsManager.controlStyle == ControlStyle.PointAndClick)
			{
				charState = CharState.Move;
			}
			
			if (!lockedPath)
			{
				CheckIfStuck ();
			}
		}
		
		base.FixedUpdate ();
	}
	
	
	new public void EndPath ()
	{
		lockedPath = false;
		
		base.EndPath ();
	}
	
	
	public void SetLockedPath (Paths pathOb)
	{
		// Ignore if using "point and click" or first person methods
		if (settingsManager)
		{
			if (settingsManager.controlStyle == ControlStyle.Direct && settingsManager.inputType != InputType.TouchScreen)
			{
				lockedPath = true;
				
				if (pathOb.pathSpeed == PathSpeed.Run)
				{
					isRunning = true;
				}
				else
				{
					isRunning = false;
				}
			
				if (pathOb.affectY)
				{
					transform.position = pathOb.transform.position;
				}
				else
				{
					transform.position = new Vector3 (pathOb.transform.position.x, transform.position.y, pathOb.transform.position.z);
				}
		
				activePath = pathOb;
				targetNode = 1;
				charState = CharState.Idle;
			}
			else
			{
				Debug.LogWarning ("Path-constrained player movement is only available with Direct control for Point And Click and Controller input only.");
			}
		}
	}

}