/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"LevelStorage.cs"
 * 
 *	This script handles the loading and unloading of per-scene data.
 *	Below the main class is a series of data classes for the different object types.
 * 
 */

using UnityEngine;
using System.Collections.Generic;

public class LevelStorage : MonoBehaviour
{
	
	[HideInInspector] public List<SingleLevelData> allLevelData;
	
	
	private void Awake ()
	{
		allLevelData = new List<SingleLevelData>();
	}
	
	
	public void ReturnCurrentLevelData ()
	{
		foreach (SingleLevelData levelData in allLevelData)
		{
			if (levelData.sceneNumber == Application.loadedLevel)
			{
				UnloadNavMesh (levelData.navMesh);
				UnloadTransformData (levelData.transforms);
				UnloadConversationData (levelData.conversations);
				UnloadHotspotData (levelData.hotspots);
				UnloadNPCData (levelData.npcs);
				break;
			}
		}
	}
	
	
	public void StoreCurrentLevelData ()
	{
		List <TransformData> thisLevelTransforms = PopulateTransformData ();
		List <ConversationData> thisLevelConversations = PopulateConversationData ();
		List <HotspotData> thisLevelHotspots = PopulateHotspotData ();
		List <NPCData> thisLevelNPCs = PopulateNPCData ();
		
		SingleLevelData thisLevelData = new SingleLevelData ();
		thisLevelData.sceneNumber = Application.loadedLevel;
		
		SceneSettings sceneSettings = GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>();
		if (sceneSettings && sceneSettings.navMesh && sceneSettings.navMesh.GetComponent <ConstantID>())
		{
			thisLevelData.navMesh = Serializer.GetConstantID (sceneSettings.navMesh.gameObject);
		}
			
		thisLevelData.transforms = thisLevelTransforms;
		thisLevelData.hotspots = thisLevelHotspots;
		thisLevelData.conversations = thisLevelConversations;
		thisLevelData.npcs = thisLevelNPCs;
		
		bool found = false;
		for (int i=0; i<allLevelData.Count; i++)
		{
			if (allLevelData[i].sceneNumber == Application.loadedLevel)
			{
				allLevelData[i] = thisLevelData;
				found = true;
				break;
			}
		}
		
		if (!found)
		{
			allLevelData.Add (thisLevelData);
		}
	}

	
	private void UnloadNavMesh (int navMeshInt)
	{
		NavigationMesh navMesh = Serializer.returnComponent <NavigationMesh> (navMeshInt);
		SceneSettings sceneSettings = GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>();
		
		if (navMesh && navMesh.collider && sceneSettings)
		{
			NavigationMesh oldNavMesh = sceneSettings.navMesh;
			oldNavMesh.TurnOff ();
			navMesh.collider.GetComponent <NavigationMesh>().TurnOn ();
			
			sceneSettings.navMesh = navMesh;
		}
	}	
	
	
	private void UnloadTransformData (List <TransformData> _transforms)
	{
		foreach (TransformData _transform in _transforms)
		{
			RememberTransform saveObject = Serializer.returnComponent <RememberTransform> (_transform.objectID);
			
			if (saveObject != null)
			{
				saveObject.transform.position = new Vector3 (_transform.LocX, _transform.LocY, _transform.LocZ);
				saveObject.transform.eulerAngles = new Vector3 (_transform.RotX, _transform.RotY, _transform.RotZ);
				saveObject.transform.localScale = new Vector3 (_transform.ScaleX, _transform.ScaleY, _transform.ScaleZ);
			}
		}
	}
	
	
	private void UnloadNPCData (List <NPCData> _npcs)
	{
		foreach (NPCData _npc in _npcs)
		{
			RememberNPC saveObject = Serializer.returnComponent <RememberNPC> (_npc.objectID);
			
			if (saveObject != null)
			{
				if (_npc.isOn)
				{
					saveObject.gameObject.layer = LayerMask.NameToLayer ("Default");
				}
				else
				{
					saveObject.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
				}
				
				saveObject.transform.position = new Vector3 (_npc.LocX, _npc.LocY, _npc.LocZ);
				saveObject.transform.eulerAngles = new Vector3 (_npc.RotX, _npc.RotY, _npc.RotZ);
				saveObject.transform.localScale = new Vector3 (_npc.ScaleX, _npc.ScaleY, _npc.ScaleZ);
				
				if (_npc.pathID != 0)
				{
					Paths pathObject = Serializer.returnComponent <Paths> (_npc.pathID);
					
					if (pathObject)
					{
						saveObject.GetComponent <Char>().SetPath (pathObject, _npc.targetNode, _npc.prevNode);
					}
					else
					{
						Debug.LogWarning ("Trying to assign a path for NPC " + saveObject.name + ", but the path was not found - was it deleted?");
					}
				}
			}
		}
	}
	
	
	private void UnloadHotspotData (List <HotspotData> _hotspots)
	{
		foreach (HotspotData _hotspot in _hotspots)
		{
			RememberHotspot saveObject = Serializer.returnComponent <RememberHotspot> (_hotspot.objectID);
			
			if (saveObject != null)
			{
				if (_hotspot.isOn)
				{
					saveObject.gameObject.layer = LayerMask.NameToLayer ("Default");
				}
				else
				{
					saveObject.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
				}
			}
		}
	}
	
	
	private void UnloadConversationData (List <ConversationData> _conversations)
	{
		foreach (ConversationData _conversation in _conversations)
		{
			RememberConversation saveObject = Serializer.returnComponent <RememberConversation> (_conversation.objectID);
			
			if (saveObject != null)
			{
				saveObject.GetComponent <Conversation>().SetOptionStates (_conversation.optionStates);
				saveObject.GetComponent <Conversation>().SetOptionLocks (_conversation.optionLocks);
			}
		}
	}
	
	
	private List <TransformData> PopulateTransformData ()
	{
		List <TransformData> allTransformData = new List<TransformData>();
		
		RememberTransform[] transforms = FindObjectsOfType (typeof (RememberTransform)) as RememberTransform[];
		
		foreach (RememberTransform _transform in transforms)
		{
			if (_transform.constantID != 0)
			{
				TransformData transformData = new TransformData();
			
				transformData.objectID = _transform.constantID;
				
				transformData.LocX = _transform.transform.position.x;
				transformData.LocY = _transform.transform.position.y;
				transformData.LocZ = _transform.transform.position.z;
	
				transformData.RotX = _transform.transform.eulerAngles.x;
				transformData.RotY = _transform.transform.eulerAngles.y;
				transformData.RotZ = _transform.transform.eulerAngles.z;
				
				transformData.ScaleX = _transform.transform.localScale.x;
				transformData.ScaleY = _transform.transform.localScale.y;
				transformData.ScaleZ = _transform.transform.localScale.z;
				
				allTransformData.Add (transformData);
			}
			else
			{
				Debug.LogWarning ("GameObject " + _transform.name + " was not saved because it's ConstantID has not been set!");
			}
		}
		
		return allTransformData;
	}
	
	
	private List <NPCData> PopulateNPCData ()
	{
		List <NPCData> allNPCData = new List<NPCData>();
		
		RememberNPC[] npcs = FindObjectsOfType (typeof (RememberNPC)) as RememberNPC[];
		
		foreach (RememberNPC _npc in npcs)
		{
			if (_npc.constantID != 0)
			{
				NPCData npcData = new NPCData();
			
				npcData.objectID = _npc.constantID;
				
				if (_npc.gameObject.layer == LayerMask.NameToLayer ("Default"))
				{
					npcData.isOn = true;
				}
				else
				{
					npcData.isOn = false;
				}
				
				npcData.LocX = _npc.transform.position.x;
				npcData.LocY = _npc.transform.position.y;
				npcData.LocZ = _npc.transform.position.z;
	
				npcData.RotX = _npc.transform.eulerAngles.x;
				npcData.RotY = _npc.transform.eulerAngles.y;
				npcData.RotZ = _npc.transform.eulerAngles.z;
				
				npcData.ScaleX = _npc.transform.localScale.x;
				npcData.ScaleY = _npc.transform.localScale.y;
				npcData.ScaleZ = _npc.transform.localScale.z;
				
				if (_npc.GetComponent <Char>().GetPath () && _npc.GetComponent <Char>().GetPath () != _npc.GetComponent <Paths>())
				{
					if (_npc.GetComponent <Char>().GetPath ().GetComponent <ConstantID>())
					{
						npcData.pathID = _npc.GetComponent <Char>().GetPath ().GetComponent <ConstantID>().constantID;
					}
					else
					{
						Debug.LogWarning ("Want to save path data for " + _npc.name + " but path has no ID!");
					}
				}
				
				npcData.targetNode = _npc.GetComponent <Char>().GetTargetNode ();
				npcData.prevNode = _npc.GetComponent <Char>().GetPrevNode ();
				
				allNPCData.Add (npcData);
			}
			else
			{
				Debug.LogWarning ("GameObject " + _npc.name + " was not saved because it's ConstantID has not been set!");
			}
		}
		
		return allNPCData;
	}
	
	
	private List <HotspotData> PopulateHotspotData ()
	{
		List <HotspotData> allHotspotData = new List<HotspotData>();
		
		RememberHotspot[] hotspots = FindObjectsOfType (typeof (RememberHotspot)) as RememberHotspot[];
		
		foreach (RememberHotspot _hotspot in hotspots)
		{
			if (_hotspot.constantID != 0)
			{
				HotspotData hotspotData = new HotspotData ();
				hotspotData.objectID = _hotspot.constantID;
				
				if (_hotspot.gameObject.layer == LayerMask.NameToLayer ("Default"))
				{
					hotspotData.isOn = true;
				}
				else
				{
					hotspotData.isOn = false;
				}
				
				allHotspotData.Add (hotspotData);
			}
			else
			{
				Debug.LogWarning ("GameObject " + _hotspot.name + " was not saved because it's ConstantID has not been set!");
			}
		}
		
		return allHotspotData;
	}
	
	
	private List <ConversationData> PopulateConversationData ()
	{
		List <ConversationData> allConversationData = new List<ConversationData>();
		
		RememberConversation[] conversations = FindObjectsOfType (typeof (RememberConversation)) as RememberConversation[];
		
		foreach (RememberConversation _conversation in conversations)
		{
			if (_conversation.constantID != 0)
			{
				ConversationData conversationData = new ConversationData();
				conversationData.objectID = _conversation.constantID;
				
				conversationData.optionStates = _conversation.GetComponent <Conversation>().GetOptionStates ();
				conversationData.optionLocks = _conversation.GetComponent <Conversation>().GetOptionLocks ();
				
				allConversationData.Add (conversationData);
			}
			else
			{
				Debug.LogWarning ("GameObject " + _conversation.name + " was not saved because it's ConstantID has not been set!");
			}
		}
		
		return allConversationData;
	}
}
	

