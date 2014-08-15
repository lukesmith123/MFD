/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"RuntimeActionList.cs"
 * 
 *	This is a special derivative of ActionList, attached to the GameEngine.
 *	It is used to run InventoryActionLists, which are assets defined outside of the scene.
 *	When an InventoryActionList is called upon, it's actions are copied here and run locally.
 * 
 */

using UnityEngine;
using System.Collections;

public class RuntimeActionList : ActionList
{

	public void Play (InvActionList invActionList)
	{
		
		if (invActionList.actions.Count > 0)
		{
			actions.Clear ();
			
			foreach (Action action in invActionList.actions)
			{
				actions.Add (action);
			}
			
			Interact ();
		}
		
	}
	
}
