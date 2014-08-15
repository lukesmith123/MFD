using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor (typeof (ActionList))]

[System.Serializable]
public class ActionListEditor : Editor
{

	private int typeNumber;
	
	private ActionsManager actionsManager;
	

	public override void OnInspectorGUI()
	{
		ActionList _target = (ActionList) target;
		
		DrawSharedElements ();

		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}
	}

	
	protected void DrawSharedElements ()
	{
		if (AdvGame.GetReferences () == null)
		{
			Debug.LogError ("A References file is required - please use the Adventure Creator window to create one.");
			EditorGUILayout.LabelField ("No References file found!");
		}
		else
		{
			actionsManager = AdvGame.GetReferences ().actionsManager;
				
			ActionList _target = (ActionList) target;
			
			if (actionsManager)
			{
				
				int numActions = _target.actions.Count;
				if (numActions < 1)
				{
					numActions = 1;
					
					string defaultAction = actionsManager.GetDefaultAction ();
					
					_target.actions.Add ((Action) CreateInstance(defaultAction));
				}
				
				EditorGUILayout.BeginHorizontal ();
				
					if (GUILayout.Button ("Expand all"))
					{
						Undo.RegisterUndo (_target, "Expand actions");
						
						foreach (Action action in _target.actions)
						{
							action.isDisplayed = true;
						}
					}
				
					if (GUILayout.Button ("Collapse all"))
					{
						Undo.RegisterUndo (_target, "Collapse actions");
						
						foreach (Action action in _target.actions)
						{
							action.isDisplayed = false;
						}
					}
				
				EditorGUILayout.EndHorizontal ();
				
				for (int i=0; i<_target.actions.Count; i++)
				{
					EditorGUILayout.BeginVertical("Button");
						typeNumber = GetTypeNumber (i);
						
						string actionLabel = " " + (i).ToString() + ": " + _target.actions[i].title + _target.actions[i].SetLabel ();
						
						_target.actions[i].isDisplayed = EditorGUILayout.Foldout(_target.actions[i].isDisplayed, actionLabel);
			
						if (_target.actions[i].isDisplayed)
						{
							typeNumber = EditorGUILayout.Popup("Action type:", typeNumber, actionsManager.GetActionTitles ());
							EditorGUILayout.Space ();
										
							// Rebuild constructor if Subclass and type string do not match
							if (_target.actions[i].GetType().ToString() != actionsManager.GetActionName (typeNumber))
							{
								_target.actions[i] = RebuildAction (_target.actions[i], typeNumber);
							}
			
							ShowActionGUI (_target.actions[i], _target.gameObject, i, _target.actions.Count);
			
							EditorGUILayout.BeginHorizontal();
			
								if (i > 0)
								{
									if (GUILayout.Button("Move up"))
									{
										Undo.RegisterUndo (_target, "Move action up");
										_target.actions = AdvGame.SwapActions (_target.actions, i, i-1);
									}
								}
				
								if (i < _target.actions.Count-1)
								{
									if (GUILayout.Button("Insert new"))
									{
										Undo.RegisterUndo (_target, "Create action");
										
										numActions += 1;
										
										_target.actions = ResizeList (_target.actions, numActions);
										// Swap all elements up one
										for (int k = numActions-1; k > i+1; k--)
										{
											_target.actions = AdvGame.SwapActions (_target.actions, k, k-1);
										}
									}
								}
					
								if (_target.actions.Count > 1)
								{
									if (GUILayout.Button("Delete"))
									{
										Undo.RegisterUndo (_target, "Delete action");
										
										_target.actions.RemoveAt (i);
										numActions -= 1;
									}
								}
			
								if (i < _target.actions.Count-1)
								{
									if (GUILayout.Button("Move down"))
									{	
										Undo.RegisterUndo (_target, "Move action down");
										_target.actions = AdvGame.SwapActions (_target.actions, i, i+1);
									}
								}
				
							EditorGUILayout.EndHorizontal ();
						}
		
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space ();
				}
				
				if (GUILayout.Button("Add new action"))
				{
					Undo.RegisterUndo (_target, "Create action");
					numActions += 1;
				}
				
				_target.actions = ResizeList (_target.actions, numActions);
			}
		}
	}


	private int GetTypeNumber (int i)
	{
		ActionList _target = (ActionList) target;
		
		int number = 0;
		
		if (actionsManager)
		{
			for (int j=0; j<actionsManager.GetActionsSize(); j++)
			{
				try
				{
					if (_target.actions[i].GetType().ToString() == actionsManager.GetActionName(j))
					{
						number = j;
						break;
					}
				}
				 
				catch
				{
					string defaultAction = actionsManager.GetDefaultAction ();
					_target.actions[i] = (Action) CreateInstance (defaultAction);
				}
			}
		}
		
		return number;
	}	
	
	
	private void ShowActionGUI (Action action, GameObject _target, int i, int count)
	{

		if (action is ActionInventoryCheck)
		{
			ActionInventoryCheck tempAction = (ActionInventoryCheck) action;
			tempAction.ShowGUI (i+2, count-1);
 		}
		
		else if (action is ActionSendMessage)
		{
			ActionSendMessage tempAction = (ActionSendMessage) action;
			tempAction.ShowGUI (_target);
		}
		
		else if (action is ActionVarCheck)
		{
			ActionVarCheck tempAction = (ActionVarCheck) action;
			tempAction.ShowGUI (i+2, count-1);
		}
		
		else if (action is ActionSceneCheck)
		{
			ActionSceneCheck tempAction = (ActionSceneCheck) action;
			tempAction.ShowGUI (i+2, count-1);
		}
		
		else
		{
			action.ShowGUI ();
		}
	}
	
	
	private Action RebuildAction (Action action, int number)
	{
		actionsManager = AdvGame.GetReferences ().actionsManager;
		
		if (actionsManager)
		{
			string ClassName = actionsManager.GetActionName (number);
			
			if (action.GetType().ToString() != ClassName)
			{
				action = (Action) CreateInstance (ClassName);
			}
		}
		
		return action;
	}
	
	
	private List<Action> ResizeList (List<Action> list, int listSize)
	{
		actionsManager = AdvGame.GetReferences ().actionsManager;
		
		string defaultAction = "";
		
		if (actionsManager)
		{
			defaultAction = actionsManager.GetDefaultAction ();
		}

		if (list.Count < listSize)
		{
			// Increase size of list
			while (list.Count < listSize)
			{
				List<int> idArray = new List<int>();
		
				foreach (Action _action in list)
				{
					idArray.Add (_action.id);
				}
		
				idArray.Sort ();
				
				list.Add ((Action) CreateInstance (defaultAction));
				
				// Update id based on array
				foreach (int _id in idArray.ToArray())
				{
					if (list [list.Count -1].id == _id)
						list [list.Count -1].id ++;
				}
			}
		}
		else if (list.Count > listSize)
		{
			// Decrease size of list
			while (list.Count > listSize)
				list.RemoveAt (list.Count - 1);
		}
		
		return (list);
	}

}