[System.Serializable]
public class SingleLevelData
{
	
	public List<TransformData> transforms;
	public List<ConversationData> conversations;
	public List<HotspotData> hotspots;
	public List<NPCData> npcs;
	public int sceneNumber;
	public int navMesh;
	
	public SingleLevelData () { }
	
}


[System.Serializable]
public class TransformData
{
	
	public int objectID;
	
	public float LocX;
	public float LocY;
	public float LocZ;
	
	public float RotX;
	public float RotY;
	public float RotZ;
	
	public float ScaleX;
	public float ScaleY;
	public float ScaleZ;
	
	public TransformData () { }
	
}


[System.Serializable]
public class ConversationData
{
	public int objectID;
	public List<bool> optionStates;
	public List<bool> optionLocks;
	
	public ConversationData () { }
}


[System.Serializable]
public class HotspotData
{
	public int objectID;
	public bool isOn;
	
	public HotspotData () { }
}


[System.Serializable]
public class NPCData
{
	
	public int objectID;
	
	public bool isOn;
	
	public float LocX;
	public float LocY;
	public float LocZ;
	
	public float RotX;
	public float RotY;
	public float RotZ;
	
	public float ScaleX;
	public float ScaleY;
	public float ScaleZ;
	
	public int pathID;
	public int targetNode;
	public int prevNode;
	
	public NPCData () { }
	
}