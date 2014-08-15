/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"ActionCharAnim.cs"
 * 
 *	This action is used to control character animation.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionCharAnim : Action
{

	public bool isPlayer;
	public Char animChar;
	public AnimationClip clip;

	public enum AnimMethodChar { PlayCustom, StopCustom, ResetToIdle, SetStandard };
	public AnimMethodChar method;
	
	public AnimationBlendMode blendMode;
	public AnimLayer layer = AnimLayer.Base;
	public AnimStandard standard;

	public AnimPlayMode playMode;
	public AnimPlayModeBase playModeBase = AnimPlayModeBase.PlayOnceAndClamp;

	public float fadeTime = 0f;
	
	
	public ActionCharAnim ()
	{
		this.isDisplayed = true;
		title = "Character: Animate";
	}
	
	
	override public float Run ()
	{
		if (isPlayer)
		{
			animChar = GameObject.FindWithTag (Tags.player).GetComponent <Char>();
		}
		
		if (!isRunning)
		{
			isRunning = true;
			
			
			if (animChar)
			{
				if (method == AnimMethodChar.PlayCustom && clip)
				{
					AdvGame.CleanUnusedClips (animChar.animation);
					
					WrapMode wrap = WrapMode.Once;
					Transform mixingTransform = null;
					
					if (layer == AnimLayer.Base)
					{
						animChar.charState = CharState.Custom;
						blendMode = AnimationBlendMode.Blend;
						playMode = (AnimPlayMode) playModeBase;
					}
					else if (layer == AnimLayer.UpperBody)
					{
						mixingTransform = animChar.upperBodyBone;
					}
					else if (layer == AnimLayer.LeftArm)
					{
						mixingTransform = animChar.leftArmBone;
					}
					else if (layer == AnimLayer.RightArm)
					{
						mixingTransform = animChar.rightArmBone;
					}
					else if (layer == AnimLayer.Neck || layer == AnimLayer.Head || layer == AnimLayer.Face || layer == AnimLayer.Mouth)
					{
						mixingTransform = animChar.neckBone;
					}
					
					if (playMode == AnimPlayMode.PlayOnceAndClamp)
					{
						wrap = WrapMode.ClampForever;
					}
					else if (playMode == AnimPlayMode.Loop)
					{
						wrap = WrapMode.Loop;
					}

					AdvGame.PlayAnimClip (animChar.GetComponent <Animation>(), (int) layer, clip, blendMode, wrap, fadeTime, mixingTransform);
				}
				
				else if (method == AnimMethodChar.StopCustom && clip)
				{
					if (clip != animChar.idleAnim && clip != animChar.walkAnim)
					{
						animChar.animation.Blend (clip.name, 0f, fadeTime);
					}
				}
				
				else if (method == AnimMethodChar.ResetToIdle)
				{
					animChar.ResetBaseClips ();
					animChar.charState = CharState.Idle;
					AdvGame.CleanUnusedClips (animChar.animation);
				}
				
				else if (method == AnimMethodChar.SetStandard && clip)
				{
					if (standard == AnimStandard.Idle)
					{
						animChar.idleAnim = clip;
					}
						
					else if (standard == AnimStandard.Walk)
					{
						animChar.walkAnim = clip;
					}
						
					else if (standard == AnimStandard.Run)
					{
						animChar.runAnim = clip;
					}
				}
				
				
				if (willWait && clip)
				{
					if (method == AnimMethodChar.PlayCustom)
					{
						return defaultPauseTime;
					}
					else if (method == AnimMethodChar.StopCustom)
					{
						return fadeTime;
					}
				}
			}	

			return 0f;
		}
		else
		{
			if (animChar.animation[clip.name] && animChar.animation[clip.name].normalizedTime < 1f && animChar.animation.IsPlaying (clip.name))
			{
				return defaultPauseTime;
			}
			else
			{
				isRunning = false;
				
				if (playMode == AnimPlayMode.PlayOnce)
				{
					animChar.animation.Blend (clip.name, 0f, fadeTime);
					
					if (layer == AnimLayer.Base && method == AnimMethodChar.PlayCustom)
					{
						animChar.charState = CharState.Idle;
						animChar.ResetBaseClips ();
					}
				}
				
				AdvGame.CleanUnusedClips (animChar.animation);

				return 0f;
			}
		}
		
	}
	
	
	#if UNITY_EDITOR

	override public void ShowGUI ()
	{
		method = (AnimMethodChar) EditorGUILayout.EnumPopup ("Method:", method);

		isPlayer = EditorGUILayout.Toggle ("Is Player?", isPlayer);
		if (!isPlayer)
		{
			animChar = (Char) EditorGUILayout.ObjectField ("Character:", animChar, typeof (Char), true);
		}
		
		if (method == AnimMethodChar.PlayCustom || method == AnimMethodChar.StopCustom)
		{
			clip = (AnimationClip) EditorGUILayout.ObjectField ("Clip:", clip, typeof (AnimationClip), true);
			
			if (method == AnimMethodChar.PlayCustom)
			{
				layer = (AnimLayer) EditorGUILayout.EnumPopup ("Layer:", layer);
				
				if (layer == AnimLayer.Base)
				{
					EditorGUILayout.LabelField ("Blend mode:", "Blend");
					playModeBase = (AnimPlayModeBase) EditorGUILayout.EnumPopup ("Play mode:", playModeBase);
				}
				else
				{
					blendMode = (AnimationBlendMode) EditorGUILayout.EnumPopup ("Blend mode:", blendMode);
					playMode = (AnimPlayMode) EditorGUILayout.EnumPopup ("Play mode:", playMode);
				}
			}
			
			fadeTime = EditorGUILayout.Slider ("Transition time:", fadeTime, 0f, 1f);
			willWait = EditorGUILayout.Toggle ("Pause until finish?", willWait);
		}
		
		else if (method == AnimMethodChar.SetStandard)
		{
			clip = (AnimationClip) EditorGUILayout.ObjectField ("Clip:", clip, typeof (AnimationClip), true);
			standard = (AnimStandard) EditorGUILayout.EnumPopup ("Change:", standard);
		}
		
		AfterRunningOption ();
	}
	
	override public string SetLabel ()
	{
		string labelAdd = "";
		
		if (isPlayer)
		{
			labelAdd = " ( Player )";
		}
		else if (animChar)
		{
			labelAdd = " (" + animChar.name + ")";
		}
		
		return labelAdd;
	}
	
	#endif

}