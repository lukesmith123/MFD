/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"VariablesManager.cs"
 * 
 *	This script handles the "Variables" tab of the main wizard.
 *	Boolean and integer, which can be used regardless of scene, are defined here.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class VariablesManager : ScriptableObject
{

	public List<GVar> vars = new List<GVar>();
	
	
	#if UNITY_EDITOR

	private string[] boolType = {"False", "True"};
	
	private static GUIContent
		insertContent = new GUIContent("+", "Insert variable"),
		deleteContent = new GUIContent("-", "Delete variable");

	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth (20f),
		valueWidth = GUILayout.MaxWidth (60f);
	
	
	public void ShowGUI ()
	{
		// List variables
		foreach (GVar _var in vars)
		{
			EditorGUILayout.BeginVertical("Button");
				EditorGUILayout.BeginHorizontal ();
				
					_var.type = (VariableType) EditorGUILayout.EnumPopup (_var.type);
					_var.label = EditorGUILayout.TextField (_var.label);
					
					if (_var.type == VariableType.Boolean)
					{
						if (_var.val != 1)
							_var.val = 0;
						_var.val = EditorGUILayout.Popup (_var.val, boolType, valueWidth);
					}
					else
					{
						_var.val = EditorGUILayout.IntField (_var.val, valueWidth);
					}
					
					if (GUILayout.Button (insertContent, EditorStyles.miniButtonLeft, buttonWidth))
					{
						Undo.RegisterUndo (this, "Add variable");
						int position = vars.IndexOf (_var) + 1;
						vars.Insert (position, new GVar (GetIDArray ()));
						break;
					}
					if (GUILayout.Button (deleteContent, EditorStyles.miniButtonRight, buttonWidth))
					{
						Undo.RegisterUndo (this, "Delete variable: " + _var.label);
						vars.Remove (_var);
						break;
					}
			
				EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical();
		}

		
		if (GUILayout.Button("Create new variable"))
		{
			Undo.RegisterUndo (this, "Add variable");
			vars.Add (new GVar (GetIDArray ()));
		}
		

		if (GUI.changed)
		{
			EditorUtility.SetDirty (this);
		}
	}

	#endif

	
	private int[] GetIDArray ()
	{
		// Returns a list of id's in the list
		
		List<int> idArray = new List<int>();
		
		foreach (GVar variable in vars)
		{
			idArray.Add (variable.id);
		}
		
		idArray.Sort ();
		return idArray.ToArray ();
	}

}