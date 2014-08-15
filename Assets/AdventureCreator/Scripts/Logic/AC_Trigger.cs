/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Trigger.cs"
 * 
 *	This ActionList runs when the Player enters it.
 * 
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class AC_Trigger : ActionList
{
	
	public int triggerType;
	public bool showInEditor = false;
	
	
	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag (Tags.player) && stateHandler && stateHandler.gameState == GameState.Normal && triggerType == 0)
		{
			Interact ();
		}
	}
	
	void OnTriggerStay (Collider other)
	{
		if (other.CompareTag (Tags.player) && stateHandler && stateHandler.gameState == GameState.Normal && triggerType == 1)
		{
			Interact ();
		}
	}
	
	private void TurnOn ()
	{
		if (collider)
		{
			collider.enabled = true;
		}
		else
		{
			Debug.LogWarning ("Cannot turn " + this.name + " on because it has no collider component.");
		}
	}
	
	
	private void TurnOff ()
	{
		if (collider)
		{
			collider.enabled = false;
		}
		else
		{
			Debug.LogWarning ("Cannot turn " + this.name + " off because it has no collider component.");
		}
	}
	
	
	private void OnDrawGizmos ()
	{
		if (showInEditor)
		{
			DrawGizmos ();
		}
	}
	
	
	private void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}
	
	
	private void DrawGizmos ()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = new Color (1f, 0.3f, 0f, 0.8f);
		Gizmos.DrawCube (Vector3.zero, Vector3.one);
	}
	
}
