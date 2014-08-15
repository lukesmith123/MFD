/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"GameCamera.cs"
 * 
 *	This is attached to cameras that act as "guides" for the Main Camera.
 *	They are never active: only the Main Camera is ever active.
 * 
 */

using UnityEngine;
using System.Collections;

public class GameCamera : _Camera
{

	public bool followCursor = false;
	public Vector2 cursorInfluence = new Vector2 (0.3f, 0.1f);

	public bool targetIsPlayer = true;
	public Transform target;

	public bool lockXLocAxis = true;
	public bool lockYRotAxis = true;
	public bool lockZLocAxis = true;

	public CameraLocConstrainType xLocConstrainType;
	public CameraRotConstrainType yRotConstrainType;
	public CameraLocConstrainType zLocConstrainType;
	
	public float xGradient = 1f;
	public float yGradient = 2f;
	public float zGradient = 1f;

	public float xOffset = 0f;
	public float yOffset = 0f;
	public float zOffset = 0f;
	
	public bool limitX;
	public bool limitY;
	public bool limitZ;

	public float targetHeight;

	public Vector2 constrainX;
	public Vector2 constrainY;
	public Vector2 constrainZ;
	
	public float dampSpeed = 0.9f;

	private Vector3 desiredPosition;
	private float desiredRotation;
	
	private Vector3 originalTargetPosition;
	private Vector3 originalPosition;
	private float originalRotation;

	
	void Awake ()
	{
		this.camera.enabled = false;
		
		originalPosition = transform.position;
		originalRotation = transform.eulerAngles.y;
		
		desiredPosition = transform.position;
		desiredRotation = transform.eulerAngles.y;
		
		if (!lockXLocAxis && limitX)
		{
			desiredPosition.x = ConstrainAxis (desiredPosition.x, constrainX);
		}
		
		if (!lockYRotAxis && limitY && yRotConstrainType != CameraRotConstrainType.LookAtTarget)
		{
			desiredRotation = ConstrainAxis (desiredRotation, constrainY);
		}
		
		if (!lockZLocAxis && limitZ)
		{
			desiredPosition.z = ConstrainAxis (desiredPosition.z, constrainZ);
		}
	}
	
	
	void Start ()
	{
		if (targetIsPlayer && GameObject.FindWithTag (Tags.player))
		{
			target = GameObject.FindWithTag (Tags.player).transform;
		}
		
		if (target)
		{
			SetTargetOriginalPosition ();
			MoveCameraInstant ();
		}
	}

	
	public void SwitchTarget (Transform _target)
	{
		target = _target;
		originalTargetPosition = Vector3.zero;
	}
	
	
	void Update ()
	{
		if (target)
		{
			SetTargetOriginalPosition ();
			MoveCamera ();
		}
	}

	
	private void SetTargetOriginalPosition ()
	{
		if (originalTargetPosition == Vector3.zero)
		{
			originalTargetPosition = target.transform.position;
		}
	}
	
	
	private float ConstrainAxis (float desired, Vector2 range)
	{
		if (range.x < range.y)
		{
			desired = Mathf.Clamp (desired, range.x, range.y);
		}
		
		else if (range.x > range.y)
		{
			desired = Mathf.Clamp (desired, range.y, range.x);
		}
		
		else
		{
			desired = range.x;
		}
			
		return desired;
	}

	
	private float GetDesiredPosition (float originalValue, float gradient, float offset, CameraLocConstrainType constrainType )
	{
		float desiredPosition = originalValue + offset;
		
		if (constrainType == CameraLocConstrainType.TargetX)
		{
			desiredPosition += (target.transform.position.x - originalTargetPosition.x) * gradient;
		}
		
		else if (constrainType == CameraLocConstrainType.TargetZ)
		{
			desiredPosition += (target.transform.position.z - originalTargetPosition.z) * gradient;
		}
		
		else if (constrainType == CameraLocConstrainType.TargetIntoScreen)
		{
			desiredPosition += (PositionRelativeToCamera (originalTargetPosition).x - PositionRelativeToCamera (target.position).x) * gradient;
		}
		
		else if (constrainType == CameraLocConstrainType.TargetAcrossScreen)
		{
			desiredPosition += (PositionRelativeToCamera (originalTargetPosition).z - PositionRelativeToCamera (target.position).z) * gradient;
		}

		return desiredPosition;
	}
	
	
	private void MoveCamera ()
	{
		SetDesired ();
		
		if (!lockXLocAxis || !lockZLocAxis)
		{
			transform.position = Vector3.Lerp (transform.position, desiredPosition, Time.deltaTime * dampSpeed);
		}
		
		if (!lockYRotAxis)
		{
			if (yRotConstrainType == CameraRotConstrainType.LookAtTarget)
			{
				if (target)
				{
					Vector3 lookAtPos = target.position;
					lookAtPos.y += targetHeight;
					
					// Look at and dampen the rotation
					Quaternion rotation = Quaternion.LookRotation (lookAtPos - transform.position);
					
					transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * dampSpeed);
					
				}
				else if (!targetIsPlayer)
				{
					Debug.LogWarning (this.name + " has no target");
				}
			}
			else
			{
				float newRotation = Mathf.Lerp (transform.eulerAngles.y, desiredRotation, Time.deltaTime * dampSpeed);
				transform.eulerAngles = new Vector3 (transform.eulerAngles.x, newRotation, transform.eulerAngles.z);
			}
		}
	}
	
	
	public override void MoveCameraInstant ()
	{
		if (targetIsPlayer && GameObject.FindWithTag (Tags.player))
		{
			target = GameObject.FindWithTag (Tags.player).transform;
		}
				
		SetDesired ();
		
		if (!lockXLocAxis || !lockZLocAxis)
		{
			transform.position = desiredPosition;
		}
		
		if (!lockYRotAxis)
		{
			if (yRotConstrainType == CameraRotConstrainType.LookAtTarget)
			{
				if (target)
				{
					Vector3 lookAtPos = target.position;
					lookAtPos.y += targetHeight;
					
					Quaternion rotation = Quaternion.LookRotation (lookAtPos - transform.position);
					transform.rotation = rotation;
				}
			}
			else
			{
				transform.eulerAngles = new Vector3 (transform.eulerAngles.x, desiredRotation, transform.eulerAngles.z);
			}
		}
	}

	
	private void SetDesired ()
	{
		if (lockXLocAxis)
		{
			desiredPosition.x = transform.position.x;
		}
		else
		{
			if (target)
			{
				desiredPosition.x = GetDesiredPosition (originalPosition.x, xGradient, xOffset, xLocConstrainType);
			}
			
			if (limitX)
			{
				desiredPosition.x = ConstrainAxis (desiredPosition.x, constrainX);
			}
		}
		
		if (lockYRotAxis)
		{
			desiredRotation = 0f;
		}
		else
		{
			if (target)
			{
				desiredRotation = GetDesiredPosition (originalRotation, yGradient, yOffset, (CameraLocConstrainType) yRotConstrainType);
			}
			
			if (limitY)
			{
				desiredRotation = ConstrainAxis (desiredRotation, constrainY);
			}

		}
		
		if (lockZLocAxis)
		{
			desiredPosition.z = transform.position.z;
		}
		else
		{
			if (target)
			{
				desiredPosition.z = GetDesiredPosition (originalPosition.z, zGradient, zOffset, zLocConstrainType);
			}
			
			if (limitZ)
			{
				desiredPosition.z = ConstrainAxis (desiredPosition.z, constrainZ);
			}
		}
	
		desiredPosition.y = transform.position.y;
	}
	
}