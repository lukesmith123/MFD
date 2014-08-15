using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Conversation))]
public class ConversationEditor : Editor
{
	private static GUIContent
		deleteContent = new GUIContent("-", "Delete this option");

	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth(20f);
	
	private Conversation _target;
 
	
    public void OnEnable()
    {
        _target = (Conversation) target;
    }

	
	public override void OnInspectorGUI()
    {

		_target.isTimed = EditorGUILayout.Toggle ("Is timed?", _target.isTimed);
		
		if (_target.isTimed)
		{
			_target.timer = EditorGUILayout.FloatField ("Timer length (s):", _target.timer);
		}
		
		foreach (ButtonDialog option in _target.options)
		{
			EditorGUILayout.BeginVertical ("Button");
				EditorGUILayout.BeginHorizontal ();
					option.label = EditorGUILayout.TextField ("Label", option.label);
					if (GUILayout.Button (deleteContent, EditorStyles.miniButtonRight, buttonWidth))
					{
						Undo.RegisterSceneUndo ("Delete dialogue option");
						_target.options.Remove (option);
						break;
					}
				EditorGUILayout.EndHorizontal ();
			
				EditorGUILayout.BeginHorizontal ();
			
				option.dialogueOption = (DialogueOption) EditorGUILayout.ObjectField ("Interaction:", option.dialogueOption, typeof (DialogueOption), true);
			
				if (option.dialogueOption == null)
				{
					if (GUILayout.Button ("Auto-create", GUILayout.MaxWidth (90f)))
					{
						Undo.RegisterSceneUndo ("Auto-create dialogue option");
						DialogueOption newDialogueOption = AdvGame.GetReferences ().sceneManager.AddPrefab ("Logic", "DialogueOption", true, false, true).GetComponent <DialogueOption>();
						
						newDialogueOption.gameObject.name = AdvGame.UniqueName (_target.gameObject.name + "_Option");
						option.dialogueOption = newDialogueOption;
					}
				}
			
				EditorGUILayout.EndHorizontal ();
	
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Is enabled?", GUILayout.MaxWidth (90));
					option.isOn = EditorGUILayout.Toggle (option.isOn, buttonWidth);
					if (_target.isTimed)
					{
						if (_target.defaultOption == option)
						{
							EditorGUILayout.LabelField ("Default");
						}
						else
						{
							if (GUILayout.Button ("Make default", GUILayout.MaxWidth (80)))
							{
								Undo.RegisterUndo (_target, "Change default conversation option");
								_target.defaultOption = option;
							}
						}
					}
				EditorGUILayout.EndHorizontal ();
			
				option.returnToConversation = EditorGUILayout.Toggle ("Return to conversation?", option.returnToConversation);
			
			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space ();
		}
		
		if (GUILayout.Button ("Add new dialogue option"))
		{
			Undo.RegisterSceneUndo ("Create dialogue option");
			_target.options.Add (new ButtonDialog ());
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}

	}
	
}