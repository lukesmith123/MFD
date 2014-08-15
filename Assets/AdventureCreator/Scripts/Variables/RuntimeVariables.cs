/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"RuntimeVariables.cs"
 * 
 *	This script creates a local copy of the VariableManager's vars.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RuntimeVariables : MonoBehaviour
{
	
	[HideInInspector] public List<GVar> localVars = new List<GVar>();
	
	
	public void Awake ()
	{
		// Transfer the vars set in VariablesManager to self on runtime
		UpdateSelf ();
	}
	
	
	private void UpdateSelf ()
	{
		if (AdvGame.GetReferences () && AdvGame.GetReferences ().variablesManager)
		{
			VariablesManager variablesManager = AdvGame.GetReferences ().variablesManager;

			localVars.Clear ();
			foreach (GVar assetVar in variablesManager.vars)
			{
				localVars.Add (new GVar (assetVar));
			}
		}
	}
	
	
	public void SendVars (List<GVar> vars)
	{
		localVars = vars;
	}
	
	
	public VariableType GetVarType (int _id)
	{
		foreach (GVar _var in localVars)
		{
			if (_var.id == _id)
			{
				return _var.type;
			}
		}
		
		Debug.LogWarning ("Variable not found!");
		
		return VariableType.Boolean;
	}
	
	
	public void SetValue (int _id, int newValue, bool isCumulative)
	{
		foreach (GVar _var in localVars)
		{
			if (_var.id == _id)
			{
				if (isCumulative)
				{
					_var.val += newValue;
				}
				else
				{
					_var.val = newValue;
				}
				
				if (_var.type == VariableType.Boolean)
				{
					if (_var.val > 0)
					{
						_var.val = 1;
					}
					else
					{
						_var.val = 0;
					}
				}
			}
		}
	}
	
}
