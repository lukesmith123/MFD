using UnityEngine;
using UnityEditor;
using System.IO;

public class AdventureCreator : EditorWindow
{
	
	public References references;
	
	private string version = "1.13";
	
	private bool showScene = true;
	private bool showSettings = false;
	private bool showActions = false;
	private bool showGVars = false;
	private bool showInvItems = false;
	private bool showSpeech = false;
	
	private Vector2 scroll;


	[MenuItem ("Window/Adventure Creator")]
	static void Init ()
	{
		// Get existing open window or if none, make a new one:
		AdventureCreator window = (AdventureCreator) EditorWindow.GetWindow (typeof (AdventureCreator));
		window.GetReferences ();
	}
	
	
	void GetReferences ()
	{
		references = (References) Resources.Load (Resource.references);
	}
	
	
	public void OnInspectorUpdate()
	{
		Repaint();
	}
	
	
	void OnGUI ()
	{
				
		if (!references)
		{
			GetReferences ();
		}
		
		if (references)
		{
			GUILayout.Space (10);
	
			GUILayout.BeginHorizontal ();
			
				if (GUILayout.Toggle (showScene, "Scene", "toolbarbutton", GUILayout.ExpandWidth (true)))
				{
					SetTab (0);
				}
				if (GUILayout.Toggle (showSettings, "Settings", "toolbarbutton", GUILayout.ExpandWidth (true))) 
				{
					SetTab (1);
				}
				if (GUILayout.Toggle (showActions, "Actions", "toolbarbutton", GUILayout.ExpandWidth (true)))
				{
					SetTab (2);
				}
				if (GUILayout.Toggle (showGVars, "Variables", "toolbarbutton", GUILayout.ExpandWidth (true)))
				{
					SetTab (3);
				}
				if (GUILayout.Toggle (showInvItems, "Inventory", "toolbarbutton", GUILayout.ExpandWidth (true)))
				{
					SetTab (4);
				}
				if (GUILayout.Toggle (showSpeech, "Speech", "toolbarbutton", GUILayout.ExpandWidth (true)))
				{
					SetTab (5);
				}
	
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);
			
			scroll = GUILayout.BeginScrollView (scroll);
			
			if (showScene)
			{
				GUILayout.Label ("Scene manager", EditorStyles.largeLabel);
				
				references.sceneManager = (SceneManager) EditorGUILayout.ObjectField ("Asset file: ", references.sceneManager, typeof (SceneManager), false);
				
				EditorGUILayout.Separator ();
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				
				if (!references.sceneManager)
				{
					AskToCreate <SceneManager> ("SceneManager");
				}
				else
				{
					references.sceneManager.ShowGUI ();
				}
			}
			
			else if (showSettings)
			{
				GUILayout.Label ("Settings manager", EditorStyles.largeLabel);
				
				references.settingsManager = (SettingsManager) EditorGUILayout.ObjectField ("Asset file: ", references.settingsManager, typeof (SettingsManager), false);
				
				EditorGUILayout.Separator ();
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				
				if (!references.settingsManager)
				{
					AskToCreate <SettingsManager> ("SettingsManager");
				}
				else
				{
					references.settingsManager.ShowGUI ();
				}
			}
			
			else if (showActions)
			{
				GUILayout.Label ("Actions manager", EditorStyles.largeLabel);
				
				references.actionsManager = (ActionsManager) EditorGUILayout.ObjectField ("Asset file: ", references.actionsManager, typeof (ActionsManager), false);
				
				EditorGUILayout.Separator ();
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				
				if (!references.actionsManager)
				{
					AskToCreate <ActionsManager> ("ActionsManager");
				}
				else
				{
					references.actionsManager.ShowGUI ();
				}
			}
			
			else if (showGVars)
			{
				GUILayout.Label ("Gloval variables manager", EditorStyles.largeLabel);
				
				references.variablesManager = (VariablesManager) EditorGUILayout.ObjectField ("Asset file: ", references.variablesManager, typeof (VariablesManager), false);
				
				EditorGUILayout.Separator ();
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				
				if (!references.variablesManager)
				{
					AskToCreate <VariablesManager> ("VariablesManager");
				}
				else
				{
					references.variablesManager.ShowGUI ();
				}
			}
			
			else if (showInvItems)
			{
				GUILayout.Label ("Inventory items manager", EditorStyles.largeLabel);
				
				references.inventoryManager = (InventoryManager) EditorGUILayout.ObjectField ("Asset file: ", references.inventoryManager, typeof (InventoryManager), false);
				
				EditorGUILayout.Separator ();
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

				if (!references.inventoryManager)
				{
					AskToCreate <InventoryManager> ("InventoryManager");
				}
				else
				{
					references.inventoryManager.ShowGUI ();
				}
			}
			
			else if (showSpeech)
			{
				GUILayout.Label ("Speech manager", EditorStyles.largeLabel);
				
				
				references.speechManager = (SpeechManager) EditorGUILayout.ObjectField ("Asset file: ", references.speechManager, typeof (SpeechManager), false);
				EditorGUILayout.Separator ();
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				
				if (!references.speechManager)
				{
					AskToCreate <SpeechManager> ("SpeechManager");
				}
				else
				{
					references.speechManager.ShowGUI ();
				}
			}
			
			GUILayout.Label ("Adventure Creator - Version " + version);

			GUILayout.EndScrollView ();
		}
		else
		{
			EditorStyles.label.wordWrap = true;
			GUILayout.Label ("No 'References' asset found in the resources folder. Please click to create one.", EditorStyles.label);
			
			if (GUILayout.Button ("Create 'References' file"))
			{
				references = CustomAssetUtility.CreateAndReturnAsset<References> ("References", "AdventureCreator" + Path.DirectorySeparatorChar.ToString () + "Resources");
			}
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (this);
			EditorUtility.SetDirty (references);
		}
	}
	
	
	private void SetTab (int tab)
	{
		showScene = false;
		showSettings = false;
		showActions = false;
		showGVars = false;
		showInvItems = false;
		showSpeech = false;
		
		if (tab == 0)
		{
			showScene = true;
		}
		else if (tab == 1)
		{
			showSettings = true;
		}
		else if (tab == 2)
		{
			showActions = true;
		}
		else if (tab == 3)
		{
			showGVars = true;
		}
		else if (tab == 4)
		{
			showInvItems = true;
		}
		else if (tab == 5)
		{
			showSpeech = true;
		}
	}
	
	
	private void AskToCreate<T> (string obName) where T : ScriptableObject
	{
		EditorStyles.label.wordWrap = true;
		GUILayout.Label ("A '" + obName + "' asset is required for the game to run correctly.", EditorStyles.label);
		
		if (GUILayout.Button ("Create new " + obName + " file"))
		{
			try {
				ScriptableObject t = CustomAssetUtility.CreateAndReturnAsset<T> (obName, Resource.managersDirectory);
				
				Undo.RegisterUndo (references, "Assign " + obName);
				
				if (t is SceneManager)
				{
					references.sceneManager = (SceneManager) t;
				}
				else if (t is SettingsManager)
				{
					references.settingsManager = (SettingsManager) t;
				}
				else if (t is ActionsManager)
				{
					references.actionsManager = (ActionsManager) t;
					references.actionsManager.RefreshList ();
				}
				else if (t is VariablesManager)
				{
					references.variablesManager = (VariablesManager) t;
				}
				else if (t is InventoryManager)
				{
					references.inventoryManager = (InventoryManager) t;
				}
				else if (t is SpeechManager)
				{
					references.speechManager = (SpeechManager) t;
				}
			}
			catch
			{
				Debug.LogWarning ("Could not create " + obName + ". Does the subdirectory " + Resource.managersDirectory + " exist?");
			}
		}
	}
	
}