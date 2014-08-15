/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"PlayerCursor.cs"
 * 
 *	This script displays a cursor graphic on the screen.
 *	PlayerInput decides if this should be at the mouse position,
 *	or a position based on controller input.
 *	The cursor graphic changes based on what hotspot is underneath it.
 * 
 */

using UnityEngine;
using System.Collections;

public class PlayerCursor : MonoBehaviour
{
	
	private bool showCursor = false;
	
	private SettingsManager settingsManager;
	private StateHandler stateHandler;
	private RuntimeInventory runtimeInventory;
	private PlayerInput playerInput;
	private PlayerInteraction playerInteraction;
	
	
	private void Awake ()
	{
		playerInput = this.GetComponent <PlayerInput>();
		playerInteraction = this.GetComponent <PlayerInteraction>();
		
		if (AdvGame.GetReferences () == null)
		{
			Debug.LogError ("A References file is required - please use the Adventure Creator window to create one.");
		}
		else
		{
			settingsManager = AdvGame.GetReferences ().settingsManager;
		}
	}
	
	
	private void Start ()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
		{
			stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
		}
		
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>())
		{
			runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();
		}
	}
	
	
	private void Update ()
	{
		
		if (settingsManager && (!settingsManager.allowMainCursor || settingsManager.pointerTexture == null) && runtimeInventory.selectedID == -1 && settingsManager.inputType == InputType.MouseAndKeyboard && stateHandler.gameState != GameState.Cutscene)
		{
			Screen.showCursor = true;
		}
		else
		{
			Screen.showCursor = false;
		}
		
		if (settingsManager && stateHandler)
		{
			if (stateHandler.gameState == GameState.Cutscene)
			{
				showCursor = false;
			}
			else if (stateHandler.gameState != GameState.Normal && settingsManager.inputType == InputType.Controller)
			{
				showCursor = false;
			}
			else
			{
				showCursor = true;
			}
		}
		
	}
	
	
	private void OnGUI ()
	{
		if (playerInput && playerInteraction && stateHandler && settingsManager && runtimeInventory && showCursor)
		{
			GUI.depth = -1;
			
			if (runtimeInventory.selectedID > -1 && settingsManager.inventoryHandling != InventoryHandling.ChangeHotspotLabel && stateHandler.gameState != GameState.Paused)
			{
				// Cursor becomes selected inventory
				if (runtimeInventory.GetTexture (runtimeInventory.selectedID))
				{
					GUI.DrawTexture (AdvGame.GUIBox (playerInput.mousePosition, settingsManager.inventoryCursorSize), runtimeInventory.GetTexture (runtimeInventory.selectedID), ScaleMode.ScaleToFit, true, 0f);
				}
				else
				{
					Debug.LogWarning ("No texture defined for " + runtimeInventory.GetLabel (runtimeInventory.selectedID) + " - please set in InventoryManager");
				}
			}
			else
			{
				if (playerInteraction.hotspot && stateHandler.gameState == GameState.Normal && playerInteraction.hotspot.provideUseInteraction && settingsManager.allowInteractionCursor)
				{
					if (playerInteraction.hotspot.useIcon == InteractionIcon.Talk)
					{
						if (settingsManager.talkTexture)
						{
							GUI.DrawTexture (AdvGame.GUIBox (playerInput.mousePosition, settingsManager.iconCursorSize), settingsManager.talkTexture, ScaleMode.ScaleToFit, true, 0f);
						}
						else
						{
							Debug.LogWarning ("No 'talk' texture defined - please set in SettingsManager.");
						}
					}
					
					else if (playerInteraction.hotspot.useIcon == InteractionIcon.Examine)
					{
						if (settingsManager.lookTexture)
						{
							GUI.DrawTexture (AdvGame.GUIBox (playerInput.mousePosition, settingsManager.iconCursorSize), settingsManager.lookTexture, ScaleMode.ScaleToFit, true, 0f);
						}
						else
						{
							Debug.LogWarning ("No 'look' texture defined - please set in SettingsManager.");
						}
					}
					
					else
					{
						if (settingsManager.useTexture)
						{
							GUI.DrawTexture (AdvGame.GUIBox (playerInput.mousePosition, settingsManager.iconCursorSize), settingsManager.useTexture, ScaleMode.ScaleToFit, true, 0f);
						}
						else
						{
							Debug.LogWarning ("No 'use' texture defined - please set in SettingsManager.");
						}
					}
				}
				else if (playerInteraction.hotspot && playerInteraction.hotspot.provideLookInteraction && settingsManager.allowInteractionCursor)
				{
					if (settingsManager.lookTexture)
					{
						GUI.DrawTexture (AdvGame.GUIBox (playerInput.mousePosition, settingsManager.iconCursorSize), settingsManager.lookTexture, ScaleMode.ScaleToFit, true, 0f);
					}
					else
					{
						Debug.LogWarning ("No 'examine' texture defined - please set in SettingsManager.");
					}
				}
				else if (settingsManager.allowMainCursor || settingsManager.inputType == InputType.Controller)
				{
					if (settingsManager.pointerTexture)
					{
						GUI.DrawTexture (AdvGame.GUIBox (playerInput.mousePosition, settingsManager.normalCursorSize), settingsManager.pointerTexture, ScaleMode.ScaleToFit, true, 0f);
					}
					else
					{
						Debug.LogWarning ("No 'main' texture defined - please set in SettingsManager.");
					}
				}
			}
			
			// Drag line
			if (stateHandler.gameState == GameState.Normal && playerInput.activeArrows == null && settingsManager.inputType == InputType.TouchScreen && settingsManager.controlStyle != ControlStyle.PointAndClick && playerInput.dragStartPosition != Vector2.zero)
			{
				Vector2 pointA = playerInput.dragStartPosition;
			    Vector2 pointB = playerInput.invertedMouse;
			    DrawStraightLine.Draw (pointA, pointB, settingsManager.dragLineColor, settingsManager.dragLineWidth);
			}
		}
	}

	
	private void OnDestroy ()
	{
		stateHandler = null;
		runtimeInventory = null;
		playerInput = null;
		playerInteraction = null;
	}

}
