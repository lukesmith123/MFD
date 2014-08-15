/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"PlayerStart.cs"
 * 
 *	This script defines a possible starting position for the
 *	player when the scene loads, based on what the previous
 *	scene was.  If no appropriate PlayerStart is found, the
 *	one define in StartSettings is used as the default.
 * 
 */

using UnityEngine;
using System.Collections;

public class PlayerStart : Marker
{
	
	public bool fadeInOnStart;
	public float fadeSpeed = 0.5f;
	public int previousScene;
	public GameCamera cameraOnStart;
	
	private GameObject playerOb;

	
	public void SetPlayerStart ()
	{
		if (GameObject.FindWithTag (Tags.mainCamera) && GameObject.FindWithTag (Tags.mainCamera).GetComponent <MainCamera>())
		{
			MainCamera mainCam = GameObject.FindWithTag (Tags.mainCamera).GetComponent <MainCamera>();
			
			if (mainCam && fadeInOnStart)
			{
				mainCam.FadeIn (fadeSpeed);
			}
			
			playerOb = GameObject.FindWithTag (Tags.player);
	
			if (playerOb)
			{
				playerOb.transform.position = this.transform.position;
				playerOb.transform.rotation = this.transform.rotation;
			}
	
			if (AdvGame.GetReferences () && AdvGame.GetReferences ().settingsManager)
			{
				SettingsManager settingsManager = AdvGame.GetReferences ().settingsManager;
			
				if (settingsManager.controlStyle == ControlStyle.FirstPerson)
				{
					mainCam.SetFirstPerson ();
				}
				
				else if (cameraOnStart && mainCam)
				{
					mainCam.SetGameCamera (cameraOnStart);
					mainCam.lastNavCamera = cameraOnStart;
					cameraOnStart.MoveCameraInstant ();
					mainCam.SnapToAttached ();
				}
				
				else if (cameraOnStart == null)
				{
					Debug.LogWarning (this.name + " has no Camera On Start");
				}
			}
		}
	}
	
}
