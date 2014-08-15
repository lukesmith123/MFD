/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"_Camera.cs"
 * 
 *	This is the base class for GameCamera and FirstPersonCamera.
 * 
 */


using UnityEngine;
using System.Collections;

public class _Camera : MonoBehaviour
{
	
	public Vector3 PositionRelativeToCamera (Vector3 _position)
	{
		return (_position.x * ForwardVector ()) + (_position.z * RightVector ());
	}
	
	
	public Vector3 RightVector ()
	{
		return (transform.right);
	}
	
	
	public Vector3 ForwardVector ()
	{
		Vector3 camForward;
		
		camForward = transform.forward;
		camForward.y = 0;
		
		return (camForward);
	}
	
	
	public virtual void MoveCameraInstant ()
	{ 	}
	
}
