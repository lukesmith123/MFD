/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionInventorySet.cs"
 * 
 *	This action is used to set the value of integer and boolean Variables, defined in the Variables Manager.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionVarSet : Action
{
	public int variableID;
	public int variableNumber;
	
	public int intValue;
	public bool isCumulative = false;	
	
	public BoolValue boolValue;
	
	private VariablesManager variablesManager;

	#if UNITY_EDITOR
	private static GUILayoutOption
		intWidth = GUILayout.MaxWidth (60);
	#endif
	
	
	public ActionVarSet ()
	{
		this.isDisplayed = true;
		title = "Variable: Set";
	}

	
	override public float Run ()
	{
		RuntimeVariables runtimeVariables = GameObject.FindWithTag(Tags.persistentEngine).GetComponent <RuntimeVariables>();
		
		if (runtimeVariables)
		{
			if (variableID != -1 && runtimeVariables.localVars.Count > 0)
			{
				if (runtimeVariables.GetVarType (variableID) == VariableType.Integer)
				{
					runtimeVariables.SetValue (variableID, intValue, isCumulative);
				}
				else
				{
					runtimeVariables.SetValue (variableID, (int) boolValue, false);
				}
			}
		}
		
		return 0f;
	}
	
	
	#if UNITY_EDITOR
	
	override public void ShowGUI ()
	{
		if (!variablesManager)
		{
			variablesManager = AdvGame.GetReferences ().variablesManager;
		}
		
		if (variablesManager)
		{
			// Create a string List of the field's names (for the PopUp box)
			List<string> labelList = new List<string>();
			
			int i = 0;
			variableNumber = -1;
			
			if (variablesManager.vars.Count > 0)
			{
			
				foreach (GVar _var in variablesManager.vars)
				{
					labelList.Add (_var.label);
					
					// If a GlobalVar variable has been removed, make sure selected variable is still valid
					if (_var.id == variableID)
					{
						variableNumber = i;
					}
					
					i ++;
				}
				
				if (variableNumber == -1)
				{
					// Wasn't found (variable was deleted?), so revert to zero
					Debug.LogWarning ("Previously chosen variable no longer exists!");
					variableNumber = 0;
					variableID = 0;
				}
		
				
				EditorGUILayout.BeginHorizontal();
				
				variableNumber = EditorGUILayout.Popup (variableNumber, labelList.ToArray());
				variableID = variablesManager.vars[variableNumber].id;
				
				if (isCumulative)
				{
					EditorGUILayout.LabelField ("+=", intWidth);
				}
				else
				{
					EditorGUILayout.LabelField ("=", intWidth);
				}
				
				if (variablesManager.vars[variableNumber].type == VariableType.Boolean)
				{
					boolValue = (BoolValue) EditorGUILayout.EnumPopup (boolValue, intWidth);
					EditorGUILayout.EndHorizontal();
					isCumulative = false;
				}
				else
				{
					intValue = EditorGUILayout.IntField (intValue, intWidth);
					EditorGUILayout.EndHorizontal();
					isCumulative = EditorGUILayout.Toggle ("Is cumulative?", isCumulative);
				}
				
				AfterRunningOption ();
			}
			else
			{
				EditorGUILayout.LabelField ("No global variables exist!");
				variableID = -1;
				variableNumber = -1;
			}
		}
	}

	#endif

}