/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"PlayerInteraction.cs"
 * 
 *	This script processes cursor clicks over hotspots and NPCs
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{

	[HideInInspector] public Hotspot hotspot;
	
	private PlayerInput playerInput;
	private SceneSettings sceneSettings;
	private Player player;
	private StateHandler stateHandler;
	private RuntimeInventory runtimeInventory;
	private SettingsManager settingsManager;

	
	void Awake ()
	{
		playerInput = this.GetComponent <PlayerInput>();
		sceneSettings = this.GetComponent <SceneSettings>();
		
		if (AdvGame.GetReferences () == null)
		{
			Debug.LogError ("A References file is required - please use the Adventure Creator window to create one.");
		}
		else
		{
			settingsManager = AdvGame.GetReferences ().settingsManager;
		}
	}
	
	
	void Start ()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine))
		{
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
			{
				stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
			}
			
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
			{
				runtimeInventory = stateHandler.GetComponent <RuntimeInventory>();
			}
		}
		
		if (GameObject.FindWithTag (Tags.player) && GameObject.FindWithTag (Tags.player).GetComponent <Player>())
		{
			player = GameObject.FindWithTag (Tags.player).GetComponent <Player>();
		}
	}
	
	
	void OnGUI ()
	{
		if (stateHandler && playerInput && settingsManager && runtimeInventory && stateHandler.gameState == GameState.Normal)			
		{
				
			if (!playerInput.mouseOverMenu && Camera.main)
			{
				
				if (settingsManager.inputType == InputType.TouchScreen)
				{
					if (playerInput.CanClick () && playerInput.buttonPressed == 1)
					{
						playerInput.ResetClick ();
						
						// Check Hotspots only when click/tap
						Hotspot newHotspot = null;
						Ray ray = Camera.main.ScreenPointToRay (playerInput.mousePosition);
						RaycastHit hit;
						
						if (Physics.Raycast (ray, out hit, settingsManager.hotspotRaycastLength, 1 << LayerMask.NameToLayer ("Default")))
						{
							newHotspot = hit.collider.gameObject.GetComponent <Hotspot>();
						}
						
						if (hotspot && !newHotspot)
						{
							hotspot.Deselect ();
							hotspot = null;
						}
						else if (newHotspot)
						{
							if (settingsManager.controlStyle != ControlStyle.PointAndClick && playerInput.dragStartPosition != Vector2.zero)
							{
								// Disable hotspots while dragging player
								if (hotspot)
								{
									hotspot.Deselect ();
									hotspot = null;
								}
							}
							else
							{
								if (newHotspot != hotspot)
								{
									if (hotspot)
									{
										hotspot.Deselect ();
									}
									
									hotspot = newHotspot;
									hotspot.Select ();
								}
								else if (hotspot)
								{
									playerInput.ResetClick ();
									HandleInteraction ();
								}
							}
						}
					}
					else if (playerInput.buttonPressed == 2 && playerInput.CanClick ())
					{
						playerInput.ResetClick ();
						HandleInteraction ();
					}
					else if (playerInput.buttonPressed > 0 && !IsMouseOverHotspot () && runtimeInventory.selectedID > -1)
					{
						runtimeInventory.SetNull ();
					}
				}
				
				else
				{
					// Mouse-over objects
					
					Hotspot newHotspot = null;
					Ray ray = Camera.main.ScreenPointToRay (playerInput.mousePosition);
					RaycastHit hit;
					
					if (Physics.Raycast (ray, out hit, settingsManager.hotspotRaycastLength, 1 << LayerMask.NameToLayer ("Default")))
					{
						newHotspot = hit.collider.gameObject.GetComponent <Hotspot>();
					}
					
					if (hotspot && !newHotspot)
					{
						hotspot.Deselect ();
						hotspot = null;
					}
					else if (newHotspot && hotspot != newHotspot)
					{
						if (hotspot)
						{
							hotspot.Deselect ();
						}
						
						hotspot = newHotspot;
						hotspot.Select ();
					}
					
					if (playerInput.buttonPressed == 1 && hotspot == null && runtimeInventory.selectedID > -1)
					{
						runtimeInventory.SetNull ();
					}
					
					if (playerInput.CanClick () && playerInput.buttonPressed > 0 && hotspot)
					{
						playerInput.ResetClick ();
						HandleInteraction ();
					}
				}

			}
			else if (hotspot)
			{
				hotspot.Deselect ();
				hotspot = null;
			}
		

			
		}
	}
	
	
	private void HandleInteraction ()
	{
		if (hotspot)
		{
			if (playerInput.buttonPressed == 1)
			{
	
				if (runtimeInventory.selectedID == -1 && hotspot.provideUseInteraction)
				{
					// Perform "Use" interaction
					StartCoroutine ("UseObject", InteractionType.Use);
				}
				
				else if (runtimeInventory.selectedID > -1)
				{
					// Perform "Use Inventory" interaction
					StartCoroutine ("UseObject", InteractionType.Inventory);
				}
			}
			else if (playerInput.buttonPressed == 2)
			{
				if (hotspot.provideLookInteraction)
				{
					// Perform "Look" interaction
					StartCoroutine ("UseObject", InteractionType.Examine);
				}
			}
		}
	}

	private IEnumerator UseObject (InteractionType _interactionType)
	{
		Hotspot _hotspot = hotspot;
		hotspot.Deselect ();
		hotspot = null;
		
		player.EndPath ();
		
		Button _button = null;
		bool isUnhandled = false;
		
		if (_interactionType == InteractionType.Use)
		{
			_button = _hotspot.useButton;
		}
		else if (_interactionType == InteractionType.Examine)
		{
			_button = _hotspot.lookButton;
		}
		else if (_interactionType == InteractionType.Inventory)
		{
			foreach (Button invButton in _hotspot.invButtons)
			{
				if (invButton.invID == runtimeInventory.selectedID)
				{
					_button = invButton;
					break;
				}
			}
			
			if (_button == null)
			{
				isUnhandled = true;
			}
		}
			

		if (_button != null && _button.playerAction != PlayerAction.DoNothing)
		{
			stateHandler.gameState = GameState.Cutscene;

			Vector3 lookVector = _hotspot.transform.position - player.transform.position;
			lookVector.y = 0;
			
			player.SetLookDirection (lookVector, false);
			
			if (_button.playerAction == PlayerAction.TurnToFace)
			{
				while (player.IsTurning ())
				{
					yield return new WaitForFixedUpdate ();			
				}
			}
			
			Vector3 targetPos = _hotspot.transform.position;
			
			
			if (_button.playerAction == PlayerAction.WalkToMarker && _hotspot.walkToMarker)
			{
				if (sceneSettings && sceneSettings.navMesh && sceneSettings.navMesh.GetComponent <Collider>())
				{
					Vector3[] pointArray = sceneSettings.navMesh.GetPointsArray (player.transform.position, _hotspot.walkToMarker.transform.position);
					player.MoveAlongPoints (pointArray, false);
					targetPos = pointArray [pointArray.Length - 1];
				}
				else
				{
					player.MoveToPoint (_hotspot.walkToMarker.transform.position, false);
				}
				
				while (player.activePath)
				{
					yield return new WaitForFixedUpdate ();
				}
			}
			
			else if (lookVector.magnitude > 2f && _button.playerAction == PlayerAction.WalkTo)
			{
				if (sceneSettings && sceneSettings.navMesh && sceneSettings.navMesh.GetComponent <Collider>())
				{
					Vector3[] pointArray = sceneSettings.navMesh.GetPointsArray (player.transform.position, _hotspot.transform.position);
					player.MoveAlongPoints (pointArray, false);
					targetPos = pointArray [pointArray.Length - 1];
				}
				else
				{
					player.MoveToPoint (_hotspot.transform.position, false);
				}
				
				if (_button.setProximity)
				{
					_button.proximity = Mathf.Max (_button.proximity, 1f);
					targetPos.y = player.transform.position.y;
					
					while (Vector3.Distance (player.transform.position, targetPos) > _button.proximity && player.activePath)
					{
						yield return new WaitForFixedUpdate ();
					}
				}
				else
				{
					yield return new WaitForSeconds (0.6f);
				}
			}
		}
		else
		{
			player.charState = CharState.Decelerate;
		}
		
		player.EndPath ();
		yield return new WaitForSeconds (0.1f);
		
		runtimeInventory.SetNull ();
		
		if (isUnhandled)
		{
			// Unhandled event
			if (runtimeInventory.unhandledHotspot)
			{
				RuntimeActionList runtimeActionList = this.GetComponent <RuntimeActionList>();
				
				runtimeInventory.SetNull ();
				runtimeActionList.Play (runtimeInventory.unhandledHotspot);	
			}
		}
		else
		{
			if (_button.interaction)
			{
				_button.interaction.Interact ();
			}
			else
			{
				Debug.Log ("No interaction object found for " + _hotspot.name);
				stateHandler.gameState = GameState.Normal;
			}
		}

	}
	
	
	public string GetLabel ()
	{
		string label = "";
		
		if (settingsManager.inventoryHandling != InventoryHandling.ChangeCursor && runtimeInventory.selectedID > -1)
		{
			label = "Use " + runtimeInventory.GetLabel (runtimeInventory.selectedID) + " on ";
		}
		
		if (hotspot)
		{
			if (hotspot.hotspotName != "")
			{
				label += hotspot.hotspotName;
			}
			else
			{
				label += hotspot.name;
			}
		}
		
		return (label);		
	}
	
	
	public void StopInteraction ()
	{
		StopCoroutine ("UseObject");
	}
	
	
	public bool IsMouseOverHotspot ()
	{
		Ray ray = Camera.main.ScreenPointToRay (playerInput.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit, settingsManager.hotspotRaycastLength, 1 << LayerMask.NameToLayer ("Default")))
		{
			return true;
		}
		
		return false;
	}
	
	
	private void OnDestroy ()
	{
		playerInput = null;
		sceneSettings = null;
		stateHandler = null;
		runtimeInventory = null;
		player = null;
	}
}

