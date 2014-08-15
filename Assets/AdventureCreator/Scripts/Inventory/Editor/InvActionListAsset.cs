using UnityEngine;
using UnityEditor;
using System;

public class InvActionListAsset
{
	
	[MenuItem("Assets/Create/Inventory ActionList")]
	
	public static void CreateAsset ()
	{
		CustomAssetUtility.CreateAsset <InvActionList> ();
	}
	
}