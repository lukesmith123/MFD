/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionVarCheck.cs"
 * 
 *	This action checks to see if a Variable has been assigned a certain value,
 *	and performs something accordingly.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionVarCheck : Action
{
	
	public int variableID;
	public int variableNumber;
	
	public int intValue;
	public enum IntCondition { EqualTo, NotEqualTo, LessThan, MoreThan };
	public IntCondition intCondition;
	public bool isAdditive = false;
	
	public BoolValue boolValue;
	public enum BoolCondition { EqualTo, NotEqualTo };
	public BoolCondition boolCondition;
	
	public ResultAction resultActionTrue;
	public ResultAction resultActionFail;

	public int skipActionTrue;
	public Cutscene linkedCutsceneTrue;
	
	public int skipActionFail;
	public Cutscene linkedCutsceneFail;
	
	private VariablesManager variablesManager;
	
	
	public ActionVarCheck ()
	{
		this.isDisplayed = true;
		title = "Variable: Check";
	}

	
	override public int End ()
	{
		RuntimeVariables runtimeVariables = GameObject.FindWithTag(Tags.persistentEngine).GetComponent <RuntimeVariables>();
			
		if (runtimeVariables && variableID != -1)
		{
		
			bool result = false;
			result = CheckCondition (runtimeVariables.localVars[variableNumber]);
			
			if (result)
			{
				if (resultActionTrue == ResultAction.Continue)
				{
					return 0;
				}
				
				else if (resultActionTrue == ResultAction.Stop)
				{
					return -1;
				}
				
				else if (resultActionTrue == ResultAction.Skip)
				{
					return (skipActionTrue);
				}
				
				else if (resultActionTrue == ResultAction.RunCutscene)
				{
					if (linkedCutsceneTrue)
					{
						linkedCutsceneTrue.SendMessage ("Interact");
					}
					
					return -2;
				}
			}
			
			else
			{
				if (resultActionFail == ResultAction.Continue)
				{
					return 0;
				}
				
				else if (resultActionFail == ResultAction.Stop)
				{
					return -1;
				}
				
				else if  (resultActionFail == ResultAction.Skip)
				{
					return (skipActionFail);						
				}
				
				else if (resultActionFail == ResultAction.RunCutscene)
				{
					if (linkedCutsceneFail)
					{
						linkedCutsceneFail.SendMessage ("Interact");
					}
					
					return -2;
				}
			}

		}
		
		return 0;
	}
	
	
	private bool CheckCondition (GVar _var)
	{
		int fieldValue = _var.val;
		
		if (_var.type == VariableType.Boolean)
		{
			if (boolCondition == BoolCondition.EqualTo)
			{
				if (fieldValue == (int) boolValue)
				{
					return true;
				}
			}
			else
			{
				if (fieldValue != (int) boolValue)
				{
					return true;
				}
			}
		}

		else
		{
			if (intCondition == IntCondition.EqualTo)
			{
				if (fieldValue == intValue)
				{
					return true;
				}
			}
			
			else if (intCondition == IntCondition.NotEqualTo)
			{
				if (fieldValue != intValue)
				{
					return true;
				}
			}
			
			else if (intCondition == IntCondition.LessThan)
			{
				if (fieldValue < intValue)
				{
					return true;
				}
			}
			
			else if (intCondition == IntCondition.MoreThan)
			{
				if (fieldValue > intValue)
				{
					return true;
				}
			}
		}
		
		return false;
	}

	
	#if UNITY_EDITOR

	public void ShowGUI (int lowerValue, int upperValue)
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
					
					i++;
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
					
					if (variablesManager.vars[variableNumber].type == VariableType.Boolean)
					{
						boolCondition = (BoolCondition) EditorGUILayout.EnumPopup(boolCondition);
						boolValue = (BoolValue) EditorGUILayout.EnumPopup (boolValue);
					}
					else
					{
						intCondition = (IntCondition) EditorGUILayout.EnumPopup (intCondition);
						intValue = EditorGUILayout.IntField (intValue);
					}
				
				EditorGUILayout.EndHorizontal();
				
				if (lowerValue > upperValue || lowerValue == upperValue)
				{
					lowerValue = upperValue;
				}
		
				resultActionTrue = (Action.ResultAction) EditorGUILayout.EnumPopup("If condition is met:", (Action.ResultAction) resultActionTrue);
				if (resultActionTrue == Action.ResultAction.RunCutscene)
				{
					linkedCutsceneTrue = (Cutscene) EditorGUILayout.ObjectField ("Cutscene to run:", linkedCutsceneTrue, typeof (Cutscene), true);
				}
				else if (resultActionTrue == Action.ResultAction.Skip)
				{
					skipActionTrue = EditorGUILayout.IntSlider ("Action # to skip to:", skipActionTrue, lowerValue, upperValue);
				}
				
				resultActionFail = (Action.ResultAction) EditorGUILayout.EnumPopup("If condition is not met:", (Action.ResultAction) resultActionFail);
				if (resultActionFail == Action.ResultAction.RunCutscene)
				{
					linkedCutsceneFail = (Cutscene) EditorGUILayout.ObjectField ("Cutscene to run:", linkedCutsceneFail, typeof (Cutscene), true);
				}
				else if (resultActionFail == Action.ResultAction.Skip)
				{
					skipActionFail = EditorGUILayout.IntSlider ("Action # to skip to:", skipActionFail, lowerValue, upperValue);
				}
								
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