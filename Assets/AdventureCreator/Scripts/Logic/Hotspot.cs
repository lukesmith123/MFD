/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Hotspot.cs"
 * 
 *	This script handles all the possible
 *	interactions on both hotspots and NPCs.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hotspot : MonoBehaviour
{

	public bool showInEditor = true;
	
	public string hotspotName;
	public InteractionIcon useIcon;
	public Highlight highlight;
	public bool playUseAnim;
	public Marker walkToMarker;
	
	public bool provideUseInteraction;
	public Button useButton = new Button();
	
	public bool provideLookInteraction;
	public Button lookButton = new Button();
	
	public bool provideInvInteraction;
	public List<Button> invButtons = new List<Button>();
	
	
	private void TurnOn ()
	{
		gameObject.layer = LayerMask.NameToLayer ("Default");
	}
	
	
	private void TurnOff ()
	{
		gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
	}
	
	
	public void Select ()
	{
		if (highlight)
		{
			highlight.HighlightOn ();
		}
	}
	
	
	public void Deselect ()
	{
		if (highlight)
		{
			highlight.HighlightOff ();
		}
	}
	
	
	public void DeselectInstant ()
	{
		if (highlight)
		{
			highlight.HighlightOffInstant ();
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
		if (this.GetComponent <Char>() == null)
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color (1f, 1f, 0f, 0.6f);
			Gizmos.DrawCube (Vector3.zero, Vector3.one);
		}
	}
	
	
}
