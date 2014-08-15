/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionAnim.cs"
 * 
 *	This action is used for standard animation playback for GameObjects.
 *	It is fairly simplistic, and not meant for characters.
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionAnim : Action
{

	public Animation _anim;
	public AnimationClip clip;
	
	public enum AnimMethod { PlayCustom, StopCustom };
	public AnimMethod method;
	
	public AnimationBlendMode blendMode = AnimationBlendMode.Blend;
	public AnimPlayMode playMode;
	public float fadeTime = 0f;

	
	public ActionAnim ()
	{
		this.isDisplayed = true;
		title = "Object: Animate";
	}
	
	
	override public float Run ()
	{

		if (!isRunning)
		{
			isRunning = true;
			
			if (_anim && clip)
			{	
				if (method == AnimMethod.PlayCustom)
				{
					AdvGame.CleanUnusedClips (_anim);
					
					WrapMode wrap = WrapMode.Once;
					if (playMode == AnimPlayMode.PlayOnceAndClamp)
					{
						wrap = WrapMode.ClampForever;
					}
					else if (playMode == AnimPlayMode.Loop)
					{
						wrap = WrapMode.Loop;
					}
						
					AdvGame.PlayAnimClip (_anim, 0, clip, blendMode, wrap, fadeTime, null);
				}
				
				else if (method == AnimMethod.StopCustom)
				{
					AdvGame.CleanUnusedClips (_anim);
					_anim.animation.Blend (clip.name, 0f, fadeTime);
				}
				
				if (willWait)
				{
					return (defaultPauseTime);
				}
			}

			return 0f;
		}
		else
		{
			if (!_anim.IsPlaying (clip.name))
			{
				isRunning = false;
				return 0f;
			}
			else
			{
				return defaultPauseTime;
			}
		}
		
	}
	
	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		method = (AnimMethod) EditorGUILayout.EnumPopup ("Method:", method);
		
		_anim = (Animation) EditorGUILayout.ObjectField ("Object:", _anim, typeof (Animation), true);
		
		clip = (AnimationClip) EditorGUILayout.ObjectField ("Clip:", clip, typeof (AnimationClip), true);
		
		if (method == AnimMethod.PlayCustom)
		{
			playMode = (AnimPlayMode) EditorGUILayout.EnumPopup ("Play mode:", playMode);
			blendMode = (AnimationBlendMode) EditorGUILayout.EnumPopup ("Blend mode:", blendMode);
		}
		
		fadeTime = EditorGUILayout.Slider ("Transition time:", fadeTime, 0f, 1f);
		
		willWait = EditorGUILayout.Toggle ("Pause until finish?", willWait);

		AfterRunningOption ();
	}
	
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		if (_anim)
		{
			labelAdd = " (" + _anim.name + ")";
		}
		
		return labelAdd;
	}
	
	#endif

}