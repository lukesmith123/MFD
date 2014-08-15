/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"FirstPersonCamera.cs"
 * 
 *	An optional script that allows First Person control.
 *	This is attached to a camera which is a child of the player.
 *	It must be tagged as "FirstPersonCamera" to work.
 *	Only one First Person Camera should ever exist in the scene at runtime.
 *	Only the yaw is affected here: the pitch is determined by the player parent object.
 * 
 */

using UnityEngine;
using System.Collections;

public class FirstPersonCamera : _Camera
{
	
	public bool smoothChange = false;
	public float targetTilt;
	public float speed;
	
	public float rotationY = 0f;
	
	public float minY = -60F;
	public float maxY = 60F;
	
	public Vector2 sensitivity = new Vector2 (15f, 15f);
	
	
	private void FixedUpdate ()
	{
		if (smoothChange)
		{
			rotationY = Mathf.Lerp (rotationY, targetTilt, Time.deltaTime * speed);
			
			if (Mathf.Abs (targetTilt - rotationY) < 2f)
			{
				smoothChange = false;
			}
		}
		
		rotationY = Mathf.Clamp (rotationY, minY, maxY);
		
		transform.localEulerAngles = new Vector3 (-rotationY, 0, 0);		
	}
	
	
	public void SetTilt (Vector3 lookAtPosition, bool isInstant)
	{
		if (isInstant)
		{
			smoothChange = false;
			
			transform.LookAt (lookAtPosition);
			float tilt = transform.localEulerAngles.x;
			
			if (tilt > 180)
			{
				rotationY = 360 - tilt;
			}
			else
			{
				rotationY = tilt;
			}
		}
		else
		{
			// Base the speed of tilt change on how much horizontal rotation is needed
			
			Vector3 flatLookVector = lookAtPosition - transform.position;
			flatLookVector.y = 0f;
			
			Player player = GameObject.FindWithTag (Tags.player).GetComponent <Player>();
			speed = 5 / Vector3.Dot (player.transform.forward.normalized, flatLookVector.normalized);
			
			smoothChange = true;
			
			Quaternion oldRotation = transform.rotation;
			transform.LookAt (lookAtPosition);
			targetTilt = transform.localEulerAngles.x;
			
			transform.rotation = oldRotation;
			
			if (targetTilt > 180)
			{
				targetTilt = 360 - targetTilt;
			}
		}
	}
	
}
