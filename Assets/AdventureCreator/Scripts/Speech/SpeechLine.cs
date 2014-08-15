/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"SpeechLine.cs"
 * 
 *	This script is a data container for speech lines found by Speech Manager.
 *	Such data is used to provide translation support, as well as auto-numbering
 *	of speech lines for sound files.
 * 
 */

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SpeechLine
{
	
	public int lineID;
	public string scene;
	public string character;
	public string text;
	
	public List<string> translationText = new List<string>();
	
	private GUIStyle style;
	
	
	public SpeechLine ()
	{
		lineID = 0;
		scene = "";
		character = "";
		text = "";
		translationText = new List<string> ();
	}
	
	
	public SpeechLine (int _id, string _scene, string _character, string _text, int _languagues)
	{
		lineID = _id;
		scene = _scene;
		character = _character;
		text = _text;
		
		translationText = new List<string>();
		for (int i=0; i<_languagues; i++)
		{
			translationText.Add (_text);
		}
	}
	
	
	public SpeechLine (int[] idArray, string _scene, string _character, string _text, int _languagues)
	{
		// Update id based on array
		lineID = 0;
		foreach (int _id in idArray)
		{
			if (lineID == _id)
				lineID ++;
		}
		
		scene = _scene;
		character = _character;
		text = _text;
		
		translationText = new List<string>();
		for (int i=0; i<_languagues; i++)
		{
			translationText.Add (_text);
		}
	}

	
	#if UNITY_EDITOR
	
	public void ShowGUI (List<string> languages)
	{
		style = new GUIStyle ();
		style.wordWrap = true;
		style.alignment = TextAnchor.MiddleLeft;
	
		GUILayout.BeginVertical ("Button");

			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (character, style, GUILayout.MaxWidth (70));
				EditorGUILayout.LabelField (lineID.ToString (), style, GUILayout.MaxWidth (30));
				EditorGUILayout.LabelField ('"' + text + '"', style, GUILayout.MaxWidth (340));
			EditorGUILayout.EndHorizontal ();
			
			if (translationText.Count > 0)
			{
				for (int i=0; i<translationText.Count; i++)
				{
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (languages [i+1], GUILayout.MaxWidth(80f));
					translationText[i] = EditorGUILayout.TextField (translationText[i]);
					EditorGUILayout.EndHorizontal ();
				}
			}
				
		GUILayout.EndVertical ();	
	}
	
	
	public string Print ()
	{
		string result = "Character: " + character + "\nFilename: " + character + lineID.ToString () + "\n";
		result += '"';
		result += text;
		result += '"';

		return (result);
	}
	
	#endif
	
}