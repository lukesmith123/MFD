/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"PlayerInput.cs"
 * 
 *	This script records all input and processes it for other scripts.
 * 
 */

using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	
	[HideInInspector] public int buttonPressed;
	[HideInInspector] public bool mouseOverMenu;
	
	[HideInInspector] public Vector2 moveKeys = new Vector2 (0f, 0f);
	[HideInInspector] public bool isRunning = false;
	[HideInInspector] public float timeScale = 1f;
	
	[HideInInspector] public bool isUpLocked = false;
	[HideInInspector] public bool isDownLocked = false;
	[HideInInspector] public bool isLeftLocked = false;
	[HideInInspector] public bool isRightLocked = false;
	[HideInInspector] public PlayerMoveLock runLock = PlayerMoveLock.Free;
	
	[HideInInspector] public int selected_option;
	[HideInInspector] public Vector2 invertedMouse;
	
	private float clickDelay = 0.3f;
	private float clickTime = 0f;
	private float doubleClickDelay = 1f;
	private float doubleClickTime = 0;
	
	// Controller movement
	private Vector2 xboxCursor;
	private float cursorMoveSpeed = 4f;
  	[HideInInspector] public Vector2 mousePosition;
	private bool scrollingLocked = false;
	
	// Touch-Screen movement
	[HideInInspector] public Vector2 dragStartPosition = Vector2.zero;
	[HideInInspector] public float dragSpeed = 0f;
	[HideInInspector] public Vector2 dragVector;
	
	// 1st person movement
	[HideInInspector] public Vector2 freeAim;
	private bool canMoveMouse = false;

	[HideInInspector] public Conversation activeConversation = null;
	[HideInInspector] public ArrowPrompt activeArrows = null;
	
	private StateHandler stateHandler;
	private RuntimeInventory runtimeInventory;
	private SettingsManager settingsManager;

	
	void Awake ()
	{
		if (AdvGame.GetReferences () && AdvGame.GetReferences ().settingsManager)
		{
			settingsManager = AdvGame.GetReferences ().settingsManager;
		}
		
		ResetClick ();
		
		xboxCursor.x = Screen.width / 2;
		xboxCursor.y = Screen.height / 2;
	}
	
	
	void Start ()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine))
		{
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
			{
				stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
			}
			
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>())
			{
				runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();
			}
		}
	}
	
	
	void Update ()
	{
		// This will run during paused, so this function is used to close menus
		
		if (clickTime > 0f)
		{
			clickTime -= 0.1f;
		}
		if (clickTime < 0f)
		{
			clickTime = 0f;
		}
		
		if (doubleClickTime > 0f)
		{
			doubleClickTime -= 0.1f;
		}
		if (doubleClickTime < 0f)
		{
			doubleClickTime = 0f;
		}

		buttonPressed = 0;

		if (stateHandler && settingsManager && Time.time > 0f)
		{
			if (settingsManager.inputType == InputType.MouseAndKeyboard)
			{
				if (Input.GetMouseButtonDown (0))
				{
					buttonPressed = 1;
				}
				else if (Input.GetMouseButtonDown (1))
				{
					buttonPressed = 2;
				}
			}
			else if (settingsManager.inputType == InputType.TouchScreen)
			{
				if (Input.GetMouseButtonDown (0))
				{
					buttonPressed = 1;
				}
				
				else if (Input.GetMouseButton (0) && Input.GetMouseButtonDown (1) || (Input.GetMouseButton (0) && Input.touchCount > 1))
				{
					buttonPressed = 2;
					clickTime = 0f;
				}
				else if (Input.GetMouseButton (0))
				{
					buttonPressed = 3;
				}
			}
			else if (settingsManager.inputType == InputType.Controller)
			{
				if (Input.GetButtonDown ("Controller_A"))
				{
					buttonPressed = 1;
				}
				else if (Input.GetButtonDown ("Controller_B"))
				{
					buttonPressed = 2;
				}
				
				// Menu option changing
				if (stateHandler.gameState == GameState.DialogOptions || stateHandler.gameState == GameState.Paused)
				{
					if (!scrollingLocked)
					{
						if (Input.GetAxis ("Vertical") > 0.1 || Input.GetAxis ("Horizontal") < -0.1)
						{
							// Up / Left
							scrollingLocked = true;
							selected_option --;
						}
						else if (Input.GetAxis ("Vertical") < -0.1 || Input.GetAxis ("Horizontal") > 0.1)
						{
							// Down / Right
							scrollingLocked = true;
							selected_option ++;
						}
					}
					else if (Input.GetAxis ("Vertical") < 0.05 && Input.GetAxis ("Vertical") > -0.05 && Input.GetAxis ("Horizontal") < 0.05 && Input.GetAxis ("Horizontal") > -0.05)
					{
						scrollingLocked = false;
					}
				}
			}		
		
			// Handle cursor position
			if (settingsManager.controlStyle == ControlStyle.FirstPerson)
			{
				if (settingsManager.inputType == InputType.Controller || settingsManager.inputType == InputType.MouseAndKeyboard)
				{
					try
					{
						if (Input.GetButtonDown ("ToggleCursor") && stateHandler.gameState == GameState.Normal)
						{
							if (canMoveMouse)
							{
								canMoveMouse = false;
							}
							else
							{
								canMoveMouse = true;
							}
						}
					}
					catch
					{
						canMoveMouse = false;
					}
				}
				
				if (settingsManager.inputType == InputType.Controller)
				{
					if (!canMoveMouse)
					{
						mousePosition = new Vector2 (Screen.width / 2, Screen.height / 2);
						freeAim = new Vector2 (Input.GetAxis ("ControllerCursorHorizontal") * 50f, Input.GetAxis ("ControllerCursorVertical") * 50f);
					}
					else
					{
						xboxCursor.x += Input.GetAxis ("ControllerCursorHorizontal") * cursorMoveSpeed * Screen.width;
						xboxCursor.y += Input.GetAxis ("ControllerCursorVertical") * cursorMoveSpeed * Screen.width;
						
						xboxCursor.x = Mathf.Clamp (xboxCursor.x, 0f, Screen.width);
						xboxCursor.y = Mathf.Clamp (xboxCursor.y, 0f, Screen.width);
						
						mousePosition = xboxCursor;
							
						freeAim = Vector2.zero;
					}
				}
				
				else if (settingsManager.inputType == InputType.MouseAndKeyboard)
				{
					if (stateHandler.gameState == GameState.Normal && !canMoveMouse)
					{
						mousePosition = new Vector2 (Screen.width / 2, Screen.height / 2);
						freeAim = new Vector2 (Input.GetAxis ("MouseHorizontal"), Input.GetAxis ("MouseVertical"));
					}
					else
					{
						mousePosition = Input.mousePosition;
						freeAim = Vector2.zero;
					}
				}
				
				else if (settingsManager.inputType == InputType.TouchScreen)
				{
					mousePosition = Input.mousePosition;
					
					if (dragStartPosition != Vector2.zero)
					{
						freeAim = new Vector2 (dragVector.x * settingsManager.freeAimTouchSpeed, 0f);
					}
					else
					{
						freeAim = Vector2.zero;
					}
				}
			}
			else
			{
				if (settingsManager.inputType == InputType.MouseAndKeyboard || settingsManager.inputType == InputType.TouchScreen)
				{
					mousePosition = Input.mousePosition;
				}
	
				else if (settingsManager.inputType == InputType.Controller && stateHandler.gameState == GameState.Normal)
				{
					xboxCursor.x += Input.GetAxis ("ControllerCursorHorizontal") * cursorMoveSpeed * Screen.width;
					xboxCursor.y += Input.GetAxis ("ControllerCursorVertical") * cursorMoveSpeed * Screen.width;
					
					xboxCursor.x = Mathf.Clamp (xboxCursor.x, 0f, Screen.width);
					xboxCursor.y = Mathf.Clamp (xboxCursor.y, 0f, Screen.width);
					
					mousePosition = xboxCursor;
				}
			}
			
			invertedMouse = new Vector2 (mousePosition.x, Screen.height - mousePosition.y);
		}		
	}
	

	void FixedUpdate ()
	{
		if (stateHandler && stateHandler.gameState == GameState.Normal)
		{
			if (buttonPressed == 2)
			{
				runtimeInventory.SetNull ();
			}
			
			if (settingsManager && settingsManager.inputType != InputType.TouchScreen && activeArrows && (activeArrows.arrowPromptType == ArrowPromptType.KeyOnly || activeArrows.arrowPromptType == ArrowPromptType.KeyAndClick))
			{
				// Arrow Prompt is displayed: respond to movement keys
				if (Input.GetAxis("Vertical") > 0.1)
				{
					activeArrows.DoUp ();
				}
				
				else if (Input.GetAxis ("Vertical") < -0.1)
				{
					activeArrows.DoDown ();
				}
				
				else if (Input.GetAxis ("Horizontal") < -0.1)
				{
					activeArrows.DoLeft ();
				}
				
				else if (Input.GetAxis ("Horizontal") > 0.1)
				{
					activeArrows.DoRight ();
				}
			}
			
			if (activeArrows && (activeArrows.arrowPromptType == ArrowPromptType.ClickOnly || activeArrows.arrowPromptType == ArrowPromptType.KeyAndClick))
			{
				// Arrow Prompt is displayed: respond to mouse clicks
				if (buttonPressed == 1)
				{
					if (activeArrows.upArrow.rect.Contains (invertedMouse))
					{
						activeArrows.DoUp ();
					}
					
					else if (activeArrows.downArrow.rect.Contains (invertedMouse))
					{
						activeArrows.DoDown ();
					}
					
					else if (activeArrows.leftArrow.rect.Contains (invertedMouse))
					{
						activeArrows.DoLeft ();
					}
					
					else if (activeArrows.rightArrow.rect.Contains (invertedMouse))
					{
						activeArrows.DoRight ();
					}
				}
			}
			
			else if (settingsManager.controlStyle != ControlStyle.PointAndClick)
			{
				float h = 0f;
				float v = 0f;
				bool run;
				
				if (settingsManager.inputType != InputType.TouchScreen)
				{
					h = Input.GetAxis ("Horizontal");
					v = Input.GetAxis ("Vertical");
				}
				else
				{
					h = dragVector.x;
					v = -dragVector.y;
				}
				
				if ((isUpLocked && v > 0f) || (isDownLocked && v < 0f))
				{
					v = 0f;
				}
				
				if ((isLeftLocked && h > 0f) || (isRightLocked && h < 0f))
				{
					h = 0f;
				}

				if (runLock == PlayerMoveLock.Free)
				{
					if (settingsManager.inputType == InputType.TouchScreen)
					{
						if (dragStartPosition != Vector2.zero && dragSpeed > settingsManager.dragRunThreshold * 10f)
						{
							run = true;
						}
						else
						{
							run = false;
						}
					}
					else
					{
						try
						{
							run = Input.GetButton ("Run");
						}
						catch
						{
							run = false;
							Debug.LogWarning ("No 'Run' button exists - please define one in the Input Manager.");
						}
					}
				}
				else if (runLock == PlayerMoveLock.AlwaysWalk)
				{
					run = false;
				}
				else
				{
					run = true;
				}
				
				isRunning = run;
				moveKeys = new Vector2 (h, v);
			}
		}
	}
	
	
	public void RemoveActiveArrows ()
	{
		if (activeArrows)
		{
			activeArrows.TurnOff ();
		}
	}
	
	
	public void ResetClick ()
	{
		clickTime = clickDelay;
	}
	
	
	public void ResetDoubleClick ()
	{
		doubleClickTime = doubleClickDelay;
	}
	
	
	public bool CanClick ()
	{
		if (clickTime == 0f)
		{
			return true;
		}
		
		return false;
	}
	
	
	public bool CanDoubleClick ()
	{
		if (doubleClickTime > 0f)
		{
			return true;
		}
		
		return false;
	}
	
	
	private void OnDestroy ()
	{
		stateHandler = null;
		runtimeInventory = null;
		settingsManager = null;
	}
	
}
