/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"PlayerMovement.cs"
 * 
 *	This script analyses the variables in PlayerInput, and moves the character
 *	based on the control style, defined in the SettingsManager.
 *	To move the Player during cutscenes, a PlayerPath object must be defined.
 *	This Path will dynamically change based on where the Player must travel to.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
	
	private FirstPersonCamera firstPersonCamera;
	private StateHandler stateHandler;
	private Player player;
	private PlayerInput playerInput;
	private PlayerInteraction playerInteraction;
	private MainCamera mainCamera;
	private SettingsManager settingsManager;
	private SceneSettings sceneSettings;
	
	
	private void Awake ()
	{
		if (AdvGame.GetReferences () && AdvGame.GetReferences ().settingsManager)
		{
			settingsManager = AdvGame.GetReferences ().settingsManager;
		}
		
		playerInput = this.GetComponent <PlayerInput>();
		playerInteraction = this.GetComponent <PlayerInteraction>();
		sceneSettings = this.GetComponent <SceneSettings>();
		
		if (GameObject.FindWithTag (Tags.mainCamera) && GameObject.FindWithTag (Tags.mainCamera).GetComponent <MainCamera>())
		{
			mainCamera = GameObject.FindWithTag (Tags.mainCamera).GetComponent <MainCamera>();
		}
	}
	
	
	private void Start ()
	{
		if (GameObject.FindWithTag (Tags.player) && GameObject.FindWithTag (Tags.player).GetComponent <Player>())
		{
			player = GameObject.FindWithTag (Tags.player).GetComponent <Player>();
		}
		
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
		{
			stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
		}
		
		if (GameObject.FindWithTag (Tags.firstPersonCamera) && GameObject.FindWithTag (Tags.firstPersonCamera).GetComponent <FirstPersonCamera>())
		{
			firstPersonCamera = GameObject.FindWithTag (Tags.firstPersonCamera).GetComponent <FirstPersonCamera>();
		}
	}
	
	
	private void LateUpdate ()
	{
		if (stateHandler && settingsManager && stateHandler.gameState == GameState.Normal && playerInput.activeArrows == null)
		{
			if (settingsManager.controlStyle == ControlStyle.Direct)
			{
				if (settingsManager.inputType == InputType.TouchScreen)
				{
					DragPlayer (true);
				}
				
				else
				{
					if (player.GetPath () == null || !player.lockedPath)
					{
						// Normal gameplay
						DirectControlPlayer (true);
					}
					else
					{
						// Move along pre-determined path
						DirectControlPlayerPath ();
					}
				}
			}
			
			else if (settingsManager.controlStyle == ControlStyle.PointAndClick)
			{
				PointControlPlayer ();
			}
			
			else if (settingsManager.controlStyle == ControlStyle.FirstPerson)
			{
				if (settingsManager.inputType == InputType.TouchScreen)
				{
					FirstPersonControlPlayer ();
					DragPlayer (false);
				}
				else
				{
					FirstPersonControlPlayer ();
					DirectControlPlayer (false);
				}
			}
		}
	}
	
	
	// Drag functions
	
	private void DragPlayer (bool doRotation)
	{
		if (player && playerInput && settingsManager && playerInput.CanClick ())
		{
			if (playerInput.buttonPressed == 0)
			{
				playerInput.dragStartPosition = Vector2.zero;
				playerInput.dragSpeed = 0f;
				
				if (player.charState == CharState.Move)
				{
					player.charState = CharState.Decelerate;
				}
			}
			
			else if (!playerInput.mouseOverMenu && (playerInput.buttonPressed == 2 || !playerInteraction.IsMouseOverHotspot ()))
			{
				if (playerInput.buttonPressed == 1)
				{
					if (playerInteraction.hotspot)
					{
						playerInteraction.hotspot.Deselect ();
						playerInteraction.hotspot = null;
					}
					
					playerInput.dragStartPosition = playerInput.invertedMouse;
					
					playerInput.ResetClick ();
					playerInput.ResetDoubleClick ();
				}
				else
				{
					playerInput.dragVector = playerInput.invertedMouse - playerInput.dragStartPosition;
					playerInput.dragSpeed = playerInput.dragVector.magnitude;
					playerInput.dragVector.Normalize ();
					
					Vector3 moveDirectionInput = (playerInput.moveKeys.y * mainCamera.ForwardVector ()) + (playerInput.moveKeys.x * mainCamera.RightVector ());
					
					if (playerInput.dragSpeed > settingsManager.dragWalkThreshold * 10f)
					{
						player.isRunning = playerInput.isRunning;
					
						player.charState = CharState.Move;
					
						if (doRotation)
						{
							player.SetLookDirection (moveDirectionInput, false);
							player.SetMoveDirectionAsForward ();
						}
						else
						{
							if (playerInput.dragVector.y < 0f)
							{
								player.SetMoveDirectionAsForward ();
							}
							else
							{
								player.SetMoveDirectionAsBackward ();
							}
						}
					}
					else
					{
						if (player.charState == CharState.Move)
						{
							player.charState = CharState.Decelerate;
						}
					}
				}
			}
		}
	}
	
	
	// Direct-control functions
	
	private void DirectControlPlayer (bool doRotation)
	{
		if (player && playerInput)
		{
			if (playerInput.moveKeys != Vector2.zero)
			{
	
				Vector3 moveDirectionInput = (playerInput.moveKeys.y * mainCamera.ForwardVector ()) + (playerInput.moveKeys.x * mainCamera.RightVector ());
		
				player.isRunning = playerInput.isRunning;
				player.charState = CharState.Move;
				
				if (doRotation)
				{
					player.SetLookDirection (moveDirectionInput, false);
					player.SetMoveDirectionAsForward ();
				}
				else
				{
					player.SetMoveDirection (moveDirectionInput);
				}
			}
			else
			{
				if (player.charState == CharState.Move)
				{
					player.charState = CharState.Decelerate;
				}
			}
		}
	}
	
	
	private void DirectControlPlayerPath ()
	{
		if (player && playerInput)
		{
			if (playerInput.moveKeys != Vector2.zero)
			{
				Vector3 moveDirectionInput = (playerInput.moveKeys.y * mainCamera.ForwardVector ()) + (playerInput.moveKeys.x * mainCamera.RightVector ());
				
				if (Vector3.Dot (moveDirectionInput, player.GetMoveDirection ()) > 0f)
				{
					// Move along path, because movement keys are in the path's forward direction
					player.isRunning = playerInput.isRunning;
					player.charState = CharState.Move;
				}
			}
			else
			{
				if (player.charState == CharState.Move)
				{
					player.charState = CharState.Decelerate;
				}
			}
		}
	}
	
	
	// Point/click functions
	
	private void PointControlPlayer ()
	{
		if (player && playerInput && playerInteraction)
		{
			if (playerInput.buttonPressed == 1 && !playerInput.mouseOverMenu && !playerInteraction.IsMouseOverHotspot () && playerInput.CanClick ())
			{
				if (playerInteraction.hotspot)
				{
					playerInteraction.hotspot.Deselect ();
					playerInteraction.hotspot = null;
				}
				
				bool doubleClick = false;
				
				if (playerInput.CanDoubleClick ())
				{
					doubleClick = true;
				}

				playerInput.ResetClick ();
				playerInput.ResetDoubleClick ();
	
				if (sceneSettings.navMesh)
				{
					if (!RaycastNavMesh (playerInput.mousePosition, doubleClick))
					{
						// Move Ray down screen until we hit something
						Vector3 simulatedMouse = playerInput.mousePosition;
		
						for (int i=1; i<Screen.height/2; i+=4)
						{
							if (RaycastNavMesh (new Vector2 (simulatedMouse.x, simulatedMouse.y - i), doubleClick))
							{
								break;
							}
							if (RaycastNavMesh (new Vector2 (simulatedMouse.x, simulatedMouse.y + i), doubleClick))
							{
								break;
							}
						}
					}
				}
	
			}
			else if (player.GetPath () == null && player.charState == CharState.Move)
			{
				player.charState = CharState.Decelerate;
			}
		}
	}
	
		
	private bool RaycastNavMesh (Vector3 mousePosition, bool run)
	{
		Ray ray = Camera.main.ScreenPointToRay (mousePosition);
		
		RaycastHit hit = new RaycastHit();
		
		if (settingsManager && Physics.Raycast (ray, out hit, settingsManager.navMeshRaycastLength, 1 << sceneSettings.navMesh.gameObject.layer))
		{
			if (hit.collider.gameObject == sceneSettings.navMesh.gameObject)
			{
				
				if (playerInput.runLock == PlayerMoveLock.AlwaysRun)
				{
					run = true;
				}
				
				if (sceneSettings && sceneSettings.navMesh && sceneSettings.navMesh)
				{
					Vector3[] pointArray = sceneSettings.navMesh.GetPointsArray (player.transform.position, hit.point);
					player.MoveAlongPoints (pointArray, run);
				}
				else
				{
					player.MoveToPoint (hit.point, run);
				}
				
				return true;
			}
		}
		
		return false;
	}
	
	
	// First-person function
	
	private void FirstPersonControlPlayer ()
	{
		if (firstPersonCamera)
		{
			if (player)
			{
				float rotationX = player.transform.localEulerAngles.y + playerInput.freeAim.x * firstPersonCamera.sensitivity.x;
				firstPersonCamera.rotationY += playerInput.freeAim.y * firstPersonCamera.sensitivity.y;
				player.transform.localEulerAngles = new Vector3 (0, rotationX, 0);
			}
		}
		else
		{
			Debug.LogWarning ("Could not find first person camera");
		}
	}
	
	
	private void OnDestroy ()
	{
		firstPersonCamera = null;
		stateHandler = null;
		player = null;
		playerInput = null;
		mainCamera = null;
		settingsManager = null;
		sceneSettings = null;
	}
	
}
