/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"AdvGame.cs"
 * 
 *	This script provides a number of static functions used by various game scripts.
 * 
 * 	The "DrawOutline" function is based on Bérenger's code: http://wiki.unity3d.com/index.php/ShadowAndOutline
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdvGame : ScriptableObject
{
	
	public static References GetReferences ()
	{
		References references = (References) Resources.Load (Resource.references);
		
		if (references)
		{
			return (references);
		}
		
		return (null);
	}
	
	
	public static List<Action> SwapActions (List<Action> list, int a1, int a2)
	{
		Action tempAction = list[a1];
		list[a1] = list[a2];
		list[a2] = tempAction;
		return (list);
	}
	
	
	public static string UniqueName (string name)
	{
		if (GameObject.Find (name))
		{
			string newName = name;
			
			for (int i=2; i<20; i++)
			{
				newName = name + i.ToString ();
				
				if (!GameObject.Find (newName))
				{
					break;
				}
			}
			
			return newName;
		}
		else
		{
			return name;
		}
	}
	
	
	public static string GetName (string resourceName)
	{
		int slash = resourceName.IndexOf ("/");
		string newName;
		
		if (slash > 0)
		{
			newName = resourceName.Remove (0, slash+1);
		}
		else
		{
			newName = resourceName;
		}
		
		return newName;
	}
	
	
	public static Rect GUIBox (float centre_x, float centre_y, float size)
	{
		Rect newRect;
		newRect = GUIRect (centre_x, centre_y, size, size);
		return (newRect);
	}
	
	
	public static Rect GUIRect (float centre_x, float centre_y, float width, float height)
	{
		Rect newRect;
		newRect = new Rect (Screen.width * centre_x - (Screen.width * width)/2, Screen.height * centre_y - (Screen.width * height)/2, Screen.width * width, Screen.width * height);
		return (newRect);
	}
	
	
	public static Rect GUIBox (Vector2 posVector, float size)
	{
		Rect newRect;
		newRect = GUIRect (posVector.x / Screen.width, (Screen.height - posVector.y) / Screen.height, size, size);
		return (newRect);
	}
	
	
	public static void AddAnimClip (Animation _animation, int layer, AnimationClip clip, AnimationBlendMode blendMode, WrapMode wrapMode, Transform mixingBone)
	{
		// Initialises a clip
		_animation.AddClip (clip, clip.name);
		
		if (mixingBone != null)
		{
			_animation [clip.name].AddMixingTransform (mixingBone);
		}
		
		// Set up the state
		_animation [clip.name].layer = layer;
		_animation [clip.name].normalizedTime = 0f;
		_animation [clip.name].blendMode = blendMode;
		_animation [clip.name].wrapMode = wrapMode;
		_animation [clip.name].enabled = true;
	}
	
	
	public static void PlayAnimClip (Animation _animation, int layer, AnimationClip clip, AnimationBlendMode blendMode, WrapMode wrapMode, float fadeTime, Transform mixingBone)
	{
		// Initialises and crossfades a clip

		AddAnimClip (_animation, layer, clip, blendMode, wrapMode, mixingBone);
		
		_animation.CrossFade (clip.name, fadeTime);
		
		CleanUnusedClips (_animation);

	}
	
	
	public static void CleanUnusedClips (Animation _animation)
	{
		// Remove any non-playing animations
		
		List <string> removeClips = new List <string>();
		
		foreach (AnimationState state in _animation)
		{
			if (!_animation [state.name].enabled)
			{
				// Queued animations get " - Queued Clone" appended to it, so remove
				
				int queueIndex = state.name.IndexOf (" - Queued Clone");

				if (queueIndex > 0)
				{
					removeClips.Add (state.name.Substring (0, queueIndex));
				}
				else
				{
					removeClips.Add (state.name);
				}
			}
		}
		
		foreach (string _clip in removeClips)
		{
			_animation.RemoveClip (_clip);
		}
		
	}
	
	
	public static float SmoothTimeFactor (float startT, float deltaT)
	{
		// Return a smooth time scale
		
		float t01 = (Time.time - startT) / deltaT;
		float F = 0.5f - 0.515f * Mathf.Sin (3f * t01 + 1.5f);
		return F;
	}
	
	
	public static float LinearTimeFactor (float startT, float deltaT)
	{
		// Return a linear time scale

		float t01 = (Time.time - startT) / deltaT;
		return t01;
	}
	
	
	public static Rect Rescale (Rect _rect)
	{
		float ScaleFactor;
		ScaleFactor = Screen.width / 884.0f;
		int ScaleFactorInt = Mathf.RoundToInt(ScaleFactor);
		Rect newRect = new Rect (_rect.x * ScaleFactorInt, _rect.y * ScaleFactorInt, _rect.width * ScaleFactorInt, _rect.height * ScaleFactorInt);
		
		return (newRect);
	}
	
	
	public static int Rescale (int _int)
	{
		float ScaleFactor;
		ScaleFactor = Screen.width / 884.0f;
		int ScaleFactorInt = Mathf.RoundToInt(ScaleFactor);
		int returnValue;
		returnValue = _int * ScaleFactorInt;
		
		return (returnValue);
	}
	
	
	public static void DrawOutline (Rect rect, string text, GUIStyle style, Color outColor, Color inColor, float size)
	{
		float halfSize = size * 0.5F;
		GUIStyle backupStyle = new GUIStyle(style);
		Color backupColor = GUI.color;
		
		outColor.a = GUI.color.a;
		style.normal.textColor = outColor;
		GUI.color = outColor;

		rect.x -= halfSize;
		GUI.Label(rect, text, style);

		rect.x += size;
		GUI.Label(rect, text, style);

		rect.x -= halfSize;
		rect.y -= halfSize;
		GUI.Label(rect, text, style);

		rect.y += size;
		GUI.Label(rect, text, style);

		rect.y -= halfSize;
		style.normal.textColor = inColor;
		GUI.color = backupColor;
		GUI.Label(rect, text, style);

		style = backupStyle;
	}
	
}	