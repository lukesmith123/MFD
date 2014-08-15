using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (ConstantID), true)]
public class ConstantIDEditor : Editor
{

	public override void OnInspectorGUI()
    {
		SharedGUI ();
	}
	
	
	protected void SharedGUI()
	{
		ConstantID _target = (ConstantID) target;
		
		if (!_target.gameObject.activeInHierarchy)
		{
			_target.constantID = 0;
		}
		
		EditorGUILayout.LabelField ("ID: " + _target.constantID);
		
		EditorUtility.SetDirty(_target);
	}
}
