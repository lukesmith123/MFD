/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionNavMesh.cs"
 * 
 *	This action changes the active NavMesh.
 *	All NavMeshes must be on the same unique layer.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionNavMesh : Action
{
	
	public NavigationMesh newNavMesh;
	
	
	public ActionNavMesh ()
	{
		this.isDisplayed = true;
		title = "Engine: Change NavMesh";
	}
	
	
	override public float Run ()
	{
		if (newNavMesh)
		{
			SceneSettings sceneSettings = GameObject.FindWithTag (Tags.gameEngine).GetComponent <SceneSettings>();
			NavigationMesh oldNavMesh = sceneSettings.navMesh;
			oldNavMesh.TurnOff ();
			newNavMesh.TurnOn ();
			sceneSettings.navMesh = newNavMesh;
		}
		
		return 0f;
	}
	

	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		newNavMesh = (NavigationMesh) EditorGUILayout.ObjectField ("New NavMesh:", newNavMesh, typeof (NavigationMesh), true);
		
		AfterRunningOption ();
	}
	
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (newNavMesh)
		{
			labelAdd = " (" + newNavMesh.gameObject.name + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}