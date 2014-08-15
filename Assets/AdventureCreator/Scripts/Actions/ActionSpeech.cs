/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionSpeech.cs"
 * 
 *	This action handles the displaying of messages, and talking of characters.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionSpeech : Action
{
	
	public bool isPlayer;
	public Char speaker;
	public string messageText;
	public int lineID;
	public bool isBackground = false;
	public AnimationClip headClip;
	public AnimationClip mouthClip;
	
	private Dialog dialog;
	private StateHandler stateHandler;
	private SpeechManager speechManager;
	private Options options;
	
	
	public ActionSpeech ()
	{
		this.isDisplayed = true;
		title = "Dialogue: Play speech";
		lineID = -1;
	}
	
	
	override public float Run ()
	{
		dialog = GameObject.FindWithTag(Tags.gameEngine).GetComponent <Dialog>();
		stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
		options = stateHandler.GetComponent <Options>();
		
		if (dialog && stateHandler && options)
		{
			if (!isRunning)
			{
				isRunning = true;
				
				string _text = messageText;
				string _language = "";
				
				if (options.optionsData.language > 0)
				{
					// Not in original language, so pull translation in from Speech Manager
					if (!speechManager)
					{
						speechManager = AdvGame.GetReferences ().speechManager;
					}
					
					if (speechManager.GetLineByID (lineID) != null && speechManager.GetLineByID (lineID).translationText.Count > (options.optionsData.language - 1))
					{
						_text = speechManager.GetLineByID (lineID).translationText [options.optionsData.language - 1];
						_language = speechManager.languages[options.optionsData.language];
					}
				}
				
				if (_text != "")
				{
					dialog.KillDialog ();
					
					if (isBackground)
					{
						stateHandler.gameState = GameState.Normal;
					}
					else
					{
						stateHandler.gameState = GameState.Cutscene;
					}
					
					if (isPlayer)
					{
						speaker = GameObject.FindWithTag(Tags.player).GetComponent <Player>();
					}
					
					if (speaker)
					{
						dialog.StartDialog (speaker, _text, lineID, _language);
						
						if (headClip || mouthClip)
						{
							AdvGame.CleanUnusedClips (speaker.GetComponent <Animation>());	

							if (headClip)
							{
								AdvGame.PlayAnimClip (speaker.GetComponent <Animation>(), (int) AnimLayer.Head, headClip, AnimationBlendMode.Additive, WrapMode.Once, 0f, speaker.neckBone);
							}
							
							if (mouthClip)
							{
								AdvGame.PlayAnimClip (speaker.GetComponent <Animation>(), (int) AnimLayer.Mouth, mouthClip, AnimationBlendMode.Additive, WrapMode.Once, 0f, speaker.neckBone);
							}
						}
					}
					else
					{
						dialog.StartDialog (_text);
					}
					
					if (!isBackground)
					{
						return defaultPauseTime;
					}
				}
	
				return 0f;
			}
			else
			{
				if (!dialog.isMessageAlive)
				{
					isRunning = false;
					stateHandler.gameState = GameState.Cutscene;
					return 0f;
				}
				else
				{
					return defaultPauseTime;
				}
			}
		}
		
		return 0f;
	}
	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		
		if (lineID > -1)
		{
			EditorGUILayout.LabelField ("Speech Manager ID:", lineID.ToString ());
		}
		
		isPlayer = EditorGUILayout.Toggle ("Player line?",isPlayer);
		if (!isPlayer)
		{
			speaker = (Char) EditorGUILayout.ObjectField ("Speaker:", speaker, typeof(Char), true);
		}
		
		messageText = EditorGUILayout.TextField ("Line text:", messageText);
		
		headClip = (AnimationClip) EditorGUILayout.ObjectField ("Head animation:", headClip, typeof (AnimationClip), true);
		mouthClip = (AnimationClip) EditorGUILayout.ObjectField ("Mouth animation:", mouthClip, typeof (AnimationClip), true);

		isBackground = EditorGUILayout.Toggle ("Play in background?", isBackground);

		AfterRunningOption ();
	}
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (messageText != "")
		{
			string shortMessage = messageText;
			if (shortMessage != null && shortMessage.Length > 30)
			{
				shortMessage = shortMessage.Substring (0, 28) + "..";
			}
			
			labelAdd = " (" + shortMessage + ")";
		}
		
		return labelAdd;
	}

	#endif
	
}