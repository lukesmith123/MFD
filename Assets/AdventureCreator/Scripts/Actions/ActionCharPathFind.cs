/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionCharPathFind.cs"
 * 
 *	This action moves characters by generating a path to a specified point.
 *	If a player is moved, the game will automatically pause.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionCharPathFind : Action
{
	
	public Marker marker;
	public bool isPlayer;
	public Char charToMove;
	public PathSpeed speed;
	public bool pathFind = true;
	
	
	public ActionCharPathFind ()
	{
		this.isDisplayed = true;
		title = "Character: Move to point";
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
			
			if (charToMove && marker)
			{
				Paths path = charToMove.GetComponent <Paths>();
				if (path == null)
				{
					Debug.LogWarning ("Cannot move a character with no Paths component");
				}
				else
				{
					path.pathType = PathType.ForwardOnly;
					path.pathSpeed = speed;
					path.affectY = true;
					
					SceneSettings sceneSettings = null;
					if (GameObject.FindWithTag (Tags.gameEngine) && GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>())
					{
						sceneSettings = GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>();
					}
					
					Vector3[] pointArray;
					
					if (pathFind && sceneSettings && sceneSettings.navMesh && sceneSettings.navMesh.GetComponent <Collider>())
					{
						pointArray = sceneSettings.navMesh.GetPointsArray (charToMove.transform.position, marker.transform.position);
					}
					else
					{
						pointArray = new Vector3[] { marker.transform.position };
					}
					
					charToMove.MoveAlongPoints (pointArray, false);
				
					if (willWait || isPlayer)
					{
						return defaultPauseTime;
					}
				}
			}

			return 0f;
		}
		else
		{
			if (charToMove.GetPath () == null)
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
			charToMove = (Char) EditorGUILayout.ObjectField ("Character to move:", charToMove, typeof (Char), true);
		}

		marker = (Marker) EditorGUILayout.ObjectField ("Marker to move to:", marker, typeof (Marker), true);
		speed = (PathSpeed) EditorGUILayout.EnumPopup ("Move speed:" , speed);
		pathFind = EditorGUILayout.Toggle ("Pathfind?", pathFind);
		
		if (!isPlayer)
		{
			willWait = EditorGUILayout.Toggle ("Pause until finish?", willWait);
		}
		
		AfterRunningOption ();
	}
	
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (marker)
		{
			if (charToMove)
			{
				labelAdd = " (" + charToMove.name + " to " + marker.name + ")";
			}
			else if (isPlayer)
			{
				labelAdd = " (Player to " + marker.name + ")";
			}
		}
		
		return labelAdd;
	}

	#endif
	
}