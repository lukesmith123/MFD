/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Dialog.cs"
 * 
 *	This script handles the running of dialogue lines, speech or otherwise.
 * 
 */

using UnityEngine;
using System.Collections;
using System.IO;

public class Dialog : MonoBehaviour
{

	public bool isMessageAlive { get; set; }
	public bool foundAudio { get; set; }
	
	private Char speakerChar;
	private string speakerName;
	
	private float alpha;
	private bool isSkippable = false;
	private string displayText = "";
	private string fullText = "";
	private float textWait;
	private float endTime;
	
	private PlayerInput playerInput;
	private SettingsManager settingsManager;

	
	private void Awake ()
	{
		playerInput = this.GetComponent <PlayerInput>();
		
		if (AdvGame.GetReferences () == null)
		{
			Debug.LogError ("A References file is required - please use the Adventure Creator window to create one.");
		}
		else
		{
			settingsManager = AdvGame.GetReferences ().settingsManager;
		}
	}
	
	
	private void FixedUpdate ()
	{
		if (isSkippable && isMessageAlive)
		{
			if (playerInput && playerInput.CanClick () && playerInput.buttonPressed > 0 && settingsManager && settingsManager.allowSpeechSkipping)
			{
				// Stop message due to player click
				playerInput.ResetClick ();
				StartCoroutine ("EndMessage");
			}
			
			else if (Time.time > endTime)
			{
				// Stop message due to timeout
				StartCoroutine ("EndMessage");
			}
		}
	}
	
	
	public string GetSpeaker ()
	{
		if (speakerChar)
		{
			return speakerChar.name;
		}
		
		return "";
	}
	
	
	public string GetLine ()
	{
		if (isMessageAlive && isSkippable)
		{
			return displayText;
		}
		return "";
	}
	
	
	public string GetFullLine ()
	{
		if (isMessageAlive && isSkippable)
		{
			return fullText;
		}
		return "";
	}
	
	
	private IEnumerator EndMessage ()
	{
		StopCoroutine ("StartMessage");
		isSkippable = false;
		
		if (speakerChar)
		{
			// Turn off animations on the character's "mouth" layer
			if (speakerChar.GetComponent <Animation>())
			{
				foreach (AnimationState state in speakerChar.GetComponent <Animation>().animation)
				{
					if (state.layer == (int) AnimLayer.Mouth)
					{
						state.normalizedTime = 1f;
						state.weight = 0f;
					}
				}
			}
			
			if (speakerChar.GetComponent <AudioSource>())
			{
				speakerChar.GetComponent<AudioSource>().Stop();
			}
		}
		
		// Wait a short moment for fade-out
		yield return new WaitForSeconds (0.3f);
		isMessageAlive = false;
	}
	
	
	private IEnumerator StartMessage (string message)
	{
		isMessageAlive = true;
		isSkippable = true;
	
		displayText = "";
		fullText = message;
		
		endTime = textWait + Time.time;
		
		// Start scroll the message
		for (int i = 0; i < message.Length; i++)
		{
			displayText += message[i];
			yield return new WaitForSeconds (1 / settingsManager.textScrollSpeed);
		}
		
		if (endTime == Time.time)
		{
			endTime += 2f;
		}
	}

	
	public void StartDialog (Char _speakerChar, string message, int lineNumber, string language)
	{
		isMessageAlive = false;
		
		if (_speakerChar)
		{
			speakerChar = _speakerChar;
			
			speakerName = _speakerChar.name;
			if (_speakerChar.GetComponent <Player>())
			{
				speakerName = "Player";
			}
		
			if (_speakerChar.GetComponent <Hotspot>())
			{
				if (_speakerChar.GetComponent <Hotspot>().hotspotName != "")
				{
					speakerName = _speakerChar.GetComponent <Hotspot>().hotspotName;
				}
			}
		}
		else
		{
			speakerChar = null;
			speakerName = "";
		}
		
		// Play sound and time textWait to it
		if (lineNumber > -1 && speakerName != "" && settingsManager.searchAudioFiles)
		{
			string filename = "Speech/";
			if (language != "" && settingsManager.translateAudio)
			{
				// Not in original language
				filename += language + "/";
			}
			filename += speakerName + lineNumber;
			
			
			foundAudio = false;
			
			AudioClip clipObj = Resources.Load(filename) as AudioClip;
			if (clipObj)
			{
				if (_speakerChar.GetComponent<AudioSource>())
				{
					if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <Options>())
					{
						Options options = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <Options>();
						_speakerChar.GetComponent <AudioSource>().volume = options.optionsData.speechVolume;
					}
					
					_speakerChar.GetComponent <AudioSource>().clip = clipObj;
					_speakerChar.GetComponent <AudioSource>().Play();
					
					foundAudio = true;
				}
				else
				{
					Debug.LogWarning (_speakerChar.name + " has no audio source component!");
				}
				
				textWait = clipObj.length;
			}
			else
			{
				textWait = settingsManager.screenTimeFactor * (float) message.Length;
				if (textWait < 0.5f)
				{
					textWait = 0.5f;
				}
				
				Debug.Log ("Cannot find audio file: " + filename);
			}
		}
		else
		{
			textWait = settingsManager.screenTimeFactor * (float) message.Length;
			if (textWait < 0.5f)
			{
				textWait = 0.5f;
			}
		}
		
		StopCoroutine ("StartMessage");
		StartCoroutine ("StartMessage", message);
	}
	

	public void StartDialog (string message)
	{
		StartDialog (null, message, -1, "");
	}
	
	
	public void KillDialog ()
	{
		isSkippable = false;
		isMessageAlive = false;
		
		StopCoroutine ("StartMessage");
		StopCoroutine ("EndMessage");

		if (speakerChar && speakerChar.GetComponent <AudioSource>())
		{
			speakerChar.GetComponent <AudioSource>().Stop();
		}
	}

	
	private void OnDestroy ()
	{
		playerInput = null;
		speakerChar = null;
	}

}