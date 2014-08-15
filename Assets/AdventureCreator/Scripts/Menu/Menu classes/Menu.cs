/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Menu.cs"
 * 
 *	This script is a container of MenuElement subclasses, which together make up a menu.
 *	When menu elements are added, this script updates the size, positioning etc automatically.
 *	The handling of menu visibility, element clicking, etc is all handled in MenuSystem,
 *	rather than the Menu class itself.
 * 
 */

using UnityEngine;
using System.Collections.Generic;

public class Menu
{
	
	public List<MenuElement> visibleElements = new List<MenuElement>();
	public float alpha = 0f;
	public AppearType appearType;
	
	public MenuElement selected_element;
	public int selected_slot = 0;
	
	private List<MenuElement> elements;
	
	private Vector2 biggestElementSize;
	private Vector2 defaultElementSize;
	
	private Texture2D backgroundTexture;
	
	private float spacing;
	private bool isEnabled;
	private bool autoResize;
	
	private Orientation orientation;
	private Rect rect = new Rect ();
	
	private bool isFading = false;
	private FadeType fadeType = FadeType.fadeIn;
	
	private GameState gameStateAfterFade;
	private bool changeGameStateAfterFade = false;
	private GameState previousState;
	
	
	public Menu (float _spacing, Orientation _orientation, AppearType _appearType, Vector2 _defaultElementSize)
	{
		spacing = _spacing;
		orientation = _orientation;
		appearType = _appearType;
		defaultElementSize = _defaultElementSize;
		
		elements = new List<MenuElement>();
		isEnabled = false;
		autoResize = true;
	}
	
	
	public void SetBackground (Texture2D _backgroundTexture)
	{
		backgroundTexture = _backgroundTexture;
	}
	
	
	public void StartDisplay ()
	{
		GUI.BeginGroup (rect);

		if (backgroundTexture)
		{
			Rect texRect = new Rect (0f, 0f, rect.width, rect.height);
			GUI.DrawTexture (texRect, backgroundTexture, ScaleMode.StretchToFill, true, 0f);
		}
	}
	
	
	public void EndDisplay ()
	{
		GUI.EndGroup ();
	}
	
	
	public void SetPosition (Vector2 _position)
	{
		rect.x = _position.x * Screen.width;
		rect.y = _position.y * Screen.height;
		
		FitMenuInsideScreen ();
	}
	
	
	public void SetCentre (Vector2 _position)
	{
		Vector2 centre = new Vector2 (_position.x * Screen.width, _position.y * Screen.height);
		
		rect.x = centre.x - (rect.width / 2);
		rect.y = centre.y - (rect.height / 2);
		
		FitMenuInsideScreen ();
	}
	
	
	private void FitMenuInsideScreen ()
	{
		if (rect.x < 0f)
		{
			rect.x = 0f;
		}
		
		if (rect.y < 0f)
		{
			rect.y = 0f;
		}
		
		if ((rect.x + rect.width) > Screen.width)
		{
			rect.x = Screen.width - rect.width;
		}
		
		if ((rect.y + rect.height) > Screen.height)
		{
			rect.y = Screen.height - rect.height;
		}
	}
	
	
	public void Align (TextAnchor _anchor)
	{
		// X
		if (_anchor == TextAnchor.LowerLeft || _anchor == TextAnchor.MiddleLeft || _anchor == TextAnchor.UpperLeft)
		{
			rect.x = 0;
		}
		else if (_anchor == TextAnchor.LowerCenter || _anchor == TextAnchor.MiddleCenter || _anchor == TextAnchor.UpperCenter)
		{
			rect.x = (Screen.width - rect.width) / 2;
		}
		else
		{
			rect.x = Screen.width - rect.width;
		}
		
		// Y
		if (_anchor == TextAnchor.LowerLeft || _anchor == TextAnchor.LowerCenter || _anchor == TextAnchor.LowerRight)
		{
			rect.y = Screen.height - rect.height;
		}
		else if (_anchor == TextAnchor.MiddleLeft || _anchor == TextAnchor.MiddleCenter || _anchor == TextAnchor.MiddleRight)
		{
			rect.y = (Screen.width - rect.height) / 2;
		}
		else
		{
			rect.y = 0;
		}
	}
	
	
	public void SetSize (Vector2 _size)
	{
		rect.width = _size.x * Screen.width;
		rect.height = _size.y * Screen.height;

		autoResize = false;
	}
	
	
	public Rect GetRect ()
	{
		return rect;
	}
	
	
	public bool IsPointerOverSlot (MenuElement _element, int slot, Vector2 _pointer) 
	{
		Rect RectRelative = _element.GetSlotRectRelative (slot);
		Rect RectAbsolute = GetRectAbsolute (RectRelative);

		return (RectAbsolute.Contains (_pointer));
	}

	
	private Rect GetRectAbsolute (Rect _rectRelative)
	{
		Rect RectAbsolute = new Rect (_rectRelative.x + rect.x, _rectRelative.y + rect.y, _rectRelative.width, _rectRelative.height);
		
		return (RectAbsolute);
	}
	
	
	public void Add (MenuElement _element)
	{
		elements.Add (_element);
		_element.SetSize (defaultElementSize);
		_element.orientation = orientation;
		
		AutoResize ();
	}
	
	
	public void Add (MenuElement _element, Vector2 _size)
	{
		elements.Add (_element);
		_element.SetSize (_size);
		_element.orientation = orientation;
		
		AutoResize ();
	}
	
	
	public void AutoResize ()
	{
		autoResize = true;
		
		visibleElements.Clear ();
		biggestElementSize = new Vector2 ();
		
		foreach (MenuElement element in elements)
		{
			element.RecalculateSize ();
			
			if (element.isVisible)
			{
				visibleElements.Add (element);
				
				if (element.GetSize().x > biggestElementSize.x)
				{
					biggestElementSize.x = element.GetSize().x;
				}
				
				if (element.GetSize().y > biggestElementSize.y)
				{
					biggestElementSize.y = element.GetSize().y;
				}
			}
		}
		
		float totalLength = 0f;
		
		foreach (MenuElement element in visibleElements)
		{
			if (orientation == Orientation.Horizontal)
			{
				totalLength += element.GetSize().x + (spacing * Screen.width);
			}
			else
			{
				totalLength += element.GetSize().y + (spacing * Screen.width);
			}
		}
		
		if (orientation == Orientation.Horizontal)
		{
			rect.width = (spacing * Screen.width) + totalLength;
			rect.height = (2 * spacing * Screen.width) + biggestElementSize.y;
		}
		else
		{
			rect.width = (2 * spacing * Screen.width) + biggestElementSize.x;
			rect.height = (spacing *Screen.width) + totalLength;
		}
	}

	
	private void PositionElements ()
	{
		float totalLength = 0f;
		
		foreach (MenuElement element in visibleElements)
		{
			element.RecalculateSize ();
			
			if (orientation == Orientation.Horizontal)
			{
				element.SetPosition (new Vector2 ((spacing * Screen.width) + totalLength, (spacing * Screen.width)));
				totalLength += element.GetSize().x + (spacing * Screen.width);
			}
			else
			{
				element.SetPosition (new Vector2 ((spacing * Screen.width), (spacing * Screen.width) + totalLength));
				totalLength += element.GetSize().y + (spacing * Screen.width);
			}
		}
	}
	
	
	public void Centre ()
	{
		SetCentre (new Vector2 (0.5f, 0.5f));
	}
	
	
	public bool IsEnabled ()
	{
		return (isEnabled);
	}
	
	
	public bool IsVisible ()
	{
		if (alpha == 1f && isEnabled)
		{
			return true;
		}
		
		return false;
	}
	
	
	public void HandleFade ()
	{
		if (isFading)
		{
			if (fadeType == FadeType.fadeIn)
			{
				alpha += 0.1f;

				if (alpha > 0.95f)
				{
					alpha = 1f;
					isEnabled = true;
					isFading = false;
					
					if (changeGameStateAfterFade)
					{
						changeGameStateAfterFade = false;
						ChangeGameState (gameStateAfterFade);
					}
				}
			}

			else
			{
				alpha -= 0.1f;
				
				if (alpha < 0.05f)
				{
					alpha = 0f;
					isFading = false;
					isEnabled = false;
					
					if (changeGameStateAfterFade)
					{
						changeGameStateAfterFade = false;
						ChangeGameState (gameStateAfterFade);
					}
				}
			}
		}
	}

		
	public void TurnOn (bool doFade)
	{
		// Setting selected_slot to -2 will cause PlayerInput's selected_option to reset
		if (!isEnabled)
		{
			selected_slot = -2;
			
			if (!isEnabled && !isFading)
			{
				if (autoResize)
				{
					AutoResize ();
				}
				
				PositionElements ();
				isEnabled = true;
		
				isFading = doFade;
				
				if (doFade)
				{
					fadeType = FadeType.fadeIn;
				}
				else
				{
					alpha = 1f;
					isEnabled = true;
					isFading = false;
				}
			}
		}
	}

	
	public void TurnOn (bool doFade, GameState stateChange)
	{
		if (!isEnabled)
		{
			TurnOn (doFade);

			if (doFade)
			{
				changeGameStateAfterFade = true;
				gameStateAfterFade = stateChange;
			}
			else
			{
				changeGameStateAfterFade = false;
				ChangeGameState (stateChange);
			}
		}
	}
	
	
	public void TurnOff (bool doFade)
	{
		if (isEnabled && !isFading)
		{
			isFading = doFade;
			
			if (doFade)
			{
				fadeType = FadeType.fadeOut;
			}
			else
			{
				alpha = 0f;
				isFading = false;
				isEnabled = false;	
			}
		}
	}
	
	
	public void ForceOff ()
	{
		if (isEnabled || isFading)
		{
			alpha = 0f;
			isFading = false;
			isEnabled = false;
		}
	}
	
	
	public void TurnOffAndReturnState (bool doFade)
	{
		TurnOff (doFade, previousState);
	}
	
	
	public void TurnOff (bool doFade, GameState stateChange)
	{
		if (isEnabled && !isFading)
		{
			TurnOff (doFade);
			
			if (doFade)
			{
				changeGameStateAfterFade = true;
				gameStateAfterFade = stateChange;
			}
			else
			{
				changeGameStateAfterFade = false;
				ChangeGameState (stateChange);
			}
		}
	}
	
	
	private void ChangeGameState (GameState stateChange)
	{
		StateHandler stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
		
		if (stateHandler)
		{
			stateHandler.gameState = stateChange;
		}
	}
	

