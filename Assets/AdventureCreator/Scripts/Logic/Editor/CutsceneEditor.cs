using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Cutscene))]

[System.Serializable]
public class CutsceneEditor : ActionListEditor
{

	public override void OnInspectorGUI()
	{
		Cutscene _target = (Cutscene) target;
		
		_target.triggerTime = EditorGUILayout.Slider ("Start delay (s):", _target.triggerTime, 0f, 10f);
		_target.autosaveAfter = EditorGUILayout.Toggle ("Auto-save after running?", _target.autosaveAfter);

		DrawSharedElements ();

		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}
    }
}