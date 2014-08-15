/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionCharHold.cs"
 * 
 *	This action parents a GameObject to a character's hand.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionCharHold : Action
{
	
	public GameObject objectToHold;
	public bool isPlayer;
	public Char _char;
	public bool rotate90;
	
	public enum Hand { Left, Right };
	public Hand hand;
	
	
	public ActionCharHold ()
	{
		this.isDisplayed = true;
		title = "Character: Hold object";
	}
	
	
	override public float Run ()
	{
		if (isPlayer)
		{
			_char = GameObject.FindWithTag (Tags.player).GetComponent <Player>();
		}
		
		if (_char && objectToHold)
		{
			Transform handTransform;
			
			if (hand == Hand.Left)
			{
				handTransform = _char.leftHandBone;
			}
			else
			{
				handTransform = _char.rightHandBone;
			}
			
			if (handTransform)
			{
				objectToHold.transform.parent = handTransform;
				objectToHold.transform.localPosition = Vector3.zero;
				
				if (rotate90)
				{
					objectToHold.transform.localEulerAngles = new Vector3 (0f, 0f, 90f);
				}
				else
				{
					objectToHold.transform.localEulerAngles = Vector3.zero;
				}
			}
			else
			{
				Debug.Log ("Cannot parent object - no hand bone found.");
			}
		}
		
		return 0f;
	}
	
	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		isPlayer = EditorGUILayout.Toggle ("Is player?", isPlayer);
		if (!isPlayer)
		{
			_char = (Char) EditorGUILayout.ObjectField ("Character:", _char, typeof (Char), true);
		}
		
		objectToHold = (GameObject) EditorGUILayout.ObjectField ("Object to hold:", objectToHold, typeof (GameObject), true);
		hand = (Hand) EditorGUILayout.EnumPopup ("Hand:", hand);
		rotate90 = EditorGUILayout.Toggle ("Rotate 90 degrees?", rotate90);
		
		AfterRunningOption ();
	}
	

	public override string SetLabel ()
	{
		string labelAdd = "";
		
		if (_char && objectToHold)
		{
			labelAdd = "(" + _char.name + " hold " + objectToHold.name + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}