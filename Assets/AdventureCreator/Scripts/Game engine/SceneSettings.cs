/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"SceneSettings.cs"
 * 
 *	This script defines which cutscenes play when the scene is loaded,
 *	and where the player should begin from.
 * 
 */

using UnityEngine;
using System.Collections;

public class SceneSettings : MonoBehaviour
{

	public Cutscene cutsceneOnStart;
	public Cutscene cutsceneOnLoad;
	public PlayerStart defaultPlayerStart;
	public NavigationMesh navMesh;
	
	
	private void Awake ()
	{
		if (navMesh && navMesh.GetComponent <Collider>())
		{
			// Turn off all NavMesh objects, then turn on the selected one
			
			NavigationMesh[] navMeshes = FindObjectsOfType (typeof (NavigationMesh)) as NavigationMesh[];
			foreach (NavigationMesh _navMesh in navMeshes)
			{
				if (navMesh != _navMesh)
				{
					_navMesh.TurnOff ();
				}
			}
			
			navMesh.TurnOn ();
		}	
	}
	
	private void Start ()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>())
		{
			SaveSystem saveSystem = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>();
	
			LevelStorage levelStorage = saveSystem.GetComponent <LevelStorage>();
			if (levelStorage)
			{
				levelStorage.ReturnCurrentLevelData ();	
			}
			
			if (!saveSystem.isLoadingNewScene)
			{
				FindPlayerStart ();
			}
			else
			{
				saveSystem.isLoadingNewScene = false;
			}
		}
	}
	

	private void FindPlayerStart ()
	{
		SceneChanger sceneChanger = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SceneChanger>();
		bool foundStarter = false;
		
		PlayerStart[] starters = FindObjectsOfType (typeof (PlayerStart)) as PlayerStart[];
		foreach (PlayerStart starter in starters)
		{
			if (starter.previousScene > -1 && starter.previousScene == sceneChanger.previousScene)
			{
				foundStarter = true;
				starter.SetPlayerStart ();
				break;
			}
		}

		if (!foundStarter && defaultPlayerStart)
		{
			defaultPlayerStart.SetPlayerStart ();
		}
		
		if (cutsceneOnStart != null)
		{
			cutsceneOnStart.Interact ();
		}
	}
	
	
	public void OnLoad ()
	{
		if (cutsceneOnLoad != null)
		{
			cutsceneOnLoad.Interact ();
		}
	}
}
