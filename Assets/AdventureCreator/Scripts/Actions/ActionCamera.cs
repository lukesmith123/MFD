/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionCamera.cs"
 * 
 *	This action controls the MainCamera's "activeCamera",
 *	i.e., which GameCamera it is attached to.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionCamera : Action
{
	
	public GameCamera linkedCamera;
	public float transitionTime;
	public MoveMethod moveMethod;
	public bool returnToLast;
	
	
	public ActionCamera ()
	{
		this.isDisplayed = true;
		title = "Camera: Switch";
	}
	
	
	override public float Run ()
	{

		if (!isRunning)
		{
			isRunning = true;
			
			MainCamera mainCam = GameObject.FindWithTag (Tags.mainCamera).GetComponent <MainCamera>();
			
			if (mainCam)
			{
				_Camera cam = linkedCamera;
				
				if (returnToLast && mainCam.lastNavCamera)
				{
					cam = (_Camera) mainCam.lastNavCamera;
				}
				
				if (cam)
				{
					if (mainCam.attachedCamera != cam)
					{
						mainCam.SetGameCamera (cam);
						
						if (transitionTime > 0f)
						{
							mainCam.SmoothChange (transitionTime, moveMethod);
							
							if (willWait)
							{
								return (transitionTime);
							}
						}
						else
						{
							if (!returnToLast)
							{
								linkedCamera.MoveCameraInstant ();
							}
							mainCam.SnapToAttached ();
						}
					}
				}
			}
		}
		else
		{
			isRunning = false;
		}
		
		return 0f;

	}

	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		returnToLast = EditorGUILayout.Toggle ("Return to last gameplay?", returnToLast);
		
		if (!returnToLast)
		{
			linkedCamera = (GameCamera) EditorGUILayout.ObjectField ("New camera:", linkedCamera, typeof(GameCamera), true);
		}
			
		transitionTime = EditorGUILayout.Slider ("Transition time:", transitionTime, 0, 10f);
		
		if (transitionTime > 0f)
		{
			moveMethod = (MoveMethod) EditorGUILayout.EnumPopup ("Move method:", moveMethod);
			willWait = EditorGUILayout.Toggle ("Pause until finish?", willWait);
		}
		
		AfterRunningOption ();
	}
	
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		if (linkedCamera && !returnToLast)
		{
			labelAdd = " (" + linkedCamera.name + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}