/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"SaveSystem.cs"
 * 
 *	This script processes saved game data to and from the scene objects.
 * 
 * 	It is partially based on Zumwalt's code here:
 * 	http://wiki.unity3d.com/index.php?title=Save_and_Load_from_XML
 *  and uses functions by Nitin Pande:
 *  http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SaveSystem : MonoBehaviour
{
	
	public bool isLoadingNewScene { get; set; }

	private string saveDirectory = Application.persistentDataPath;
	private string saveExtention = ".save";
	
	private SaveData saveData;
	
	#if !UNITY_WEBPLAYER
	private LevelStorage levelStorage;
	#endif
	
	
	private void Awake ()
	{
		#if !UNITY_WEBPLAYER
		
		levelStorage = this.GetComponent <LevelStorage>();

		#endif
	}
	
	
	public void LoadGame (int slot)
	{
		#if UNITY_WEBPLAYER
		
		Debug.Log ("Cannot load games when on WebPlayer platform.");
		
		#endif
		
		
		#if !UNITY_WEBPLAYER
		
		if (!HasAutoSave ())
		{
			slot ++;
		}
		
		string allData = Serializer.LoadSaveFile (GetSaveFileName (slot));

		if (allData.ToString () != "")
		{
			string mainData;
			string roomData;
			
			int divider = allData.IndexOf ("||");
			mainData = allData.Substring (0, divider);
			roomData = allData.Substring (divider + 2);
			
			saveData = Serializer.DeserializeObjectBinary <SaveData> (mainData);
			levelStorage.allLevelData = Serializer.DeserializeRoom (roomData);
			
			// Stop any current-running ActionLists, dialogs and interactions
			KillActionLists ();
			
			// Load correct scene
			if (saveData.mainData.currentScene != Application.loadedLevel)
			{
				isLoadingNewScene = true;
				
				if (this.GetComponent <SceneChanger>())
				{
					SceneChanger sceneChanger = this.GetComponent <SceneChanger>();
					sceneChanger.ChangeScene (saveData.mainData.currentScene, false);
				}
			}
			else
			{
				OnLevelWasLoaded ();
			}
			
		}

		#endif
	}
	
	
	private void OnLevelWasLoaded ()
	{
		
		#if !UNITY_WEBPLAYER
		
		if (saveData != null)
		{
			if (GameObject.FindWithTag (Tags.gameEngine))
			{
				if (GameObject.FindWithTag (Tags.gameEngine).GetComponent <Dialog>())
				{
					Dialog dialog = GameObject.FindWithTag (Tags.gameEngine).GetComponent <Dialog>();
					dialog.KillDialog ();
				}
				
				if (GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInteraction>())
				{
					PlayerInteraction playerInteraction = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInteraction>();
					playerInteraction.StopInteraction ();
				}
			}
				
			ReturnMainData ();
			levelStorage.ReturnCurrentLevelData ();
				
			if (GameObject.FindWithTag (Tags.gameEngine) && GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>())
			{
				SceneSettings sceneSettings = GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>();
				sceneSettings.OnLoad ();
			}
		}
		
		saveData = null;
		
		#endif
		
	}
	
	
	public void SaveNewGame ()
	{
		#if UNITY_WEBPLAYER

		Debug.Log ("Cannot save games when on WebPlayer platform.");
		
		#endif
		
		
		#if !UNITY_WEBPLAYER
		
		int slot = GetNumSlots ();
		SaveGame (slot);
		
		#endif
	}
	
	
	public void SaveGame (int slot)
	{
		#if !UNITY_WEBPLAYER
		
		if (!HasAutoSave () || slot == -1)
		{
			slot ++;
		}
		
		levelStorage.StoreCurrentLevelData ();
		
		saveData = new SaveData ();
		
		Player player = GameObject.FindWithTag (Tags.player).GetComponent <Player>();
		PlayerInput playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
		MainCamera mainCamera = GameObject.FindWithTag (Tags.mainCamera).GetComponent <MainCamera>();
		RuntimeInventory runtimeInventory = this.GetComponent <RuntimeInventory>();
		RuntimeVariables runtimeVariables = runtimeInventory.GetComponent <RuntimeVariables>();
		SceneChanger sceneChanger = this.GetComponent <SceneChanger>();
		
		if (player && playerInput && mainCamera && runtimeInventory && runtimeVariables && sceneChanger)
		{
			// Assign "Main Data"
			saveData.mainData.currentScene = Application.loadedLevel;
			saveData.mainData.previousScene = sceneChanger.previousScene;
			
			saveData.mainData.playerLocX = player.transform.position.x;
			saveData.mainData.playerLocY = player.transform.position.y;
			saveData.mainData.playerLocZ = player.transform.position.z;
			saveData.mainData.playerRotY = player.transform.eulerAngles.y;
			
			if (player.GetPath () && player.lockedPath && player.GetPath () != player.GetComponent <Paths>())
			{
				saveData.mainData.playerActivePath = Serializer.GetConstantID (player.GetPath ().gameObject);
			}
			
			if (playerInput.activeArrows)
			{
				saveData.mainData.playerActiveArrows = Serializer.GetConstantID (playerInput.activeArrows.gameObject);
			}
			
			if (playerInput.activeConversation)
			{
				saveData.mainData.playerActiveConversation = Serializer.GetConstantID (playerInput.activeConversation.gameObject);
			}
			
			saveData.mainData.playerUpLock = playerInput.isUpLocked;
			saveData.mainData.playerDownLock = playerInput.isDownLocked;
			saveData.mainData.playerLeftlock = playerInput.isLeftLocked;
			saveData.mainData.playerRightLock = playerInput.isRightLocked;
			saveData.mainData.playerRunLock = (int) playerInput.runLock;
			saveData.mainData.playerInventoryLock = runtimeInventory.isLocked;
			
			saveData.mainData.timeScale = playerInput.timeScale;
			
			if (mainCamera.attachedCamera)
			{
				saveData.mainData.gameCamera = Serializer.GetConstantID (mainCamera.attachedCamera.gameObject);
			}
			
			saveData.mainData.mainCameraLocX = mainCamera.transform.position.x;
			saveData.mainData.mainCameraLocY = mainCamera.transform.position.y;
			saveData.mainData.mainCameraLocZ = mainCamera.transform.position.z;
			
			saveData.mainData.mainCameraRotX = mainCamera.transform.eulerAngles.x;
			saveData.mainData.mainCameraRotY = mainCamera.transform.eulerAngles.y;
			saveData.mainData.mainCameraRotZ = mainCamera.transform.eulerAngles.z;
			
			saveData.mainData.inventoryData = CreateInventoryData (runtimeInventory);
			saveData.mainData.selectedInventoryID = runtimeInventory.selectedID;
			saveData.mainData.variablesData = CreateVariablesData (runtimeVariables);
			
			string mainData = Serializer.SerializeObjectBinary (saveData);
			string levelData = Serializer.SerializeObjectBinary (levelStorage.allLevelData);
			string allData = mainData + "||" + levelData;
	
			Serializer.CreateSaveFile (GetSaveFileName (slot), allData);
		}
		else
		{
			if (player == null)
			{
				Debug.LogWarning ("Save failed - no Player found.");
			}
			if (playerInput == null)
			{
				Debug.LogWarning ("Save failed - no PlayerInput found.");
			}
			if (mainCamera == null)
			{
				Debug.LogWarning ("Save failed - no MainCamera found.");
			}
			if (runtimeInventory == null)
			{
				Debug.LogWarning ("Save failed - no RuntimeInventory found.");
			}
			if (runtimeVariables == null)
			{
				Debug.LogWarning ("Save failed - no RuntimeVariables found.");
			}
			if (sceneChanger == null)
			{
				Debug.LogWarning ("Save failed - no SceneChanger found.");
			}
		}
		
		#endif
	}
	
	
	public static int GetNumSlots ()
	{
		#if !UNITY_WEBPLAYER
		
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>())
		{
			SaveSystem saveSystem = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>();
			DirectoryInfo dir = new DirectoryInfo (saveSystem.saveDirectory);
			FileInfo[] info = dir.GetFiles (saveSystem.GetProjectName() + "_*" + saveSystem.saveExtention);
		
			return info.Length;
		}
		
		#endif
		
		return 0;		
	}
	
	
	public bool HasAutoSave ()
	{
		if (File.Exists (saveDirectory + Path.DirectorySeparatorChar.ToString () + GetProjectName () + "_0" + saveExtention))
		{
			return true;
		}
		
		return false;
	}
	
	
	private string GetProjectName ()
	{
		string[] s = Application.dataPath.Split('/');
		string projectName = s[s.Length - 2];
		return projectName;
	}
	
	
	private string GetSaveFileName (int slot)
	{
		string fileName = saveDirectory + Path.DirectorySeparatorChar.ToString () + GetProjectName () + "_" + slot.ToString () + saveExtention;
		return (fileName);
	}
	
	
	private void KillActionLists ()
	{
		ActionList[] actionLists = FindObjectsOfType (typeof (ActionList)) as ActionList[];
		foreach (ActionList actionList in actionLists)
		{
			if (actionList.IsRunning ())
			{
				actionList.Kill ();
			}
		}
	}

	
	#if !UNITY_WEBPLAYER
	
	public static string GetSaveSlotName (int slot)
	{
		SaveSystem saveSystem = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>();
		
		DirectoryInfo dir = new DirectoryInfo (saveSystem.saveDirectory);
		FileInfo[] info = dir.GetFiles (saveSystem.GetProjectName() + "_*" + saveSystem.saveExtention);
		
		string fileName = "Save " + (slot).ToString ();
		
		if (!saveSystem.HasAutoSave ())
		{
			fileName = "Save " + (slot + 1).ToString ();
		}
		
		if (slot == 0 && saveSystem.HasAutoSave())
		{
			fileName = "Autosave";
		}
		
		if (slot < info.Length)
		{
			fileName += " (" + info[slot].LastWriteTime.ToString () + ")";
		}
		
		return fileName;
	}
	
	
	private void ReturnMainData ()
	{
		Player player = GameObject.FindWithTag (Tags.player).GetComponent <Player>();
		PlayerInput playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
		MainCamera mainCamera = GameObject.FindWithTag (Tags.mainCamera).GetComponent <MainCamera>();
		RuntimeInventory runtimeInventory = this.GetComponent <RuntimeInventory>();
		RuntimeVariables runtimeVariables = runtimeInventory.GetComponent <RuntimeVariables>();
		SceneChanger sceneChanger = this.GetComponent <SceneChanger>();
		
		if (player && playerInput && mainCamera && runtimeInventory && runtimeVariables)
		{
			sceneChanger.previousScene = saveData.mainData.previousScene;
			
			player.transform.position = new Vector3 (saveData.mainData.playerLocX, saveData.mainData.playerLocY, saveData.mainData.playerLocZ);
			player.transform.eulerAngles = new Vector3 (0f, saveData.mainData.playerRotY, 0f);
			player.SetLookDirection (Vector3.zero, true);
			
			// Active path
			player.Halt ();
			Paths savedPath = Serializer.returnComponent <Paths> (saveData.mainData.playerActivePath);
			if (savedPath)
			{
				player.SetLockedPath (savedPath);
			}
			
			// Active screen arrows
			playerInput.RemoveActiveArrows ();
			ArrowPrompt loadedArrows = Serializer.returnComponent <ArrowPrompt> (saveData.mainData.playerActiveArrows);
			if (loadedArrows)
			{
				loadedArrows.TurnOn ();
			}
			
			// Active conversation
			playerInput.activeConversation = Serializer.returnComponent <Conversation> (saveData.mainData.playerActiveConversation);
			
			playerInput.isUpLocked = saveData.mainData.playerUpLock;
			playerInput.isDownLocked = saveData.mainData.playerDownLock;
			playerInput.isLeftLocked = saveData.mainData.playerLeftlock;
			playerInput.isRightLocked = saveData.mainData.playerRightLock;
			playerInput.runLock = (PlayerMoveLock) saveData.mainData.playerRunLock;
			runtimeInventory.isLocked = saveData.mainData.playerInventoryLock;
			
			playerInput.timeScale = saveData.mainData.timeScale;
			
			mainCamera.SetGameCamera (Serializer.returnComponent <GameCamera> (saveData.mainData.gameCamera));
			mainCamera.transform.position = new Vector3 (saveData.mainData.mainCameraLocX, saveData.mainData.mainCameraLocY, saveData.mainData.mainCameraLocZ);
			mainCamera.transform.eulerAngles = new Vector3 (saveData.mainData.mainCameraRotX, saveData.mainData.mainCameraRotY, saveData.mainData.mainCameraRotZ);
			if (mainCamera.attachedCamera)
			{
				mainCamera.attachedCamera.MoveCameraInstant ();
			}
			else
			{
				Debug.LogWarning ("MainCamera has no attached GameCamera");
			}
			
			// Inventory
			AssignInventory (runtimeInventory, saveData.mainData.inventoryData);
			if (saveData.mainData.selectedInventoryID > -1)
			{
				runtimeInventory.SelectItemByID (saveData.mainData.selectedInventoryID);
			}
			else
			{
				runtimeInventory.SetNull ();
			}
			
			// Variables
			AssignVariables (runtimeVariables, saveData.mainData.variablesData);
			
			// StateHandler
			StateHandler stateHandler = runtimeInventory.GetComponent <StateHandler>();
			if (playerInput.activeConversation)
			{
				stateHandler.gameState = GameState.DialogOptions;
			}
			else
			{
				stateHandler.gameState = GameState.Normal;
			}
			
			// Fade in camera
			mainCamera.FadeIn (0.5f);
			
			playerInput.ResetClick ();
		}
		else
		{
			if (player == null)
			{
				Debug.LogWarning ("Load failed - no Player found.");
			}
			if (playerInput == null)
			{
				Debug.LogWarning ("Load failed - no PlayerInput found.");
			}
			if (mainCamera == null)
			{
				Debug.LogWarning ("Load failed - no MainCamera found.");
			}
			if (runtimeInventory == null)
			{
				Debug.LogWarning ("Load failed - no RuntimeInventory found.");
			}
			if (runtimeVariables == null)
			{
				Debug.LogWarning ("Load failed - no RuntimeVariables found.");
			}
			if (sceneChanger == null)
			{
				Debug.LogWarning ("Load failed - no SceneChanger found.");
			}
		}
	}
	
	
	private void AssignVariables (RuntimeVariables runtimeVariables, string variablesData)
	{
		if (runtimeVariables)
		{
			if (variablesData.Length > 0)
			{
				string[] varsArray = variablesData.Split ("|"[0]);
				
				foreach (string chunk in varsArray)
				{
					string[] chunkData = chunk.Split (":"[0]);
					
					int _id = 0;
					int.TryParse (chunkData[0], out _id);
		
					int _value = 0;
					int.TryParse (chunkData[1], out _value);
					
					runtimeVariables.SetValue (_id, _value, false);
				}
			}
		}
	}

	
	private void AssignInventory (RuntimeInventory runtimeInventory, string inventoryData)
	{
		if (runtimeInventory)
		{
			runtimeInventory.localItems.Clear ();
			
			if (inventoryData.Length > 0)
			{
				string[] countArray = inventoryData.Split ("|"[0]);
				
				foreach (string chunk in countArray)
				{
					string[] chunkData = chunk.Split (":"[0]);
					
					int _id = 0;
					int.TryParse (chunkData[0], out _id);
		
					int _count = 0;
					int.TryParse (chunkData[1], out _count);
					
					runtimeInventory.Add (_id, _count);
				}
			}
		}
	}

	
	private string CreateInventoryData (RuntimeInventory runtimeInventory)
	{
		System.Text.StringBuilder inventoryString = new System.Text.StringBuilder();
		
		foreach (InvItem item in runtimeInventory.localItems)
		{
			inventoryString.Append (item.id.ToString ());
			inventoryString.Append (":");
			inventoryString.Append (item.count.ToString ());
			inventoryString.Append ("|");
		}
		
		if (runtimeInventory && runtimeInventory.localItems.Count > 0)
		{
			inventoryString.Remove (inventoryString.Length-1, 1);
		}
		
		return inventoryString.ToString ();		
	}
	
		
	private string CreateVariablesData (RuntimeVariables runtimeVariables)
	{
		System.Text.StringBuilder variablesString = new System.Text.StringBuilder();
		
		foreach (GVar _var in runtimeVariables.localVars)
		{
			variablesString.Append (_var.id.ToString ());
			variablesString.Append (":");
			variablesString.Append (_var.val.ToString());
			variablesString.Append ("|");
		}
		
		if (runtimeVariables && runtimeVariables.localVars.Count > 0)
		{
			variablesString.Remove (variablesString.Length-1, 1);
		}
		
		return variablesString.ToString ();		
	}
	
	#endif

}
