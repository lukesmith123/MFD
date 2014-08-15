using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AC_Trigger))]

[System.Serializable]
public class AC_TriggerEditor : CutsceneEditor
{
	private string[] Options = { "On enter", "Continuous" };

	public override void OnInspectorGUI()
    {
		AC_Trigger _target = (AC_Trigger) target;
   
		_target.triggerType = EditorGUILayout.Popup ("Type:", _target.triggerType, Options);
		
		// Draw all GUI elements that buttons and triggers share
		DrawSharedElements ();

		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}
    }

}
