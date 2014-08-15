/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"InvItem.cs"
 * 
 *	This script is a container class for individual inventory items.
 * 
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InvItem
{

	public int count;
	public Texture2D tex;
	public bool carryOnStart;
	public bool canCarryMultiple;
	public string label;
	public int id;
	
	public InvActionList useActionList;
	public InvActionList lookActionList;
	public List<InvActionList> combineActionList;
	public List<int> combineID;

	
	public InvItem ()
	{
		count = 0;
		tex = null;
		label = "Inventory item " + (id + 1).ToString ();
		id = 0;
	}
	
	
	public InvItem (int[] idArray)
	{
		count = 0;
		tex = null;
		id = 0;

		combineActionList = new List<InvActionList>();
		combineActionList.Add (null);
		
		combineID = new List<int>();
		combineID.Add (0);
		
		// Update id based on array
		foreach (int _id in idArray)
		{
			if (id == _id)
				id ++;
		}

		label = "Inventory item " + (id + 1).ToString ();

	}
	
	
	public InvItem (InvItem assetItem)
	{
		// Duplicates Asset to Runtime instance
		// (Do it the long way to ensure no connections remain to the asset file)
		
		count = assetItem.count;
		tex = assetItem.tex;
		id = assetItem.id;
		label = assetItem.label;
		useActionList = assetItem.useActionList;
		lookActionList = assetItem.lookActionList;
		combineActionList = assetItem.combineActionList;
		combineID = assetItem.combineID;
	}
	
}