	public void CrossFade ()
	{
		PlayerMenus playerMenus = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerMenus>();
		playerMenus.CrossFade (this);
	}
	
	
	public void CrossFade (GameState stateChange)
	{
		ChangeGameState (stateChange);
		CrossFade ();
	}
	
	
	public int ControlSelected (int selected_option)
	{

		if (selected_slot == -2)
		{
			selected_option = 0;
		}

		if (selected_option < 0)
		{
			selected_option = 0;
			selected_element = visibleElements[0];
			selected_slot = 0;
		}
		else
		{
			int sel = 0;
			selected_slot = -1;
			int element = 0;
			int slot = 0;
			
			for (element=0; element<visibleElements.Count; element++)
			{
				if (visibleElements[element].isClickable)
				{
					for (slot=0; slot<visibleElements[element].GetNumSlots (); slot++)
					{
						if (selected_option == sel)
						{
							selected_slot = slot;
							selected_element = visibleElements[element];
							break;
						}
						sel++;
					}
				}
				
				if (selected_slot != -1)
				{
					break;
				}
			}
			
			if (selected_slot == -1)
			{
				// Couldn't find match, must've maxed out
				selected_slot = slot-1;
				selected_element = visibleElements[element-1];
				selected_option = sel - 1;
			}
		}
		
		return selected_option;
	}
	
	
	public void SetPreviousState (GameState state)
	{
		previousState = state;
	}
	
}