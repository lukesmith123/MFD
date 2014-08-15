using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

[CustomEditor (typeof (Hotspot))]
public class HotspotEditor : Editor
{
	
	private Hotspot _target;
	
	private InventoryManager inventoryManager;
	
	private static GUIContent
		deleteContent = new GUIContent("-", "Delete this interaction"),
		addContent = new GUIContent("+", "Create this interaction");

	private static GUILayoutOption
		autoWidth = GUILayout.MaxWidth (90f),
		buttonWidth = GUILayout.MaxWidth (20f);
	
	
	private void OnEnable ()
	{
		_target = (Hotspot) target;
	}
	
	
	public override void OnInspectorGUI()
	{
		
		if (AdvGame.GetReferences () == null)
		{
			Debug.LogError ("A References file is required - please use the Adventure Creator window to create one.");
			EditorGUILayout.LabelField ("No References file found!");
		}
		else
		{
			if (!inventoryManager)
			{
				inventoryManager = AdvGame.GetReferences ().inventoryManager;
			}
	
			_target.hotspotName = EditorGUILayout.TextField ("Label (if not object name):", _target.hotspotName);
			_target.highlight = (Highlight) EditorGUILayout.ObjectField ("Object to highlight:", _target.highlight, typeof (Highlight), true);
			_target.walkToMarker = (Marker) EditorGUILayout.ObjectField ("Walk-to marker:", _target.walkToMarker, typeof (Marker), true);
			
			// Use
			EditorGUILayout.BeginVertical("Button");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Use interaction", EditorStyles.boldLabel);
			
			if (!_target.provideUseInteraction)
			{
				if (GUILayout.Button (addContent, EditorStyles.miniButtonRight, buttonWidth))
				{
					Undo.RegisterSceneUndo ("Create use interaction");
					_target.provideUseInteraction = true;
				}
			}
			else
			{
				if (GUILayout.Button (deleteContent, EditorStyles.miniButtonRight, buttonWidth))
				{
					Undo.RegisterSceneUndo ("Delete use interaction");
					_target.provideUseInteraction = false;
				}
			}
			
			EditorGUILayout.EndHorizontal ();
			if (_target.provideUseInteraction)
			{
				EditorGUILayout.BeginHorizontal ();
				_target.useButton.interaction = (Interaction) EditorGUILayout.ObjectField ("Interaction:", _target.useButton.interaction, typeof (Interaction), true);
				
				if (_target.useButton.interaction == null)
				{
					if (GUILayout.Button ("Auto-create", autoWidth))
					{
						Undo.RegisterSceneUndo ("Auto-create interaction");
						Interaction newInteraction = AdvGame.GetReferences ().sceneManager.AddPrefab ("Logic", "Interaction", true, false, true).GetComponent <Interaction>();
						
						newInteraction.gameObject.name = AdvGame.UniqueName (_target.gameObject.name + "_Use");
						_target.useButton.interaction = newInteraction;
					}
				}
				EditorGUILayout.EndHorizontal ();
				
				_target.useIcon = (InteractionIcon) EditorGUILayout.EnumPopup ("Icon:", _target.useIcon);
				_target.useButton.playerAction = (PlayerAction) EditorGUILayout.EnumPopup ("Player action:", _target.useButton.playerAction);
				
				if (_target.useButton.playerAction == PlayerAction.WalkTo)
				{
					EditorGUILayout.BeginVertical ("Button");
						_target.useButton.setProximity = EditorGUILayout.BeginToggleGroup ("Set minimum distance?", _target.useButton.setProximity);
						_target.useButton.proximity = EditorGUILayout.FloatField ("Proximity:", _target.useButton.proximity);
						EditorGUILayout.EndToggleGroup ();
					EditorGUILayout.EndVertical ();
				}
	
			}
			EditorGUILayout.EndVertical ();
			
			
			// Look
			EditorGUILayout.Space ();
			EditorGUILayout.BeginVertical("Button");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Examine interaction", EditorStyles.boldLabel);
			
			if (!_target.provideLookInteraction)
			{
				if (GUILayout.Button (addContent, EditorStyles.miniButtonRight, buttonWidth))
				{
					Undo.RegisterSceneUndo ("Create examine interaction");
					_target.provideLookInteraction = true;
				}
			}
			else
			{
				if (GUILayout.Button (deleteContent, EditorStyles.miniButtonRight, buttonWidth))
				{
					Undo.RegisterSceneUndo ("Delete examine interaction");
					_target.provideLookInteraction = false;
				}
			}
			
			EditorGUILayout.EndHorizontal ();
			if (_target.provideLookInteraction)
			{
				EditorGUILayout.BeginHorizontal ();
				
				_target.lookButton.interaction = (Interaction) EditorGUILayout.ObjectField ("Interaction:", _target.lookButton.interaction, typeof (Interaction), true);
				
				if (_target.lookButton.interaction == null)
				{
					if (GUILayout.Button ("Auto-create", autoWidth))
					{
						Undo.RegisterSceneUndo ("Auto-create interaction");
						Interaction newInteraction = AdvGame.GetReferences ().sceneManager.AddPrefab ("Logic", "Interaction", true, false, true).GetComponent <Interaction>();
						
						newInteraction.gameObject.name = AdvGame.UniqueName (_target.gameObject.name + "_Look");
						_target.lookButton.interaction = newInteraction;
					}
				}
				
				EditorGUILayout.EndHorizontal ();
				
				_target.lookButton.playerAction = (PlayerAction) EditorGUILayout.EnumPopup ("Player action:", _target.lookButton.playerAction);
				
				if (_target.lookButton.playerAction == PlayerAction.WalkTo)
				{
					EditorGUILayout.BeginVertical ("Button");
						_target.lookButton.setProximity = EditorGUILayout.BeginToggleGroup ("Set minimum distance?", _target.lookButton.setProximity);
						_target.lookButton.proximity = EditorGUILayout.FloatField ("Proximity:", _target.lookButton.proximity);
						EditorGUILayout.EndToggleGroup ();
					EditorGUILayout.EndVertical ();
				}
			}
			EditorGUILayout.EndVertical ();
			
			// Inventory
			EditorGUILayout.Space ();
			EditorGUILayout.BeginVertical("Button");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Inventory interactions", EditorStyles.boldLabel);
			
			if (GUILayout.Button (addContent, EditorStyles.miniButtonRight, buttonWidth))
			{
				Undo.RegisterSceneUndo ("Create inventory interaction");
				_target.invButtons.Add (new Button ());
				_target.provideInvInteraction = true;
			}
			EditorGUILayout.EndHorizontal();
	
			if (_target.provideInvInteraction)
			{
				if (inventoryManager)
				{
					// Create a string List of the field's names (for the PopUp box)
					List<string> labelList = new List<string>();
					int invNumber;
					
					if (inventoryManager.items.Count > 0)
					{
					
						foreach (InvItem _item in inventoryManager.items)
						{
							labelList.Add (_item.label);
						}
						
						foreach (Button invButton in _target.invButtons)
						{
							invNumber = -1;
							
							int j = 0;
							foreach (InvItem _item in inventoryManager.items)
							{
								// If an item has been removed, make sure selected variable is still valid
								if (_item.id == invButton.invID)
								{
									invNumber = j;
									break;
								}
								
								j++;
							}
							
							if (invNumber == -1)
							{
								// Wasn't found (item was deleted?), so revert to zero
								Debug.Log ("Previously chosen item no longer exists!");
								invNumber = 0;
								invButton.invID = 0;
							}
							
							EditorGUILayout.Space ();
							EditorGUILayout.BeginHorizontal ();
							
							invNumber = EditorGUILayout.Popup ("Inventory item:", invNumber, labelList.ToArray());
							
							// Re-assign variableID based on PopUp selection
							invButton.invID = inventoryManager.items[invNumber].id;
							
							if (GUILayout.Button (deleteContent, EditorStyles.miniButtonRight, buttonWidth))
							{
								Undo.RegisterSceneUndo ("Delete inventory interaction");
								
								_target.invButtons.Remove (invButton);
								
								if (_target.invButtons.Count == 0)
								{
									_target.provideInvInteraction = false;
								}
								
								break;
							}
							
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							
							invButton.interaction = (Interaction) EditorGUILayout.ObjectField ("Interaction:", invButton.interaction, typeof (Interaction), true);
							
							if (invButton.interaction == null)
							{
								if (GUILayout.Button ("Auto-create", autoWidth))
								{
									Undo.RegisterSceneUndo ("Create Interaction");
									Interaction newInteraction = AdvGame.GetReferences ().sceneManager.AddPrefab ("Logic", "Interaction", true, false, true).GetComponent <Interaction>();
									
									newInteraction.gameObject.name = AdvGame.UniqueName (_target.gameObject.name + "_Inv");
									invButton.interaction = newInteraction;
								}
							}
							
							EditorGUILayout.EndHorizontal ();
										
							invButton.playerAction = (PlayerAction) EditorGUILayout.EnumPopup ("Player action:", invButton.playerAction);
				
							if (invButton.playerAction == PlayerAction.WalkTo)
							{
								EditorGUILayout.BeginVertical ("Button");
									invButton.setProximity = EditorGUILayout.BeginToggleGroup ("Set minimum distance?", invButton.setProximity);
									invButton.proximity = EditorGUILayout.FloatField ("Proximity:", invButton.proximity);
									EditorGUILayout.EndToggleGroup ();
								EditorGUILayout.EndVertical ();
							}
						}
		
					}					
					else
					{
						EditorGUILayout.LabelField ("No inventory items exist!");
						invNumber = -1;
						
						for (int i=0; i<_target.invButtons.Count; i++)
						{
							_target.invButtons[i].invID = -1;
						}
					}
				}
				else
				{
					Debug.LogWarning ("An InventoryManager is required to run the game properly - please open the GameManager wizard and set one.");
				}
			}
			
			EditorGUILayout.EndVertical ();
	
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}
	}
		
}