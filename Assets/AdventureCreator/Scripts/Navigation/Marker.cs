/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Marker.cs"
 * 
 *	This script allows a simple way of teleporting
 *	characters and objects around the scene.
 * 
 */

using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour
{

	void Awake ()
	{
		this.renderer.enabled = false;
	}
	
}
