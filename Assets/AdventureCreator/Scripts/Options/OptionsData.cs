/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"OptionsData.cs"
 * 
 *	This script contains any variables we want to appear in our Options menu.
 * 
 */

[System.Serializable]
public class OptionsData
{
	
	public int language;
	public bool showSubtitles;
	
	public int sfxVolume;
	public int musicVolume;
	public int speechVolume;
	
	
	public OptionsData ()
	{
		language = 0;
		showSubtitles = false;
		
		sfxVolume = 9;
		musicVolume = 6;
		speechVolume = 10;
	}
	
}