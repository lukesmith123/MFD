/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"SceneHandler.cs"
 * 
 *	This script stores the gameState variable, which is used by
 *	other scripts to determine if the game is running normal gameplay,
 *	in a cutscene, paused, or displaying conversation options.
 * 
 */

using UnityEngine;

public class StateHandler : MonoBehaviour
{
	
	public GameState gameState = GameState.Normal;
	
	private void Awake ()
	{
		DontDestroyOnLoad(this);
	}

}