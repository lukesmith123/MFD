/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionCharMove.cs"
 * 
 *	This action moves characters by assinging them a Paths object.
 *	If a player is moved, the game will automatically pause.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionCharMove : Action
{
	
	public Paths movePath;
	public bool isPlayer;
	public Char charToMove;
	public bool doTeleport;
	public bool doStop;
	
	
	public ActionCharMove ()
	{
		this.isDisplayed = true;
		title = "Character: Move along path";
	}
	
	
	override public float Run ()
	{
		if (!isRunning)
		{
			isRunning = true;
			
			if (isPlayer)
			{
				charToMove = GameObject.FindWithTag (Tags.player).GetComponent <Player>();
			}
			
			if (charToMove)
			{
				if (doStop)
				{
					charToMove.EndPath ();
				}
				else
				{
					if (doTeleport)
					{
						charToMove.transform.position = movePath.transform.position;
						
						// Set rotation if there is more than one node
						if (movePath.nodes.Count > 1)
						{
							charToMove.SetLookDirection (movePath.nodes[1] - movePath.nodes[0], true);
						}
					}
					
					if (movePath)
					{
						if (isPlayer && movePath.pathType != PathType.ForwardOnly)
						{
							Debug.LogWarning ("Cannot move player along a non-forward only path, as this will create an indefinite cutscene.");
						}
						else
						{
							if (willWait && movePath.pathType != PathType.ForwardOnly)
							{
								willWait = false;
								Debug.LogWarning ("Cannot pause while character moves along a non-forward only path, as this will create an indefinite cutscene.");
							}
							
							charToMove.SetPath (movePath);
						
							if (willWait || isPlayer)
							{
								return defaultPauseTime;
							}
						}
					}
				}
			}

			return 0f;
		}
		else
		{
			if (charToMove.GetPath () != movePath)
			{
				isRunning = false;
				return 0f;
			}
			else
			{
				return (defaultPauseTime);
			}
		}
	}

	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		isPlayer = EditorGUILayout.Toggle ("Is Player?", isPlayer);

		if (!isPlayer)
		{
			charToMove = (Char) EditorGUILayout.ObjectField ("Character to move:", charToMove, typeof(Char), true);
		}

		doStop = EditorGUILayout.Toggle ("Stop moving?", doStop);
		if (!doStop)
		{
			movePath = (Paths) EditorGUILayout.ObjectField ("Path to follow:", movePath, typeof(Paths), true);
			doTeleport = EditorGUILayout.Toggle ("Teleport to start?", doTeleport);
			
			if (!isPlayer)
			{
				willWait = EditorGUILayout.Toggle ("Pause until finish?", willWait);
			}
		}
		
		AfterRunningOption ();
	}
	
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (charToMove && movePath)
		{
			labelAdd = " (" + charToMove.name + " to " + movePath.name + ")";
		}
		else if (isPlayer && movePath)
		{
			labelAdd = " (Player to " + movePath.name + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}