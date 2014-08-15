/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Action.cs"
 * 
 *	This is the base class from which all Actions derive.
 *	We need blank functions Run, ShowGUI and SetLabel,
 *	which will be over-ridden by the subclasses.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
abstract public class Action : ScriptableObject
{
	
	public bool willWait;
	protected float defaultPauseTime = 0.2f;
	
	public bool isRunning;
	public int id;

	public bool isDisplayed;
	public string title;
	
	public enum ResultAction { Continue, Stop, Skip, RunCutscene }
	public ResultAction endAction = ResultAction.Continue;
	public int skipAction;
	public Cutscene linkedCutscene;
	

	public Action ()
	{
		this.isDisplayed = true;
	}

	
	public virtual float Run ()
	{
		return defaultPauseTime;
	}
	
	
	public virtual void ShowGUI () {}
	
	
	public virtual int End ()
	{
		if (endAction == ResultAction.Stop)
		{
			return -1;
		}
		else if (endAction == ResultAction.Skip)
		{
			return (skipAction);
		}
		else if (endAction == ResultAction.RunCutscene && linkedCutscene)
		{	
			linkedCutscene.SendMessage ("Interact");
			
			if (linkedCutscene.triggerTime > 0f)
			{
				// End actionlist and cutscene
				return -1;
			}
			else
			{
				// End actionlist, but do not end cutscene
				return -2;
			}
		}

		return 0;
	}
	
	
	#if UNITY_EDITOR
	
	protected void AfterRunningOption ()
	{		
		endAction = (ResultAction) EditorGUILayout.EnumPopup("After running:", (ResultAction) endAction);

		if (endAction == ResultAction.RunCutscene)
		{
			linkedCutscene = (Cutscene) EditorGUILayout.ObjectField ("Cutscene to run", linkedCutscene, typeof (Cutscene), true);
		}
		
		else if (endAction == ResultAction.Skip)
		{
			skipAction = EditorGUILayout.IntField ("Action # to skip to", skipAction);
		}
	}
	
	
	public virtual string SetLabel ()
	{
		return ("");
	}
	
	#endif

}