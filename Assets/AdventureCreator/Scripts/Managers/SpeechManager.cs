/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"SpeechManager.cs"
 * 
 *	This script handles the "Speech" tab of the main wizard.
 *	It is used to auto-number lines for audio files, and handle translations.
 * 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SpeechManager : ScriptableObject
{
	
	public List<SpeechLine> lines;
	public List<string> languages = new List<string>();
	public string[] sceneFiles;
	
	private List<SpeechLine> tempLines;
	private string sceneLabel;
	private bool foundLinesInScene;

	
	#if UNITY_EDITOR
	
	private InventoryManager inventoryManager;
	
	private static GUIContent
		deleteContent = new GUIContent("-", "Delete translation");
	
	
	public void ShowGUI ()
	{
		GUILayout.Label ("Languages", EditorStyles.boldLabel);
		
		if (languages.Count == 0)
		{
			ClearLanguages ();
		}
		else
		{
			if (languages.Count > 1)
			{
				for (int i=1; i<languages.Count; i++)
				{
					EditorGUILayout.BeginHorizontal ();
						languages[i] = EditorGUILayout.TextField (languages[i]);
						
						if (GUILayout.Button (deleteContent, EditorStyles.miniButtonLeft, GUILayout.MaxWidth(20f)))
						{
							Undo.RegisterUndo (this, "Delete translation: " + languages[i]);
							DeleteLanguage (i);
						}
					EditorGUILayout.EndHorizontal ();
				}
			}
			
			if (GUILayout.Button ("Create new translation"))
			{
				Undo.RegisterUndo (this, "Add translation");
				CreateLanguage ("New " + languages.Count.ToString ());
			}
		
		}
		
		EditorGUILayout.Space ();

		GUILayout.Label ("Speech lines", EditorStyles.boldLabel);
		GUILayout.Label ("Audio files must be placed in \n/Resources/Speech");
		
		if (GUILayout.Button ("Update list"))
		{
			PopulateList ();
			
			if (sceneFiles.Length > 0)
			{
				Array.Sort (sceneFiles);
			}
		}
		
		if (lines.Count > 0)
		{
			if (GUILayout.Button ("Create script sheet"))
			{
				CreateScript ();
			}
			
			ListLines ();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty (this);
		}
	}

	
	private void ListLines ()
	{
		
		EditorGUILayout.BeginVertical ("Button");		

			if (sceneFiles.Length > 0)
			{
				foreach (string scene in sceneFiles)
				{
					int extentionPos = scene.IndexOf (".unity");
					sceneLabel = scene.Substring (7, extentionPos-7);
					GUILayout.Label ("Scene: " + sceneLabel, EditorStyles.boldLabel);
					
					foundLinesInScene = false;
					
					foreach (SpeechLine line in lines)
					{
						if (line.scene == scene)
						{
							foundLinesInScene = true;
							line.ShowGUI (languages);
						}
					}
					
					if (!foundLinesInScene)
					{
						EditorGUILayout.LabelField ("No lines found.");
					}
				}
			}
			
			// Inventory-based lines
			
			GUILayout.Label ("Inventory:", EditorStyles.boldLabel);
			foundLinesInScene = false;
			foreach (SpeechLine line in lines)
			{
				if (line.scene == "")
				{
					foundLinesInScene = true;
					line.ShowGUI (languages);
				}
			}
			if (!foundLinesInScene)
			{
				EditorGUILayout.LabelField ("No lines found.");
			}
			
		EditorGUILayout.EndVertical ();
		
	}
	
	
	private void CreateLanguage (string name)
	{
		languages.Add (name);
		
		foreach (SpeechLine line in lines)
		{
			line.translationText.Add (line.text);
		}
	}
	
	
	private void DeleteLanguage (int i)
	{
		languages.RemoveAt (i);
		
		foreach (SpeechLine line in lines)
		{
			line.translationText.RemoveAt (i-1);
		}
		
	}
	
	
	private void ClearLanguages ()
	{
		languages.Clear ();

		foreach (SpeechLine line in lines)
		{
			line.translationText.Clear ();
		}

		languages.Add ("Original");	

	}

	
	private void PopulateList ()
	{
		string originalScene = EditorApplication.currentScene;
		
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo ())
		{
			Undo.RegisterUndo (this, "Update speech list");
			
			// Store the lines temporarily, so that we can update the translations afterwards
			BackupTranslations ();

			lines.Clear ();

			sceneFiles = GetSceneFiles ();
		
			// First look for lines that already have an assigned lineID
			foreach (string sceneFile in sceneFiles)
			{
				GetLinesInScene (sceneFile, false);
			}
			
			GetLinesFromInventory (false);
			
			// Now look for new lines, which don't have a unique lineID
			foreach (string sceneFile in sceneFiles)
			{
				GetLinesInScene (sceneFile, true);
			}
			
			GetLinesFromInventory (true);
			
			RestoreTranslations ();
			
			if (EditorApplication.currentScene != originalScene)
			{
				EditorApplication.OpenScene (originalScene);
			}
		}
	}
	
	
	private void ExtractLine (ActionSpeech action, bool onlySeekNew, bool isInScene)
	{
		string speaker = "";
		
		if (action.isPlayer)
		{
			speaker = "Player";
		}
		else if (action.speaker)
		{
			speaker = action.speaker.name;
		}
		
		if (speaker != "" && action.messageText != "")
		{
			if (onlySeekNew && action.lineID == -1)
			{
				// Assign a new ID on creation
				SpeechLine newLine;
				if (isInScene)
				{
					newLine = new SpeechLine (GetIDArray(), EditorApplication.currentScene, speaker, action.messageText, languages.Count-1);
				}
				else
				{
					newLine = new SpeechLine (GetIDArray(), "", speaker, action.messageText, languages.Count-1);
				}
				action.lineID = newLine.lineID;
				lines.Add (newLine);
			}
			
			else if (!onlySeekNew && action.lineID > -1)
			{
				// Already has an ID, so don't replace
				if (isInScene)
				{
					lines.Add (new SpeechLine (action.lineID, EditorApplication.currentScene, speaker, action.messageText, languages.Count-1));
				}
				else
				{
					lines.Add (new SpeechLine (action.lineID, "", speaker, action.messageText, languages.Count-1));
				}
			}
		}
		else
		{
			// Remove from SpeechManager
			action.lineID = -1;
		}
	}
	
	
	private void GetLinesFromInventory (bool onlySeekNew)
	{
		inventoryManager = AdvGame.GetReferences ().inventoryManager;
		
		if (inventoryManager)
		{
			// Unhandled combine
			if (inventoryManager.unhandledCombine != null)
			{	
				foreach (Action action in inventoryManager.unhandledCombine.actions)
				{
					if (action is ActionSpeech)
					{
						ExtractLine (action as ActionSpeech, onlySeekNew, false);
					}
				}
				EditorUtility.SetDirty (inventoryManager.unhandledCombine);
			}
			
			// Unhandled hotspot
			if (inventoryManager.unhandledHotspot != null)
			{
				foreach (Action action in inventoryManager.unhandledHotspot.actions)
				{
					if (action is ActionSpeech)
					{
						ExtractLine (action as ActionSpeech, onlySeekNew, false);
					}
				}
				EditorUtility.SetDirty (inventoryManager.unhandledHotspot);
			}
			
			// Item-specific events
			if (inventoryManager.items.Count > 0)
			{
				foreach (InvItem item in inventoryManager.items)
				{
					
					// Use
					if (item.useActionList != null)
					{
						foreach (Action action in item.useActionList.actions)
						{
							if (action is ActionSpeech)
							{
								ExtractLine (action as ActionSpeech, onlySeekNew, false);
							}
						}
						EditorUtility.SetDirty (item.useActionList);
					}
					
					// Look
					if (item.lookActionList)
					{
						foreach (Action action in item.lookActionList.actions)
						{
							if (action is ActionSpeech)
							{
								ExtractLine (action as ActionSpeech, onlySeekNew, false);
							}
						}
						EditorUtility.SetDirty (item.lookActionList);
					}
					
					// Combines
					foreach (InvActionList actionList in item.combineActionList)
					{
						if (actionList != null)
						{
							foreach (Action action in actionList.actions)
							{
								if (action is ActionSpeech)
								{
									ExtractLine (action as ActionSpeech, onlySeekNew, false);
								}
							}
							EditorUtility.SetDirty (actionList);
						}
					}
				}
			}
		}
	}
		
	
	private void GetLinesInScene (string sceneFile, bool onlySeekNew)
	{
		if (EditorApplication.currentScene != sceneFile)
		{
			EditorApplication.OpenScene (sceneFile);
		}
		
		ActionList[] actionLists = GameObject.FindObjectsOfType (typeof (ActionList)) as ActionList[];
		
        foreach (ActionList list in actionLists)
        {
			foreach (Action action in list.actions)
			{
				if (action is ActionSpeech)
				{
					ExtractLine (action as ActionSpeech, onlySeekNew, true);
				}
			}
        }
		
		// Save the scene
		EditorApplication.SaveScene ();
	}
	
	
	private string[] GetSceneFiles ()
	{
		List<string> temp = new List<string>();
		
		foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
		{
			if (S.enabled)
			{
				temp.Add(S.path);
			}
		}
		
		return temp.ToArray();
	}
	
	
	private int[] GetIDArray ()
	{
		// Returns a list of id's in the list
		
		List<int> idArray = new List<int>();
		
		foreach (SpeechLine line in lines)
		{
			idArray.Add (line.lineID);
		}
		
		idArray.Sort ();
		return idArray.ToArray ();
	}
	
	
	private void RestoreTranslations ()
	{
		// Match IDs for each entry in lines and tempLines, send over translation data
		foreach (SpeechLine tempLine in tempLines)
		{
			foreach (SpeechLine line in lines)
			{
				if (tempLine.lineID == line.lineID)
				{
					line.translationText = tempLine.translationText;
					break;
				}
			}
		}
		
		tempLines = null;
	}
		
	
	private void BackupTranslations ()
	{
		tempLines = new List<SpeechLine>();
		foreach (SpeechLine line in lines)
		{
			tempLines.Add (line);
		}
	}
	
	
	private void CreateScript ()
	{
		string[] s = Application.dataPath.Split('/');
		string projectName = s[s.Length - 2];
		
		string script = "Script file for " + projectName + " - created " + DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy");
		
		// By scene
		foreach (string scene in sceneFiles)
		{
			bool foundLinesInScene = false;
			
			foreach (SpeechLine line in lines)
			{
				if (line.scene == scene)
				{
					if (!foundLinesInScene)
					{
						script += "\n";
						script += "\n";
						script += "Scene: " + scene;
						foundLinesInScene = true;
					}
					
					script += "\n";
					script += "\n";
					script += line.Print ();
				}
			}
		}
		
		// Inventory
		bool foundLinesInInventory = false;
		
		foreach (SpeechLine line in lines)
		{
			if (line.scene == "")
			{
				if (!foundLinesInInventory)
				{
					script += "\n";
					script += "\n";
					script += "Inventory lines: ";
					foundLinesInInventory = true;
				}
				
				script += "\n";
				script += "\n";
				script += line.Print ();
			}
		}
		
		string fileName = "Assets" + Path.DirectorySeparatorChar.ToString () + "GameScript.txt";
		
		Serializer.CreateSaveFile (fileName, script);
	}
	
	#endif
	
	
	public SpeechLine GetLineByID (int lineID)
	{
		foreach (SpeechLine line in lines)
		{
			if (line.lineID == lineID)
			{
				return line;
			}
		}
		
		return null;
	}
	
}