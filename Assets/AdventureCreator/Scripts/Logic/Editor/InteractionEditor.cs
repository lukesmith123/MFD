using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Interaction))]

[System.Serializable]
public class InteractionEditor : CutsceneEditor
{

	public override void OnInspectorGUI()
    {
		Interaction _target = (Interaction) target;
		
		// Draw all GUI elements that buttons and triggers share
		DrawSharedElements ();

		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}

    }

}
