/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"PlayerMenus.cs"
 * 
 *	This script handles the displaying of each of the menus defined in MenuSystem.
 *	It avoids referencing specific menus and menu elements as much as possible,
 *	so that the menu can be completely altered using just the MenuSystem script.
 * 
 */

using UnityEngine;
using System.Collections;

public class PlayerMenus : MonoBehaviour
{

	[HideInInspector] public bool lockSave = false;
	
	private string hotspotLabel = "";
	
	private Menu crossFadeTo;
	private Menu crossFadeFrom;
	
	private Dialog dialog;
	private PlayerInput playerInput;
	private PlayerInteraction playerInteraction;
	private MenuSystem menuSystem;
	private StateHandler stateHandler;
	private Options options;
	private SettingsManager settingsManager;

	
	private void Awake ()
	{
		if (AdvGame.GetReferences () && AdvGame.GetReferences ().settingsManager)
		{
			settingsManager = AdvGame.GetReferences ().settingsManager;
		}
		
		playerInput = this.GetComponent <PlayerInput>();
		playerInteraction = this.GetComponent <PlayerInteraction>();
		menuSystem = this.GetComponent<MenuSystem>();
		dialog = this.GetComponent<Dialog>();
	}

	
	private void Start ()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine))
		{
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
			{
				stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
			}
			
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
			{
				options = stateHandler.GetComponent <Options>();
			}
		}
	}
	
	
	private void OnGUI ()
	{
		
		if (playerInteraction && playerInput && menuSystem && stateHandler && settingsManager)
		{
			hotspotLabel = playerInteraction.GetLabel ();
			
			foreach (Menu menu in menuSystem.menus)
			{
				if (menu.IsEnabled ())
				{
					Color tempColor = GUI.color;
					tempColor.a = menu.alpha;
					GUI.color = tempColor;
					
					menu.StartDisplay ();
	
					foreach (MenuElement element in menu.visibleElements)
					{
						for (int i=0; i<element.GetNumSlots (); i++)
						{
							if (menu.IsVisible () && element.isClickable &&
								((settingsManager.inputType == InputType.MouseAndKeyboard && menu.IsPointerOverSlot (element, i, playerInput.invertedMouse)) ||
								(settingsManager.inputType == InputType.TouchScreen && menu.IsPointerOverSlot (element, i, playerInput.invertedMouse)) ||
								(settingsManager.inputType == InputType.Controller && stateHandler.gameState == GameState.Normal && menu.IsPointerOverSlot (element, i, playerInput.invertedMouse)) ||
								((settingsManager.inputType == InputType.Controller && stateHandler.gameState != GameState.Normal && menu.selected_element == element && menu.selected_slot == i))))
							{
								element.Display (menuSystem.highlightedStyle, i);
								
								if (element is MenuInventoryBox)
								{
									MenuInventoryBox inventoryBox = (MenuInventoryBox) element;
									hotspotLabel = inventoryBox.GetLabel (i);
								}
								
								if (playerInput.buttonPressed > 0)
								{
									CheckClick (menu, element, i, playerInput.buttonPressed);
								}
							}
							
							else
							{
								element.Display (menuSystem.normalStyle, i);
							}
						}
					}

					menu.EndDisplay ();
				}
			}
		}
	}

	
	private void Update ()
	{
		if (stateHandler && settingsManager && playerInput && playerInteraction && options && dialog && menuSystem && Time.time > 0f)
		{
			if (stateHandler.gameState == GameState.Paused)
			{
				Time.timeScale = 0f;
			}
			else
			{
				Time.timeScale = playerInput.timeScale;
			}
			
			playerInput.mouseOverMenu = false;
		
			foreach (Menu menu in menuSystem.menus)
			{
				menu.HandleFade ();
				
				if (settingsManager)
				{
					if (settingsManager.inputType == InputType.Controller && menu.IsEnabled () && (stateHandler.gameState == GameState.Paused || stateHandler.gameState == GameState.DialogOptions))
					{
						playerInput.selected_option = menu.ControlSelected (playerInput.selected_option);
					}
				}
				else
				{
					Debug.LogWarning ("A settings manager is not present.");
				}
				
				if (menu.appearType == AppearType.Gameplay)
				{
					if (stateHandler.gameState == GameState.Normal)
					{
						menu.TurnOn (true);
						
						if (menu.GetRect ().Contains (playerInput.invertedMouse))
						{
							playerInput.mouseOverMenu = true;
							
							if (playerInteraction.hotspot)
							{
								playerInteraction.hotspot.DeselectInstant ();
								playerInteraction.hotspot = null;
							}
						}
					}
					else
					{
						menu.TurnOff (true);
					}
				}
				
				else if (menu.appearType == AppearType.MouseOverInventory)
				{
					RuntimeInventory runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();

					if (runtimeInventory && !runtimeInventory.isLocked)
					{
						if (settingsManager.alwaysOnInventory)
						{
							if (stateHandler.gameState == GameState.Normal)
							{
								menu.TurnOn (true);
								
								if (menu.GetRect ().Contains (playerInput.invertedMouse))
								{
									playerInput.mouseOverMenu = true;
									
									if (playerInteraction.hotspot)
									{
										playerInteraction.hotspot.DeselectInstant ();
										playerInteraction.hotspot = null;
									}
								}
							}
							else
							{
								menu.TurnOff (true);
							}
						}

						else
						{
							if (menu.GetRect ().Contains (playerInput.invertedMouse) && stateHandler.gameState == GameState.Normal)
							{
								menu.TurnOn (true);
								playerInput.mouseOverMenu = true;
								
								if (playerInteraction.hotspot)
								{
									playerInteraction.hotspot.DeselectInstant ();
									playerInteraction.hotspot = null;
								}
							}
							else
							{
								menu.TurnOff (true);
							}
						}
					}
				}
				
				else if (menu.appearType == AppearType.DialogOptions && stateHandler.gameState != GameState.Paused)
				{
					if (playerInput.activeConversation != null)
					{
						menu.TurnOn (true, GameState.DialogOptions);
					}
					else
					{
						menu.TurnOff (true);
					}
				}
				
				else if (menu.appearType == AppearType.OnMenuButton)
				{
					try
					{
						if (Input.GetButtonDown ("Menu"))
						{
							if (!menu.IsEnabled ())
							{
								if (playerInteraction.hotspot)
								{
									hotspotLabel = "";
									playerInteraction.hotspot.DeselectInstant ();
									playerInteraction.hotspot = null;
								}
								
								if (stateHandler.gameState != GameState.Paused)
								{
									menu.SetPreviousState (stateHandler.gameState);
								}
								
								menu.CrossFade (GameState.Paused);
							}
							else
							{
								menu.TurnOffAndReturnState (true);
							}
						}
					}
					catch
					{
						if (settingsManager.inputType != InputType.TouchScreen)
						{
							Debug.LogWarning ("No 'Menu' button exists - please define one in the Input Manager.");
						}
					}
				}
				
				else if (menu.appearType == AppearType.OnHotspot)
				{
					menu.SetCentre (new Vector2 (playerInput.invertedMouse.x / Screen.width, playerInput.invertedMouse.y / Screen.height - 0.05f));
					if (hotspotLabel != "" && stateHandler.gameState == GameState.Normal)
					{
						menuSystem.hotspot_Label.label = hotspotLabel;
						menu.TurnOn (true);
					}
					else
					{
						menu.TurnOff (true);
					}
				}
				
				else if (menu.appearType == AppearType.OnSpeech)
				{
					if (stateHandler.gameState != GameState.Paused)
					{
						if (dialog.GetLine () != "" && stateHandler.gameState != GameState.DialogOptions)
						{
							if 	(options.optionsData.showSubtitles || (settingsManager.forceSubtitles && !dialog.foundAudio)) 
							{
								menuSystem.subs_Speaker.label = dialog.GetSpeaker ();
								menuSystem.subs_Line.label = dialog.GetLine ();
								
								// Auto-resize subtitles label
								Vector2 size = menuSystem.subs_Line.GetSize ();
								GUIContent content = new GUIContent (dialog.GetFullLine ());
								size.y = menuSystem.normalStyle.CalcHeight (content, size.x);
								menuSystem.subs_Line.SetAbsoluteSize (size);
								menu.AutoResize ();
		
								menu.TurnOn (true);
							}
							else
							{
								menu.TurnOff (true);	
							}
						}
						else
						{
							menu.TurnOff (true);
						}
					}
				}
			}			
		}
		
		if (crossFadeFrom != null && crossFadeTo != null && !crossFadeFrom.IsEnabled ())
		{
			crossFadeTo.TurnOn (true);
			crossFadeTo = null;
		}
		
		if (menuSystem && stateHandler)
		{
			if ((lockSave || stateHandler.gameState == GameState.Cutscene || stateHandler.gameState == GameState.DialogOptions) && menuSystem.pause_SaveButton.isVisible)
			{
				menuSystem.pause_SaveButton.isVisible = false;
			}
			else if (!lockSave && stateHandler.gameState == GameState.Normal && !menuSystem.pause_SaveButton.isVisible)
			{
				menuSystem.pause_SaveButton.isVisible = true;
			}
		}
	}
	
	
	public void CheckClick (Menu _menu, MenuElement _element, int _slot, int _buttonPressed)
	{
		if (playerInput && playerInput.CanClick ())
		{
			playerInput.ResetClick ();
			menuSystem.ProcessClick (_menu, _element, _slot, _buttonPressed);
		}
	}
	
	
	public void CrossFade (Menu _menuTo)
	{
		if (!_menuTo.IsEnabled())
		{
			// Turn off all other menus
			crossFadeFrom = null;
			
			foreach (Menu menu in menuSystem.menus)
			{
				if (menu.IsVisible ())
				{
					menu.TurnOff (true);
					crossFadeFrom = menu;
				}
				else
				{
					menu.ForceOff ();
				}
			}
			
			if (crossFadeFrom != null)
			{
				crossFadeTo = _menuTo;
			}
			else
			{
				_menuTo.TurnOn (true);
			}
		}
	}

	
	private void OnDestroy ()
	{
		dialog = null;
		playerInput = null;
		playerInteraction = null;
		menuSystem = null;
		stateHandler = null;
		options = null;
		settingsManager = null;
	}
	
}
