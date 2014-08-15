/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"SettingsManager.cs"
 * 
 *	This script handles the "Settings" tab of the main wizard.
 *	It is used to define the player, and control methods of the game.
 * 
 */

using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SettingsManager : ScriptableObject
{
	
	// Runtime resources
	public Player player;
	
	// Control settings
	public ControlStyle controlStyle;
	public InputType inputType;
	
	// TouchScreen settings
	public float freeAimTouchSpeed = 0.1f;
	public float dragWalkThreshold = 5f;
	public float dragRunThreshold = 20f;
	public bool drawDragLine = false;
	public float dragLineWidth = 3f;
	public Color dragLineColor = Color.white;
	
	// Interface settings
	public bool alwaysOnInventory = true;
	
	// Raycast settings
	public float navMeshRaycastLength = 100f;
	public float hotspotRaycastLength = 100f;
	
	// Speech settings
	public float textScrollSpeed = 50;
	public float screenTimeFactor = 0.1f;
	public bool allowSpeechSkipping;
	public bool searchAudioFiles = true;
	public bool forceSubtitles = true;
	public bool translateAudio = true;
	
	// Cursor settings
	public float normalCursorSize = 0.015f;
	public float iconCursorSize = 0.04f;
	public float inventoryCursorSize = 0.06f;
	public bool allowMainCursor = false;
	public InventoryHandling inventoryHandling = InventoryHandling.ChangeCursor;
	public bool allowInteractionCursor = false;
	public Texture2D useTexture;
	public Texture2D lookTexture;
	public Texture2D talkTexture;
	public Texture2D pointerTexture;
	

	#if UNITY_EDITOR
	
	public void ShowGUI ()
	{
		EditorGUILayout.LabelField ("Runtime resources", EditorStyles.boldLabel);
		player = (Player) EditorGUILayout.ObjectField ("Player:", player, typeof (Player), false);
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.LabelField ("Control settings", EditorStyles.boldLabel);
		
		controlStyle = (ControlStyle) EditorGUILayout.EnumPopup ("Control style:", controlStyle);
		inputType = (InputType) EditorGUILayout.EnumPopup ("Input type:", inputType);
		
		if (inputType == InputType.TouchScreen)
		{
			EditorGUILayout.Space ();
		
			EditorGUILayout.LabelField ("Touch-screen settings", EditorStyles.boldLabel);
			
			dragWalkThreshold = EditorGUILayout.FloatField ("Walk threshold:", dragWalkThreshold);
			dragRunThreshold = EditorGUILayout.FloatField ("Run threshold:", dragRunThreshold);
			
			if (controlStyle == ControlStyle.FirstPerson)
			{
				freeAimTouchSpeed = EditorGUILayout.FloatField ("Freelook speed:", freeAimTouchSpeed);
			}
			
			if (controlStyle != ControlStyle.PointAndClick)
			{
				drawDragLine = EditorGUILayout.BeginToggleGroup ("Draw drag line?", drawDragLine);
					dragLineWidth = EditorGUILayout.FloatField ("Drag line width:", dragLineWidth);
					dragLineColor = EditorGUILayout.ColorField ("Drag line colour:", dragLineColor);
				EditorGUILayout.EndToggleGroup ();				
			}
		}
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.LabelField ("Interface settings", EditorStyles.boldLabel);
		
		alwaysOnInventory = EditorGUILayout.Toggle ("Always show inventory?", alwaysOnInventory);
	
		EditorGUILayout.Space ();
		
		EditorGUILayout.LabelField ("Raycast settings", EditorStyles.boldLabel);
		
		navMeshRaycastLength = EditorGUILayout.FloatField ("NavMesh ray length:", navMeshRaycastLength);
		hotspotRaycastLength = EditorGUILayout.FloatField ("Hotspot ray length:", hotspotRaycastLength);
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.LabelField ("Speech settings", EditorStyles.boldLabel);
		
		textScrollSpeed = EditorGUILayout.FloatField ("Text scroll speed:", textScrollSpeed);
		screenTimeFactor = EditorGUILayout.FloatField ("Display factor:", screenTimeFactor);
		allowSpeechSkipping = EditorGUILayout.Toggle ("Allow speech skipping?", allowSpeechSkipping);
		forceSubtitles = EditorGUILayout.Toggle ("Force subtitles if no audio?", forceSubtitles);
		searchAudioFiles = EditorGUILayout.Toggle ("Auto-play audio files?", searchAudioFiles);
		translateAudio = EditorGUILayout.Toggle ("Audio translations?", translateAudio);
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.LabelField ("Cursor settings", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginVertical ("Button");
			allowMainCursor = EditorGUILayout.Toggle ("Replace mouse cursor?", allowMainCursor);
			if (allowMainCursor || inputType == InputType.Controller)
			{
				pointerTexture = (Texture2D) EditorGUILayout.ObjectField ("Main cursor texture:", pointerTexture, typeof (Texture2D), false);
				normalCursorSize = EditorGUILayout.FloatField ("Main cursor size:", normalCursorSize);
			}
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.BeginVertical ("Button");
			inventoryHandling = (InventoryHandling) EditorGUILayout.EnumPopup ("When inventory selected:", inventoryHandling);
			if (inventoryHandling == InventoryHandling.ChangeCursor || inventoryHandling == InventoryHandling.ChangeCursorAndHotspotLabel)
			{
				inventoryCursorSize = EditorGUILayout.FloatField ("Inventory cursor size:", inventoryCursorSize);
			}
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.BeginVertical ("Button");
			allowInteractionCursor = EditorGUILayout.Toggle ("Change for interactions?", allowInteractionCursor);
			if (allowInteractionCursor)
			{
				iconCursorSize = EditorGUILayout.FloatField ("Interaction cursor size:", iconCursorSize);
				useTexture = (Texture2D) EditorGUILayout.ObjectField ("Use icon texture:", useTexture, typeof (Texture2D), false);
				lookTexture = (Texture2D) EditorGUILayout.ObjectField ("Look icon texture:", lookTexture, typeof (Texture2D), false);
				talkTexture = (Texture2D) EditorGUILayout.ObjectField ("Talk icon texture:", talkTexture, typeof (Texture2D), false);
			}
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.Space ();
		
		if (GUILayout.Button ("Clear options cache"))
		{
			PlayerPrefs.DeleteKey ("Options");
			Debug.Log ("PlayerPrefs cleared");
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (this);
		}
	}
	
	#endif
	
}