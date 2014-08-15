/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionCharFace.cs"
 * 
 *	This action is used to make characters turn to face GameObjects.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionCharFace : Action
{
	
	public bool isInstant;
	public Char charToMove;
	public GameObject faceObject;
	public bool copyRotation;
	public bool facePlayer;
	
	public bool isPlayer;
	public bool lookUpDown;

	public ActionCharFace ()
	{
		this.isDisplayed = true;
		title = "Character: Face object";
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
			else if (facePlayer)
			{
				faceObject = GameObject.FindWithTag (Tags.player);
			}

			if (charToMove && faceObject)
			{
				FirstPersonCamera firstPersonCamera = null;
				if (GameObject.FindWithTag (Tags.firstPersonCamera) && GameObject.FindWithTag (Tags.firstPersonCamera).GetComponent <FirstPersonCamera>())
				{
					firstPersonCamera = GameObject.FindWithTag (Tags.firstPersonCamera).GetComponent <FirstPersonCamera>();
				}
				
				Vector3 lookVector = faceObject.transform.position - charToMove.transform.position;
				if (copyRotation)
				{
					lookVector = faceObject.transform.forward;
				}
				lookVector.y = 0;
				
				if (lookUpDown)
				{
					if (firstPersonCamera)
					{
						firstPersonCamera.SetTilt (faceObject.transform.position, isInstant);
					}
					else
					{
						Debug.LogWarning ("Cannot tilt player, since no FirstPersonCamera script was found");
					}
				}
				
				if (isInstant)
				{
					charToMove.SetLookDirection (lookVector, true);
					return 0f;
				}
				else
				{
					charToMove.Halt ();
					charToMove.SetLookDirection (lookVector, false);
					
					if (willWait)
					{
						return (defaultPauseTime);
					}
					else
					{
						return 0f;
					}
				}
			}

			return 0f;
		}
		else
		{
			if (!charToMove.IsTurning ())
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
		isPlayer = EditorGUILayout.Toggle ("Affect Player?", isPlayer);
		if (!isPlayer)
		{
			charToMove = (Char) EditorGUILayout.ObjectField ("Character to turn:", charToMove, typeof(Char), true);
			facePlayer = EditorGUILayout.Toggle ("Face player?", facePlayer);
		}
		else
		{
			facePlayer = false;
			lookUpDown = EditorGUILayout.Toggle ("1st-person head tilt?", lookUpDown);
		}
			
		if (!facePlayer)
		{
			faceObject = (GameObject) EditorGUILayout.ObjectField ("Object to face:", faceObject, typeof(GameObject), true);
		}
		copyRotation = EditorGUILayout.Toggle ("Use object's rotation?", copyRotation);

		isInstant = EditorGUILayout.Toggle ("Is instant?", isInstant);
		if (!isInstant)
		{
			willWait = EditorGUILayout.Toggle ("Pause until finish?", willWait);
		}

		AfterRunningOption ();
	}

	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (charToMove && faceObject)
		{
			labelAdd = " (" + charToMove.name + " to " + faceObject.name + ")";
		}
		else if (isPlayer && faceObject)
		{
			labelAdd = " (Player to " + faceObject.name + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}