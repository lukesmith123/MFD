using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(InvActionList))]

[System.Serializable]
public class InvActionListEditor : Editor
{
	private int typeNumber = -1;
	
	private ActionsManager actionsManager;
	
	private static GUILayoutOption
		labelWidth = GUILayout.MaxWidth(50f);

	
	public override void OnInspectorGUI()
	{
		InvActionList _target = (InvActionList) target;
		
		actionsManager = AdvGame.GetReferences ().actionsManager;
			
		for (int i=0; i<_target.actions.Count; i++)
		{
			EditorGUILayout.BeginVertical("Button");
			
				string actionLabel = " " + (i).ToString() + ": " + _target.actions[i].title + _target.actions[i].SetLabel ();
				
				_target.actions[i].isDisplayed = EditorGUILayout.Foldout(_target.actions[i].isDisplayed, actionLabel);
	
				if (_target.actions[i].isDisplayed)
				{
					EditorGUILayout.Space ();
					ShowActionGUI (_target.actions[i], i, _target.actions.Count);
					EditorGUILayout.BeginHorizontal();
		
						if (i > 0)
						{
							if (GUILayout.Button("Move up"))
							{
								_target.actions = AdvGame.SwapActions (_target.actions, i, i-1);
							}
						}
		
						if (GUILayout.Button ("Delete"))
						{
							DeleteAction (_target.actions[i]);
						}
		
						if (i < _target.actions.Count-1)
						{
							if (GUILayout.Button("Move down"))
							{
								_target.actions = AdvGame.SwapActions (_target.actions, i, i+1);
							}
						}
		
					EditorGUILayout.EndHorizontal ();
				}
			

			EditorGUILayout.EndVertical();
			EditorGUILayout.Space ();
		}
		
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Type:", labelWidth);
			
			if (typeNumber == -1)
			{
				typeNumber = actionsManager.defaultClass;
			}
			
			typeNumber = EditorGUILayout.Popup(typeNumber, actionsManager.GetActionTitles ());
			
			if (GUILayout.Button("Add new"))
			{
				AddAction (actionsManager.GetActionName (typeNumber));
			}
		EditorGUILayout.EndHorizontal ();
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}
	}
	
	
	private void ShowActionGUI (Action action, int i, int count)
	{
		if (action is ActionDialogOption)
		{
			ActionDialogOption tempAction = (ActionDialogOption) action;
			tempAction.ShowGUI ();
		}
		
		else if (action is ActionInventoryCheck)
		{
			ActionInventoryCheck tempAction = (ActionInventoryCheck) action;
			tempAction.ShowGUI (i+2, count-1);
 		}
		
		else if (action is ActionSendMessage)
		{
			ActionSendMessage tempAction = (ActionSendMessage) action;
			tempAction.ShowGUI ();
		}
		
		else if (action is ActionVarCheck)
		{
			ActionVarCheck tempAction = (ActionVarCheck) action;
			tempAction.ShowGUI (i+2, count-1);
		}
		
		else
		{
			action.ShowGUI ();
		}
	}
	

	private void DeleteAction (Action action)
	{
		UnityEngine.Object.DestroyImmediate(action, true);
		AssetDatabase.SaveAssets();
		
		InvActionList _target = (InvActionList) target;
		_target.actions.Remove (action);
	}
	
	private void AddAction (string className)
	{
		InvActionList _target = (InvActionList) target;
		
		List<int> idArray = new List<int>();
		
		foreach (Action _action in _target.actions)
		{
			idArray.Add (_action.id);
		}

		idArray.Sort ();
		
		Action newAction = (Action) CreateInstance (className);
		
		// Update id based on array
		foreach (int _id in idArray.ToArray())
		{
			if (newAction.id == _id)
				newAction.id ++;
		}
		newAction.name = "Action " + newAction.id.ToString ();
		
		_target.actions.Add (newAction);
		AssetDatabase.AddObjectToAsset (newAction, _target);
		AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath(newAction));
	}

}