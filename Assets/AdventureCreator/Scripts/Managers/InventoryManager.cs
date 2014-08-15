/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionsManager.cs"
 * 
 *	This script handles the "Inventory" tab of the main wizard.
 *	Inventory items are defined with this.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class InventoryManager : ScriptableObject
{
		
	public List<InvItem> items;
	public InvActionList unhandledCombine;
	public InvActionList unhandledHotspot;

	
	#if UNITY_EDITOR
	
	private int invNumber = 0;
	
	private static GUIContent
		insertContent = new GUIContent("+", "Insert item"),
		deleteContent = new GUIContent("-", "Delete item");

	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth (20f),
		valueWidth = GUILayout.MaxWidth (146f);
	

	public void ShowGUI ()
	{
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Unhandled events", EditorStyles.boldLabel);
		unhandledCombine = (InvActionList) EditorGUILayout.ObjectField ("Combine:", unhandledCombine, typeof (InvActionList), false);
		unhandledHotspot = (InvActionList) EditorGUILayout.ObjectField ("Use on hotspot:", unhandledHotspot, typeof (InvActionList), false);
		
		List<string> labelList = new List<string>();
		foreach (InvItem _item in items)
		{
			labelList.Add (_item.label);
		}
		
		// List items
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Inventory items", EditorStyles.boldLabel);
		foreach (InvItem item in items)
		{

			EditorGUILayout.BeginVertical("Button");
				EditorGUILayout.BeginHorizontal ();
			
					EditorGUILayout.LabelField ("Name:", valueWidth);
					
					item.label = EditorGUILayout.TextField (item.label);
			
					if (GUILayout.Button (insertContent, EditorStyles.miniButtonLeft, buttonWidth))
					{
						Undo.RegisterUndo (this, "Add inventory item");
						int position = items.IndexOf (item) + 1;
						items.Insert (position, new InvItem (GetIDArray ()));
						break;
					}
					if (GUILayout.Button (deleteContent, EditorStyles.miniButtonRight, buttonWidth))
					{
						Undo.RegisterUndo (this, "Delete inventory: " + item.label);
						items.Remove (item);
						break;
					}
			
				EditorGUILayout.EndHorizontal ();
			
				item.tex = (Texture2D) EditorGUILayout.ObjectField ("Texture:", item.tex, typeof (Texture2D), false);
				item.carryOnStart = EditorGUILayout.Toggle ("Carry on start?", item.carryOnStart);
				item.canCarryMultiple = EditorGUILayout.Toggle ("Can carry multiple?", item.canCarryMultiple);
			
				EditorGUILayout.LabelField ("Standard events", EditorStyles.boldLabel);
				item.useActionList = (InvActionList) EditorGUILayout.ObjectField ("Use:", item.useActionList, typeof (InvActionList), false);
				item.lookActionList = (InvActionList) EditorGUILayout.ObjectField ("Examine:", item.lookActionList, typeof (InvActionList), false);
				
				EditorGUILayout.LabelField ("Combine events", EditorStyles.boldLabel);
	
	
				for (int i=0; i<item.combineActionList.Count; i++)
				{
					
					EditorGUILayout.BeginHorizontal ();
					
						invNumber = GetArraySlot (item.combineID[i]);
						invNumber = EditorGUILayout.Popup (invNumber, labelList.ToArray());
						item.combineID[i] = items[invNumber].id;
					
						item.combineActionList[i] = (InvActionList) EditorGUILayout.ObjectField (item.combineActionList[i], typeof (InvActionList), false);
					
						if (GUILayout.Button (deleteContent, EditorStyles.miniButtonRight, buttonWidth))
						{
							Undo.RegisterUndo (this, "Delete combine event");
							item.combineActionList.RemoveAt (i);
							item.combineID.RemoveAt (i);
							break;
						}
					
					EditorGUILayout.EndHorizontal ();
				}
				if (GUILayout.Button ("Add combine event"))
				{
					Undo.RegisterUndo (this, "Add new combine event");
					item.combineActionList.Add (null);
					item.combineID.Add (0);
				}
				
			EditorGUILayout.EndVertical();
			
		}

		if (GUILayout.Button("Create new inventory item"))
		{
			Undo.RegisterUndo (this, "Add inventory item");
			items.Add (new InvItem (GetIDArray ()));
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (this);
		}
	}
	
	#endif
	
	
	int[] GetIDArray ()
	{
		// Returns a list of id's in the list
		
		List<int> idArray = new List<int>();
		
		foreach (InvItem item in items)
		{
			idArray.Add (item.id);
		}
		
		idArray.Sort ();
		return idArray.ToArray ();
	}
	

	public string GetLabel (int _id)
	{
		// Return the label of inventory with ID _id
		string result = "";
		foreach (InvItem item in items)
		{
			if (item.id == _id)
			{
				result = item.label;
			}
		}
		
		return result;
	}
	
	
	private int GetArraySlot (int _id)
	{
		int i = 0;
		foreach (InvItem item in items)
		{
			if (item.id == _id)
			{
				return i;
			}
			i++;
		}
		
		return 0;
	}


}