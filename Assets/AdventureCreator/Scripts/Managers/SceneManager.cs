/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionsManager.cs"
 * 
 *	This script handles the "Scene" tab of the main wizard.
 *	It is used to create the prefabs needed to run the game,
 *	as well as provide easy-access to game logic.
 * 
 */

using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneManager : ScriptableObject
{
	
	#if UNITY_EDITOR
	
	private DirectoryInfo dir;
	private FileInfo[] info;
	
	private string assetFolder = "Assets/AdventureCreator/Prefabs/";
	
	private string newFolderName = "";
	private string prefabName;
	private int index_name;
	private int index_dot;
	
	private GameObject gameEngine;
	
	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth(120f);

	
	public void ShowGUI ()
	{
		GUILayout.Label ("Basic structure", EditorStyles.boldLabel);

		if (GUILayout.Button ("Organise room objects"))
		{
			InitialiseObjects ();
		}
		
		gameEngine = GameObject.FindWithTag (Tags.gameEngine);
		if (gameEngine)
		{
			GUILayout.BeginHorizontal ();
				newFolderName = GUILayout.TextField (newFolderName);
				
				if (GUILayout.Button ("Create new folder", buttonWidth))
				{
					if (newFolderName != "")
					{
						Undo.RegisterSceneUndo ("Create new folder");
						
						GameObject newFolder = new GameObject();
						
						if (!newFolderName.StartsWith ("_"))
							newFolder.name = "_" + newFolderName;
						else
							newFolder.name = newFolderName;
						
						if (Selection.activeGameObject)
						{
							newFolder.transform.parent = Selection.activeGameObject.transform;
						}
						
						Selection.activeObject = newFolder;
					}
				}
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
			
			GUILayout.Label ("Scene settings", EditorStyles.boldLabel);
			if (gameEngine.GetComponent <SceneSettings>())
			{
				gameEngine.GetComponent <SceneSettings>().navMesh = (NavigationMesh) EditorGUILayout.ObjectField ("Default NavMesh", gameEngine.GetComponent <SceneSettings>().navMesh, typeof (NavigationMesh), true);
				gameEngine.GetComponent <SceneSettings>().cutsceneOnStart = (Cutscene) EditorGUILayout.ObjectField ("Cutscene on start", gameEngine.GetComponent <SceneSettings>().cutsceneOnStart, typeof (Cutscene), true);
				gameEngine.GetComponent <SceneSettings>().cutsceneOnLoad = (Cutscene) EditorGUILayout.ObjectField ("Cutscene on load", gameEngine.GetComponent <SceneSettings>().cutsceneOnLoad, typeof (Cutscene), true);
				gameEngine.GetComponent <SceneSettings>().defaultPlayerStart = (PlayerStart) EditorGUILayout.ObjectField ("Default PlayerStart", gameEngine.GetComponent <SceneSettings>().defaultPlayerStart, typeof (PlayerStart), true);
			}
			EditorGUILayout.Space ();
			
			GUILayout.Label ("Visibility", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal ();
				GUILayout.Label ("Triggers", buttonWidth);
				if (GUILayout.Button ("On"))
				{
					SetTriggerVisibility (true);
				}
				if (GUILayout.Button ("Off"))
				{
					SetTriggerVisibility (false);
				}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
				GUILayout.Label ("Collision", buttonWidth);
				if (GUILayout.Button ("On"))
				{
					SetCollisionVisiblity (true);
				}
				if (GUILayout.Button ("Off"))
				{
					SetCollisionVisiblity (false);
				}
			GUILayout.EndHorizontal ();
			
			GUILayout.BeginHorizontal ();
				GUILayout.Label ("Hotspots", buttonWidth);
				if (GUILayout.Button ("On"))
				{
					SetHotspotVisibility (true);
				}
				if (GUILayout.Button ("Off"))
				{
					SetHotspotVisibility (false);
				}
			GUILayout.EndHorizontal ();
			
			GUILayout.Label ("Create new object", EditorStyles.boldLabel);
			
			GUILayout.Label ("Camera", EditorStyles.boldLabel);
			PrefabButton ("Camera", "GameCamera");
			
			GUILayout.Label ("Logic", EditorStyles.boldLabel);
			PrefabButton ("Logic", "ArrowPrompt");
			PrefabButton ("Logic", "Conversation");
			PrefabButton ("Logic", "Cutscene");
			PrefabButton ("Logic", "DialogueOption");
			PrefabButton ("Logic", "Hotspot");
			PrefabButton ("Logic", "Interaction");
			PrefabButton ("Logic", "Sound");
			PrefabButton ("Logic", "Trigger");
			
			GUILayout.Label ("Navigation", EditorStyles.boldLabel);
			PrefabButton ("Navigation", "CollisionCube");
			PrefabButton ("Navigation", "CollisionCylinder");
			PrefabButton ("Navigation", "Marker");
			PrefabButton ("Navigation", "NavMesh");
			PrefabButton ("Navigation", "Path");
			PrefabButton ("Navigation", "PlayerStart");

			if (GUI.changed)
			{
				EditorUtility.SetDirty (gameEngine.GetComponent <SceneSettings>());
				EditorUtility.SetDirty (gameEngine.GetComponent <PlayerMovement>());
			}
		}
	}
	
	
	private void PrefabButton (string subFolder, string prefabName)
	{
		if (GUILayout.Button (prefabName))
		{
			Undo.RegisterSceneUndo ("Create " + prefabName);
			AddPrefab (subFolder, prefabName, true, true, true);
		}	
	}

	
	private void InitialiseObjects ()
	{
		Undo.RegisterSceneUndo ("Organise room objects");
		
		CreateFolder ("_Cameras");
		CreateFolder ("_Cutscenes");
		CreateFolder ("_DialogueOptions");
		CreateFolder ("_Interactions");
		CreateFolder ("_Lights");
		CreateFolder ("_Logic");
		CreateFolder ("_Navigation");
		CreateFolder ("_NPCs");
		CreateFolder ("_Sounds");
		CreateFolder ("_SetGeometry");
		
		// Create subfolders
		CreateSubFolder ("_Cameras", "_GameCameras");

		CreateSubFolder ("_Logic", "_ArrowPrompts");
		CreateSubFolder ("_Logic", "_Conversations");
		CreateSubFolder ("_Logic", "_Hotspots");
		CreateSubFolder ("_Logic", "_Triggers");
		
		CreateSubFolder ("_Navigation", "_CollisionCubes");
		CreateSubFolder ("_Navigation", "_CollisionCylinders");
		CreateSubFolder ("_Navigation", "_Markers");
		CreateSubFolder ("_Navigation", "_NavMesh");
		CreateSubFolder ("_Navigation", "_Paths");
		CreateSubFolder ("_Navigation", "_PlayerStarts");
		
		// Delete default main camera
		if (GameObject.FindWithTag (Tags.mainCamera))
		{
			GameObject mainCam = GameObject.FindWithTag (Tags.mainCamera);
			if (mainCam.GetComponent <MainCamera>() == null)
				DestroyImmediate (mainCam);
		}
		
		// Create main camera
		AddPrefab ("Automatic", "MainCamera", false, false, false);
		PutInFolder (GameObject.FindWithTag (Tags.mainCamera), "_Cameras");
		
		// Create Game engine
		AddPrefab ("Automatic", "GameEngine", false, false, false);
		
		// Assign Player Start
		SceneSettings startSettings = GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>();
		if (startSettings && startSettings.defaultPlayerStart == null)
		{
			PlayerStart playerStart = AddPrefab ("Navigation", "PlayerStart", false, false, true).GetComponent <PlayerStart>();
			startSettings.defaultPlayerStart = playerStart;
		}

	}

	
	private void SetHotspotVisibility (bool isVisible)
	{
		Undo.RegisterSceneUndo ("Hotspot visibility");
		
		Hotspot[] hotspots = FindObjectsOfType (typeof (Hotspot)) as Hotspot[];
		foreach (Hotspot hotspot in hotspots)
		{
			hotspot.showInEditor = isVisible;
			EditorUtility.SetDirty (hotspot);
		}
	}
	
	
	private void SetCollisionVisiblity (bool isVisible)
	{
		Undo.RegisterSceneUndo ("Collision visibility");
		
		_Collision[] colls = FindObjectsOfType (typeof (_Collision)) as _Collision[];
		foreach (_Collision coll in colls)
		{
			coll.showInEditor = isVisible;
			EditorUtility.SetDirty (coll);
		}
	}

	
	private void SetTriggerVisibility (bool isVisible)
	{
		Undo.RegisterSceneUndo ("Trigger visibility");
		
		AC_Trigger[] triggers = FindObjectsOfType(typeof(AC_Trigger)) as AC_Trigger[];
		foreach (AC_Trigger trigger in triggers)
		{
			trigger.showInEditor = isVisible;
			EditorUtility.SetDirty (trigger);
		}
	}
	
	
	private void RenameObject (GameObject ob, string resourceName)
	{
		ob.name = AdvGame.GetName (resourceName);
	}
	

	public GameObject AddPrefab (string folderName, string prefabName, bool canCreateMultiple, bool selectAfter, bool putInFolder)
	{
		if (canCreateMultiple || !GameObject.Find (AdvGame.GetName (prefabName)))
		{
			string fileName = assetFolder + folderName + Path.DirectorySeparatorChar.ToString () + prefabName + ".prefab";

			GameObject newOb = (GameObject) PrefabUtility.InstantiatePrefab (AssetDatabase.LoadAssetAtPath (fileName, typeof (GameObject)));
			newOb.name = "Temp";
			
			if (folderName != "" && putInFolder)
			{
				if (!PutInFolder (newOb, "_" + prefabName + "s"))
				{
					PutInFolder (newOb, "_" + prefabName);
				}
			}
			
			RenameObject (newOb, prefabName);
			
			// Select the object
			if (selectAfter)
			{
				Selection.activeObject = newOb;
			}
			
			return newOb;
		}

		return null;
	}
	
	#endif
	
	
	private bool PutInFolder (GameObject ob, string folderName)
	{
		if (ob && GameObject.Find (folderName))
		{
			if (GameObject.Find (folderName).transform.position == Vector3.zero && folderName.Contains ("_"))
			{
				ob.transform.parent = GameObject.Find (folderName).transform;
				return true;
			}
		}
		
		return false;
	}
	

	private void CreateFolder (string folderName)
	{
		if (!GameObject.Find (folderName))
		{
			GameObject newFolder = new GameObject();
			newFolder.name = folderName;
		}
	}
	
	
	private void CreateSubFolder (string baseFolderName, string subFolderName)
	{
		if (!GameObject.Find (subFolderName))
		{
			GameObject newFolder = new GameObject ();
			newFolder.name = subFolderName;
			
			if (GameObject.Find (baseFolderName))
			{
				newFolder.transform.parent = GameObject.Find (baseFolderName).transform;
			}
			else
			{
				Debug.Log ("Folder " + baseFolderName + " does not exist!");
			}
		}
	}

}