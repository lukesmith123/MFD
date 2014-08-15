/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Highlight.cs"
 * 
 *	This script is attached to any gameObject that glows
 *	when a cursor is placed over it's associated interaction
 *	object.  These are not always the same object.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highlight : MonoBehaviour
{

	private float maxHighlight = 2f;
	private float highlight = 1f;
	private bool doHightlight = false;
	private int direction = 1;
	private float fadeStartTime;
	
	private List<Color> originalColors = new List<Color>();
	
	
	private void Awake ()
	{
		// Go through own materials
		if (renderer)
		{
			foreach (Material material in renderer.materials)
			{
				if (material.HasProperty ("_Color"))
				{
					originalColors.Add (material.color);
				}
			}
		}
		
		// Go through any child materials
		Component[] children;
		children = GetComponentsInChildren <Renderer>();
		foreach (Renderer childRenderer in children)
		{
			foreach (Material material in childRenderer.materials)
			{
				if (material.HasProperty ("_Color"))
				{
					originalColors.Add (material.color);
				}
			}
		}
	}
	
	
	private void FixedUpdate ()
	{
		if (doHightlight)
		{	
			if (direction == 1)
			{
				// Add highlight
				highlight = Mathf.Lerp (1f, maxHighlight, AdvGame.LinearTimeFactor (fadeStartTime, 0.3f));
				
				if (highlight >= maxHighlight)
				{
					highlight = maxHighlight;
					doHightlight = false;
				}
			}
			else
			{
				// Remove highlight
				highlight = Mathf.Lerp (maxHighlight, 1f, AdvGame.LinearTimeFactor (fadeStartTime, 0.3f));
				
				if (highlight <= 1f)
				{
					highlight = 1f;
					doHightlight = false;
				}
			}

			int i = 0;
			float alpha;

			// Go through own materials
			if (renderer)
			{
				foreach (Material material in renderer.materials)
				{
					if (material.HasProperty ("_Color"))
					{
						alpha = material.color.a;
						Color newColor = originalColors[i] * highlight;
						newColor.a = alpha;
						material.color = newColor;
						i++;
					}
				}
			}
			
			// Go through any child materials
			Component[] children;
			children = GetComponentsInChildren <Renderer>();
			foreach (Renderer childRenderer in children)
			{
				foreach (Material material in childRenderer.materials)
				{
					if (material.HasProperty ("_Color"))
					{
						alpha = material.color.a;
						Color newColor = originalColors[i] * highlight;
						newColor.a = alpha;
						material.color = newColor;
						i++;
					}
				}
			}
		}
	}
	
	
	public void HighlightOn ()
	{
		// Don't repeat if already turning on
		if (!doHightlight || direction == -1)
		{
			doHightlight = true;
			highlight = 1f;
			direction = 1;
			fadeStartTime = Time.time;
		}
	}
	
	
	public void HighlightOff ()
	{
		// Don't repeat if already turning off
		if (!doHightlight || direction == 1)
		{
			doHightlight = true;
			highlight = maxHighlight;
			direction = -1;
			fadeStartTime = Time.time;
		}
	}
	
	
	public void HighlightOffInstant ()
	{
		doHightlight = false;
		
		// Go through any child materials
		int i=0;
		Component[] children;
		children = GetComponentsInChildren <Renderer>();
		foreach (Renderer childRenderer in children)
		{
			foreach (Material material in childRenderer.materials)
			{
				if (material.HasProperty ("_Color"))
				{
					Color newColor = originalColors[i];
					material.color = newColor;
					i++;
				}
			}
		}
	}
	
}
