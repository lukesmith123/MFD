/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Options.cs"
 * 
 *	This script provides a runtime instance of OptionsData,
 *	and has functions for saving and loading this data
 *	into the PlayerPrefs
 * 
 */

using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour
{
	
	public OptionsData optionsData;
	
	private string ppKey = "Options";
	
	
	private void Awake ()
	{
		optionsData = new OptionsData();
		LoadPrefs ();
	}
	
	
	public void SavePrefs ()
	{
		string optionsBinary = Serializer.SerializeObjectBinary (optionsData);
		PlayerPrefs.SetString(ppKey, optionsBinary);
		Debug.Log ("PlayerPrefs saved.");
	}
	
	
	private void LoadPrefs ()
	{
		if (PlayerPrefs.HasKey (ppKey))
		{
			string optionsBinary = PlayerPrefs.GetString (ppKey);
			optionsData = Serializer.DeserializeObjectBinary <OptionsData> (optionsBinary);
			Debug.Log ("PlayerPrefs loaded.");
		}
	}
	
	
	private void OnLevelWasLoaded ()
	{
		SetVolume (SoundType.Music);
		SetVolume (SoundType.SFX);	
	}
	
	
	public void SetVolume (SoundType _soundType)
	{
		Sound[] soundObs = FindObjectsOfType (typeof (Sound)) as Sound[];
		
		foreach (Sound soundOb in soundObs)
		{
			if (soundOb.soundType == _soundType)
			{
				soundOb.SetMaxVolume ();
			}
		}
	}

}