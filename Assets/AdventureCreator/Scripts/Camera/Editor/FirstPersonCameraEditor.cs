using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (FirstPersonCamera))]

public class FirstPersonCameraEditor : Editor
{
	
	private static GUILayoutOption
		labelWidth = GUILayout.MaxWidth (40),
		intWidth = GUILayout.MaxWidth (160);
	
	
	public override void OnInspectorGUI ()
	{
		FirstPersonCamera _target = (FirstPersonCamera) target;
		
		EditorGUILayout.BeginVertical ("Button");
			EditorGUILayout.LabelField ("Constrain Y-rotation (degrees)");
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Min", labelWidth);
				_target.minY = EditorGUILayout.FloatField (_target.minY, intWidth);
				EditorGUILayout.LabelField ("Max", labelWidth);
				_target.maxY = EditorGUILayout.FloatField (_target.maxY, intWidth);
			EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.BeginVertical ("Button");
			_target.sensitivity = EditorGUILayout.Vector2Field ("Freelook sensitivity", _target.sensitivity);
		EditorGUILayout.EndVertical ();
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}
	}
	
}
