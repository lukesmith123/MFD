/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"RuntimeInventory.cs"
 * 
 *	This script creates a local copy of the InventoryManager's items.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RuntimeInventory : MonoBehaviour
{

	[HideInInspector] public List<InvItem> localItems;
	[HideInInspector] public InvActionList unhandledCombine;
	[HideInInspector] public InvActionList unhandledHotspot;
	
	[HideInInspector] public bool isLocked = true;
	[HideInInspector] public int selectedID = -1;
	
	private InventoryManager inventoryManager;
	private RuntimeActionList runtimeActionList;
	
	
	public void Awake ()
	{
		GetReferences ();

		localItems.Clear ();
		GetItemsOnStart ();
		
		if (inventoryManager)
		{
			unhandledCombine = inventoryManager.unhandledCombine;
			unhandledHotspot = inventoryManager.unhandledHotspot;
		}
		else
		{
			Debug.LogError ("An Inventory Manager is required - please use the Adventure Creator window to create one.");
		}
		
	}
	
	
	private void GetReferences ()
	{
		if (GameObject.FindWithTag (Tags.gameEngine) && GameObject.FindWithTag (Tags.gameEngine).GetComponent <RuntimeActionList>())
		{
			runtimeActionList = GameObject.FindWithTag (Tags.gameEngine).GetComponent <RuntimeActionList>();
		}
		
		if (AdvGame.GetReferences () && AdvGame.GetReferences ().inventoryManager)
		{
			inventoryManager = AdvGame.GetReferences ().inventoryManager;
		}
	}

	
	public void SetNull ()
	{
		selectedID = -1;
	}
	
	
	public void SelectItemByID (int _id)
	{
		// Only select if carrying
		bool found = false;
		
		foreach (InvItem item in localItems)
		{
			if (item.id == _id)
			{
				found = true;
			}
		}
		
		if (found)
		{
			selectedID = _id;
		}
		else
		{
			SetNull ();
			
			GetReferences ();
			Debug.LogWarning ("Want to select inventory item " + inventoryManager.GetLabel (_id) + " but player is not carrying it.");
		}
	}
	
	
	public void SelectItem (int i)
	{
				
		if (selectedID == localItems [i].id)
		{
			selectedID = -1;
		}
		else
		{
			selectedID = localItems [i].id;
		}

	}
	
	
	private void GetItemsOnStart ()
	{
		if (inventoryManager)
		{
			foreach (InvItem item in inventoryManager.items)
			{
				if (item.carryOnStart)
				{
					isLocked = false;
					localItems.Add (item);
				}
			}
		}
		else
		{
			Debug.LogError ("No Inventory Manager found - please use the Adventure Creator window to create one.");
		}
	}
	
	
	public void Add (int _id, int amount)
	{
		// Raise "count" by 1 for appropriate ID
		
		if (localItems.Count == 0)
		{
			isLocked = false;
		}
		
		bool isCarrying = false;
		
		foreach (InvItem item in localItems)
		{
			if (item.id == _id)
			{
				isCarrying = true;
				
				if (!item.canCarryMultiple)
				{
					amount = 1;
				}
				
				break;
			}
		}
		
		if (!isCarrying)
		{	
			GetReferences ();

			if (inventoryManager)
			{
				foreach (InvItem assetItem in inventoryManager.items)
				{
					if (assetItem.id == _id)
					{
						InvItem newItem = assetItem;
						
						if (!newItem.canCarryMultiple)
						{
							amount = 1;
						}
						
						newItem.count = amount;
						localItems.Add (newItem);
					}
				}
			}
		}
	
	}
	
	
	public void Remove (int _id, int amount)
	{
		// Reduce "count" by 1 for appropriate ID

		foreach (InvItem item in localItems)
		{
			if (item.id == _id)
			{
				if (item.count > 0)
				{
					item.count -= amount;
				}
				if (item.count < 1)
				{
					localItems.Remove (item);
				}
				
				if (localItems.Count == 0)
				{
					isLocked = true;
				}
				
				break;
			}
		}
	}
	
	
	public string GetLabel (int _id)
	{
		// Return the label of inventory with ID _id
		
		string result = "";
		foreach (InvItem item in localItems)
		{
			if (item.id == _id)
			{
				result = item.label;
			}
		}
		
		return result;
	}
	
	
	public int GetCount (int _id)
	{
		// Return the count of inventory with ID _id
		
		int result = 0;
		foreach (InvItem item in localItems)
		{
			if (item.id == _id)
			{
				result = item.count;
			}
		}
		
		return result;
	}


	public Texture2D GetTexture (int _id)
	{
		// Return the texture of inventory with ID _id
		
		Texture2D result = null;
		foreach (InvItem item in localItems)
		{
			if (item.id == _id)
			{
				result = item.tex;
			}
		}
		
		return result;
	}

	
	public void Look (int i)
	{
		GetReferences ();
		
		if (runtimeActionList && localItems [i].lookActionList)
		{
			runtimeActionList.Play (localItems [i].lookActionList);
		}
	}
	
	
	public void Use (int i)
	{
		GetReferences ();
		
		if (runtimeActionList)
		{
			if (localItems [i].useActionList)
			{
				selectedID = -1;
				runtimeActionList.Play (localItems [i].useActionList);
			}
			else
			{
				SelectItem (i);
			}
		}
	}
	
	
	public void Combine (int _slot)
	{
		GetReferences ();
		
		if (localItems [_slot].id == selectedID)
		{
			selectedID = -1;
		}
		else if (runtimeActionList)
		{
			bool foundMatch = false;
			for (int i=0; i<localItems [_slot].combineID.Count; i++)
			{
				if (localItems [_slot].combineID[i] == selectedID && localItems [_slot].combineActionList[i])
				{
					selectedID = -1;
					runtimeActionList.Play (localItems [_slot].combineActionList [i]);
					foundMatch = true;
					break;
				}
			}
			
			if (!foundMatch)
			{
				selectedID = -1;
				
				if (unhandledCombine)
				{
					runtimeActionList.Play (unhandledCombine);
				}
			}
		}	
	}
	
	
	private void OnEnable ()
	{
		runtimeActionList = null;
		inventoryManager = null;
	}

}