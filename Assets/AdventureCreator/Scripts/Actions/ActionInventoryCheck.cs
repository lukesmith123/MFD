/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionInventoryCheck.cs"
 * 
 *	This action checks to see if a particular inventory item
 *	is held by the player, and performs something accordingly.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionInventoryCheck : Action
{
	
	public int invID;
	private int invNumber;
	
	public bool doCount;
	public int intValue = 1;
	public enum IntCondition { EqualTo, NotEqualTo, LessThan, MoreThan };
	public IntCondition intCondition;
	
	public ResultAction resultActionTrue;
	public int skipActionTrue;
	public Cutscene linkedCutsceneTrue;
	
	public ResultAction resultActionFail;
	public int skipActionFail;
	public Cutscene linkedCutsceneFail;
	
	private InventoryManager inventoryManager;
	
	
	public ActionInventoryCheck ()
	{
		this.isDisplayed = true;
		title = "Inventory: Check";
	}
	
	
	override public int End ()
	{
		bool result = false;
		result = CheckCondition ();
			
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
				
				return -1;
			}
			
			return 0;
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
				
				return -1;
			}
		}
		
		return 0;
	}
	
	
	private bool CheckCondition ()
	{
		RuntimeInventory runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();
		
		int count = runtimeInventory.GetCount (invID);
		
		if (doCount)
		{
			if (intCondition == IntCondition.EqualTo)
			{
				if (count == intValue)
				{
					return true;
				}
			}
			
			else if (intCondition == IntCondition.NotEqualTo)
			{
				if (count != intValue)
				{
					return true;
				}
			}
			
			else if (intCondition == IntCondition.LessThan)
			{
				if (count < intValue)
				{
					return true;
				}
			}
			
			else if (intCondition == IntCondition.MoreThan)
			{
				if (count > intValue)
				{
					return true;
				}
			}
		}
		
		else if (count > 0)
		{
			return true;
		}
		
		return false;	
	}
	

	#if UNITY_EDITOR
	
	public void ShowGUI (int lowerValue, int upperValue)
	{
		
		if (!inventoryManager)
		{
			inventoryManager = AdvGame.GetReferences ().inventoryManager;
		}
		
		if (inventoryManager)
		{
				
			// Create a string List of the field's names (for the PopUp box)
			List<string> labelList = new List<string>();
			
			int i = 0;
			invNumber = -1;
			
			if (inventoryManager.items.Count > 0)
			{
			
				foreach (InvItem _item in inventoryManager.items)
				{
					labelList.Add (_item.label);
					
					// If an item has been removed, make sure selected variable is still valid
					if (_item.id == invID)
					{
						invNumber = i;
					}
					
					i++;
				}
				
				if (invNumber == -1)
				{
					// Wasn't found (item was possibly deleted), so revert to zero
					Debug.LogWarning ("Previously chosen item no longer exists!");
					
					invNumber = 0;
					invID = 0;
				}
				
				EditorGUILayout.BeginHorizontal();
					invNumber = EditorGUILayout.Popup ("Inventory item:", invNumber, labelList.ToArray());
					invID = inventoryManager.items[invNumber].id;
				EditorGUILayout.EndHorizontal();
				
				if (lowerValue > upperValue)
				{
					lowerValue = upperValue;
				}
				else if (upperValue < lowerValue)
				{
					upperValue = lowerValue;
				}
				
				if (inventoryManager.items[invNumber].canCarryMultiple)
				{
					doCount = EditorGUILayout.Toggle ("Query count?", doCount);
				
					if (doCount)
					{
						EditorGUILayout.BeginHorizontal ("");
							EditorGUILayout.LabelField ("Count is:", GUILayout.MaxWidth (70));
							intCondition = (IntCondition) EditorGUILayout.EnumPopup (intCondition);
							intValue = EditorGUILayout.IntField (intValue);
						
							if (intValue < 1)
							{
								intValue = 1;
							}
						EditorGUILayout.EndHorizontal ();
					}
				}
				else
				{
					doCount = false;
				}
				
				if (doCount)
				{
					resultActionTrue = (Action.ResultAction) EditorGUILayout.EnumPopup("If condition is met:", (Action.ResultAction) resultActionTrue);
				}
				else
				{
					resultActionTrue = (Action.ResultAction) EditorGUILayout.EnumPopup("If player is carrying:", (Action.ResultAction) resultActionTrue);
				}
				
				if (resultActionTrue == Action.ResultAction.RunCutscene)
				{
					linkedCutsceneTrue = (Cutscene) EditorGUILayout.ObjectField ("Cutscene to trigger:", linkedCutsceneTrue, typeof (Cutscene), true);
				}
				else if (resultActionTrue == Action.ResultAction.Skip)
				{
					skipActionTrue = EditorGUILayout.IntSlider ("Action # to skip to:", skipActionTrue, lowerValue, upperValue);
				}
		
				if (doCount)
				{
					resultActionFail = (Action.ResultAction) EditorGUILayout.EnumPopup("If condition is not met:", (Action.ResultAction) resultActionFail);
				}
				else
				{
					resultActionFail = (Action.ResultAction) EditorGUILayout.EnumPopup("If player is not carrying:", (Action.ResultAction) resultActionFail);
				}
				
				if (resultActionFail == Action.ResultAction.RunCutscene)
				{
					linkedCutsceneFail = (Cutscene) EditorGUILayout.ObjectField ("Cutscene to trigger:", linkedCutsceneFail, typeof (Cutscene), true);
				}
				else if (resultActionFail == Action.ResultAction.Skip)
				{
					skipActionFail = EditorGUILayout.IntSlider ("Action # to skip to:", skipActionFail, lowerValue, upperValue);
				}
			}

			else
			{
				EditorGUILayout.LabelField ("No inventory items exist!");
				invID = -1;
				invNumber = -1;
			}
		}
	}

	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (inventoryManager)
		{
			if (inventoryManager.items.Count > 0)
			{
				if (invNumber > -1)
				{
					labelAdd = " (" + inventoryManager.items[invNumber].label + ")";
				}
			}
		}
		
		return labelAdd;
	}

	#endif
	
}