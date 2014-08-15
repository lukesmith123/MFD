/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MenuSystem.cs"
 *	This script defines all menus used in the game.
 *	The ProcessClick function is called when the Player clicks
 *	on an element, allowing menu variables to largely remain private.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuSystem : MonoBehaviour
{

	public List<Menu> menus = new List<Menu>();

	private Menu pauseMenu = new Menu (0.005f, Orientation.Vertical, AppearType.OnMenuButton, new Vector2 (0.10f, 0.04f));
	private Menu optionsMenu = new Menu (0.005f, Orientation.Vertical, AppearType.Manual, new Vector2 (0.30f, 0.04f));
	private Menu saveMenu = new Menu (0.005f, Orientation.Vertical, AppearType.Manual, new Vector2 (0.30f, 0.04f));
	private Menu loadMenu = new Menu (0.005f, Orientation.Vertical, AppearType.Manual, new Vector2 (0.30f, 0.04f));
	private Menu inventoryMenu = new Menu (0.005f, Orientation.Horizontal, AppearType.MouseOverInventory, new Vector2 (0.06f, 0.10f));
	private Menu inGameMenu = new Menu (0.005f, Orientation.Horizontal, AppearType.Gameplay, new Vector2 (0.06f, 0.04f));
	private Menu conversationMenu = new Menu (0.005f, Orientation.Vertical, AppearType.DialogOptions, new Vector2 (0.2f, 0.04f));
	private Menu hotspotMenu = new Menu (0.005f, Orientation.Horizontal, AppearType.OnHotspot, new Vector2 (0.1f, 0.04f));
	private Menu subsMenu = new Menu (0.005f, Orientation.Vertical, AppearType.OnSpeech, new Vector2 (0.3f, 0.04f));

	// Pause menu elements
	private MenuButton pause_ResumeButton = new MenuButton ("Resume");
	private MenuButton pause_OptionsButton = new MenuButton ("Options");
	public MenuButton pause_SaveButton = new MenuButton ("Save");
	private MenuButton pause_LoadButton = new MenuButton ("Load");
	private MenuButton pause_QuitButton = new MenuButton ("Quit");
	
	// Options menu elements
	private MenuLabel options_Title = new MenuLabel ("Options", false);
	private MenuSlider options_Speech = new MenuSlider ("Speech volume");
	private MenuSlider options_Music = new MenuSlider ("Music volume");
	private MenuSlider options_Sfx = new MenuSlider ("SFX volume");
	private MenuCycle options_Language;
	private MenuToggle options_Subs = new MenuToggle ("Subtitles");
	private MenuButton options_BackButton = new MenuButton ("Back");
	
	// Save menu elements
	private MenuLabel save_Title = new MenuLabel ("Save game", false);
	private MenuSavesList save_SavesList = new MenuSavesList ();
	private MenuButton save_NewButton = new MenuButton ("New save");
	private MenuButton save_BackButton = new MenuButton ("Back");
	
	// Load menu elements
	private MenuLabel load_Title = new MenuLabel ("Load game", false);
	private MenuSavesList load_SavesList = new MenuSavesList ();
	private MenuButton load_BackButton = new MenuButton ("Back");
	
	// Inventory menu elements
	private MenuInventoryBox inventory_Box = new MenuInventoryBox ();
	
	// InGame menu elements
	private MenuButton inGame_MenuButton = new MenuButton ("Menu");
	
	// Dialog menu elements
	private MenuDialogList dialog_Box = new MenuDialogList ();
	private MenuTimer dialog_Timer = new MenuTimer ();
	
	// Hotspot menu elements
	public MenuLabel hotspot_Label = new MenuLabel ("", true);
	
	// Subtitles menu elements
	public MenuLabel subs_Speaker = new MenuLabel ("", true);
	public MenuLabel subs_Line = new MenuLabel ("", false);

	public Texture2D highlightTexture;
	public Texture2D backgroundTexture;
	public Texture2D sliderTexture;
	
	public Font font;
	public float fontScaleFactor = 60f;
	public Color fontColor = Color.white;
	public Color fontHighlightColor = Color.white;
	private int fontSize;

	[HideInInspector] public GUIStyle normalStyle;
	[HideInInspector] public GUIStyle highlightedStyle;

	private Options options;
	private SaveSystem saveSystem;
	private SpeechManager speechManager;
	
	
	private void Start ()
	{
		if (AdvGame.GetReferences () && AdvGame.GetReferences ().speechManager)
		{
			speechManager = AdvGame.GetReferences ().speechManager;
		}
		
		SetFontSize ();
		
		menus.Add (pauseMenu);
		menus.Add (optionsMenu);
		menus.Add (saveMenu);
		menus.Add (loadMenu);
		menus.Add (inventoryMenu);
		menus.Add (inGameMenu);
		menus.Add (conversationMenu);
		menus.Add (hotspotMenu);
		menus.Add (subsMenu);
		
		pauseMenu.Add (pause_ResumeButton);
		pauseMenu.Add (pause_OptionsButton);

		if (Application.platform != RuntimePlatform.OSXWebPlayer && Application.platform != RuntimePlatform.WindowsWebPlayer)
		{
			pauseMenu.Add (pause_SaveButton);
			pauseMenu.Add (pause_LoadButton);
		}
		pauseMenu.Add (pause_QuitButton);
		pauseMenu.SetBackground (backgroundTexture);
		pauseMenu.Centre ();
		
		optionsMenu.Add (options_Title);
		options_Speech.SetSliderTexture (sliderTexture);
		options_Music.SetSliderTexture (sliderTexture);
		options_Sfx.SetSliderTexture (sliderTexture);
		optionsMenu.Add (options_Speech);
		optionsMenu.Add (options_Music);
		optionsMenu.Add (options_Sfx);
	
		if (speechManager)
		{
			options_Language = new MenuCycle ("Language", speechManager.languages.ToArray());
			optionsMenu.Add (options_Language);
			if (speechManager.languages.Count == 1)
			{
				options_Language.isVisible = false;
			}
		}
		
		optionsMenu.Add (options_Subs);
		optionsMenu.Add (options_BackButton);
		optionsMenu.SetBackground (backgroundTexture);
		optionsMenu.Centre ();
		
		saveMenu.Add (save_Title);
		saveMenu.Add (save_SavesList);
		saveMenu.Add (save_NewButton);
		saveMenu.Add (save_BackButton);
		saveMenu.SetBackground (backgroundTexture);
		saveMenu.Centre ();
		
		loadMenu.Add (load_Title);
		loadMenu.Add (load_SavesList);
		loadMenu.Add (load_BackButton);
		loadMenu.SetBackground (backgroundTexture);
		loadMenu.Centre ();
		
		inventoryMenu.Add (inventory_Box);
		inventoryMenu.SetSize (new Vector2 (1f, 0.12f));
		inventoryMenu.Align (TextAnchor.UpperCenter);
		inventoryMenu.SetBackground (backgroundTexture);
		
		inGameMenu.Add (inGame_MenuButton);
		inGameMenu.Align (TextAnchor.LowerLeft);
		inGameMenu.SetBackground (backgroundTexture);
		
		conversationMenu.Add (dialog_Box);
		conversationMenu.Add (dialog_Timer);
		conversationMenu.SetCentre (new Vector2 (0.25f, 0.72f));
		dialog_Timer.SetSize (new Vector2 (0.2f, 0.01f));
		dialog_Timer.SetTimerTexture (sliderTexture);
		conversationMenu.SetBackground (backgroundTexture);
		
		subsMenu.Add (subs_Speaker);
		subsMenu.Add (subs_Line);
		subsMenu.SetBackground (backgroundTexture);
		subs_Line.SetAlignment (TextAnchor.UpperLeft);
		subsMenu.SetCentre (new Vector2 (0.5f, 0.85f));
		
		hotspotMenu.Add (hotspot_Label);
		
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			pause_QuitButton.isVisible = false;
			pauseMenu.AutoResize ();
		}
		
		SetStyles ();
	
		if (GameObject.FindWithTag (Tags.persistentEngine))
		{
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <Options>())
			{
				options = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <Options>();
			}
		
			if (GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>())
			{
				saveSystem = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <SaveSystem>();
			}
		}
	}
	
	
	private void SetFontSize ()
	{
		fontSize = (int) (Screen.width / fontScaleFactor);
	}


	public void ProcessClick (Menu _menu, MenuElement _element, int _slot, int _buttonPressed)
	{
		// Pause menu

		if (_menu == pauseMenu)
		{
			if (_buttonPressed == 2)
			{	
				pauseMenu.TurnOffAndReturnState (true);
			}
			else if (_element == pause_ResumeButton)
			{
				pauseMenu.TurnOffAndReturnState (true);
			}
			else if (options && _element == pause_OptionsButton)
			{
				options_Speech.amount = options.optionsData.speechVolume;
				options_Sfx.amount = options.optionsData.sfxVolume;
				options_Music.amount = options.optionsData.musicVolume;
				
				options_Subs.isOn = options.optionsData.showSubtitles;
				options_Language.selected = options.optionsData.language;
				
				optionsMenu.CrossFade ();
			}
			else if (_element == pause_SaveButton)
			{
				if (SaveSystem.GetNumSlots () < 6)
				{
					save_NewButton.isVisible = true;
				}
				else
				{
					save_NewButton.isVisible = false;
				}
				
				saveMenu.CrossFade ();
			}
			else if (_element == pause_LoadButton)
			{
				loadMenu.CrossFade ();
			}
			else if (_element == pause_QuitButton)
			{
				Application.Quit();
			}
		}
		
		// Options menu
		
		else if (_menu == optionsMenu)
		{
			if (_buttonPressed == 2)
			{	
				pauseMenu.CrossFade ();
			}
			else if (options)
			{
				if (_element == options_Speech || _element == options_Sfx || _element == options_Music)
				{
					MenuSlider slider = (MenuSlider) _element;
					slider.Change ();
					
					if (_element == options_Speech)
					{
						options.optionsData.speechVolume = options_Speech.amount;
					}
					else if (_element == options_Sfx)
					{
						options.optionsData.sfxVolume = options_Sfx.amount;
						options.SetVolume (SoundType.SFX);
					}
					else if (_element == options_Music)
					{
						options.optionsData.musicVolume = options_Music.amount;
						options.SetVolume (SoundType.Music);
					}
					
					options.SavePrefs ();
				}
				else if (_element == options_Language)
				{
					options_Language.Cycle ();
					options.optionsData.language = options_Language.selected;
					options.SavePrefs ();
				}
				else if (_element == options_Subs)
				{
					options_Subs.Toggle ();
					options.optionsData.showSubtitles = options_Subs.isOn;
					options.SavePrefs ();
				}
				else if (_element == options_BackButton)
				{
					pauseMenu.CrossFade ();
				}
			}
		}
			
		// Save menu
		
		else if (_menu == saveMenu)
		{
			if (_buttonPressed == 2)
			{	
				pauseMenu.CrossFade ();
			}
			else if (saveSystem)
			{
				if (_element == save_SavesList)
				{
					saveMenu.TurnOff (true, GameState.Normal);
					saveSystem.SaveGame (_slot);
				}
				else if (_element == save_NewButton)
				{
					saveMenu.TurnOff (true, GameState.Normal);
					saveSystem.SaveNewGame ();
				}
				else if (_element == save_BackButton)
				{
					pauseMenu.CrossFade ();
				}
			}
		}
		
		// Load menu
		
		else if (_menu == loadMenu)
		{
			if (_buttonPressed == 2)
			{	
				pauseMenu.CrossFade ();
			}
			else if (saveSystem)
			{
				if (_element == load_SavesList)
				{
					loadMenu.TurnOff (false);
					saveSystem.LoadGame (_slot);
				}
				else if (_element == load_BackButton)
				{
					pauseMenu.CrossFade ();
				}
			}
		}
		
		// Inventory menu
		
		else if (_menu == inventoryMenu)
		{
			RuntimeInventory runtimeInventory = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <RuntimeInventory>();
			
			if (runtimeInventory && _element == inventory_Box)
			{
				
				if (_buttonPressed == 1)
				{
					if (runtimeInventory.selectedID == -1)
					{
						runtimeInventory.Use (_slot);
					}
					
					else
					{
						runtimeInventory.Combine (_slot);
					}
				}
				
				else if (_buttonPressed == 2)
				{
					runtimeInventory.Look (_slot);
				}

			}
		}
		
		// InGame menu
		
		else if (_menu == inGameMenu)
		{
			if (_element == inGame_MenuButton)
			{
				pauseMenu.SetPreviousState (GameState.Normal);
				pauseMenu.CrossFade (GameState.Paused);					
			}
		}
		
		// Dialog menu
		
		else if (_menu == conversationMenu)
		{
			if (_element == dialog_Box)
			{
				dialog_Box.RunOption (_slot);
			}
		}
			
	}

	
	private void SetStyles ()
	{
		normalStyle = new GUIStyle();
		normalStyle.normal.textColor = fontColor;
		normalStyle.font = font;
		normalStyle.fontSize = fontSize;
		normalStyle.alignment = TextAnchor.MiddleCenter;
		
		highlightedStyle = new GUIStyle();
		highlightedStyle.font = font;
		highlightedStyle.fontSize = fontSize;
		highlightedStyle.normal.textColor = fontHighlightColor;
		highlightedStyle.alignment = TextAnchor.MiddleCenter;
		
		highlightedStyle.normal.background = highlightTexture;
	}

	
	private void OnEnable ()
	{
		options = null;
		saveSystem = null;
		speechManager = null;
	}
	
}