/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Char.cs"
 * 
 *	This is the base class for both NPCs and the Player.
 *	It contains the functions needed for animation and movement.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Char : MonoBehaviour
{
	
	public AnimationClip idleAnim;
	public AnimationClip walkAnim;
	public AnimationClip runAnim;
	public AnimationClip turnLeftAnim;
	public AnimationClip turnRightAnim;
	
	public Transform upperBodyBone;
	public Transform leftArmBone;
	public Transform rightArmBone;
	public Transform neckBone;
	public Transform leftHandBone;
	public Transform rightHandBone;
	
	public float walkSpeedScale = 2f;
	public float runSpeedScale = 6f;
	public float turnSpeed = 7f;
	public float acceleration = 6f;

	[HideInInspector] public Paths activePath = null;
	public bool isRunning { get; set; }
	
	[HideInInspector] public CharState charState;
	
	protected float moveSpeed;
	protected Vector3 moveDirection; 
	
	protected int targetNode = 0;
	protected bool pausePath = false;

	private Vector3 lookDirection;
	private float pausePathTime;
	private int prevNode = 0;
	private Vector3 oldPosition;
	
	private bool isTurning = false;
	private bool isTurningLeft = false;
	private float animCrossfadeSpeed = 0.2f;
	
	protected StateHandler stateHandler;
	

	private void Awake ()
	{
		ResetBaseClips ();
	}
	
	
	private void Start ()
	{
		lookDirection = transform.forward;
		
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
		{
			stateHandler = GameObject.FindWithTag(Tags.persistentEngine).GetComponent<StateHandler>();
		}
	}
	
	
	protected void FixedUpdate ()
	{
		PathUpdate ();
		SpeedUpdate ();
		PhysicsUpdate ();
		AnimUpdate ();
		MoveUpdate ();  
	}
	
	
	protected void PathUpdate ()
	{
		if (activePath && activePath.nodes.Count > 0)
		{
			if (pausePath)
			{
				if (Time.time > pausePathTime)
				{
					pausePath = false;
					SetNextNodes ();
				}
			}
			else
			{
				Vector3 direction = activePath.nodes[targetNode] - transform.position;
				
				if (!activePath.affectY)
				{
					direction.y = 0;
				}
				
				SetLookDirection (direction, false);
				SetMoveDirectionAsForward ();
				
				if (direction.magnitude < 0.5)
				{
					if ((targetNode == 0 && prevNode == 0) || activePath.nodePause <= 0)
					{
						SetNextNodes ();
					}
					else
					{
						PausePath ();
					}
				}
			}
		}
	}
	
	
	private void SpeedUpdate ()
	{
		if (charState == CharState.Move)
		{
			Accelerate ();
		}
		else if (charState == CharState.Decelerate || charState == CharState.Custom)
		{
			Decelerate ();
		}
	}

		
	private void PhysicsUpdate ()
	{
		if (rigidbody)
		{
			if (charState == CharState.Custom && moveSpeed < 0.01f)
			{
				rigidbody.useGravity = false;
			}
			else
			{
				if (activePath && activePath.affectY)
				{
					rigidbody.useGravity = false;
				}
				else
				{
					rigidbody.useGravity = true;
				}
			}
		}
		else
		{
			Debug.LogWarning ("No rigidbody attached");
		}
	}
	
	
	private void AnimUpdate ()
	{
		if (charState == CharState.Idle || charState == CharState.Decelerate)
		{
			if (isTurning)
			{
				if (isTurningLeft && turnLeftAnim)
				{
					PlayStandardAnim (turnLeftAnim, false);
				}
				else if (!isTurningLeft && turnRightAnim)
				{
					PlayStandardAnim (turnRightAnim, false);
				}
				else
				{
					PlayIdle ();
				}
			}
			else
			{
				PlayIdle ();
			}
		}
		
		else if (charState == CharState.Move)
		{
			if (isRunning)
			{
				PlayRun ();
			}
			else
			{
				PlayWalk ();
			}
		}	
	}

	
	
	private void MoveUpdate ()
	{
		if (moveSpeed > 0.01f && rigidbody)
		{
			Vector3 newVel;
			newVel = moveDirection * moveSpeed * walkSpeedScale;
			rigidbody.MovePosition (rigidbody.position + newVel * Time.deltaTime);
		}
		
		if (isTurning)
		{
			Turn (false);
		}
	}
	
	
	private void Accelerate ()
	{
		float targetSpeed;
		
		if (isRunning)
		{
			targetSpeed = moveDirection.magnitude * runSpeedScale / walkSpeedScale;
		}
		else
		{
			targetSpeed = moveDirection.magnitude;
		}
		
		moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, Time.deltaTime * acceleration);
	}

	
	private void Decelerate ()
	{
		moveSpeed = Mathf.Lerp (moveSpeed, 0f, Time.deltaTime * acceleration);

		if (moveSpeed < 0.01f)
		{
			moveSpeed = 0f;
			
			if (charState != CharState.Custom)
			{
				charState = CharState.Idle;
			}
		}
	}
	
	
	public bool IsTurning ()
	{
		return isTurning;
	}
	
	
	public void Turn (bool isInstant)
	{
		if (lookDirection != Vector3.zero)
		{
			float actualTurnSpeed = turnSpeed * Time.deltaTime;
			if (moveSpeed == 0f)
			{
				actualTurnSpeed /= 2;
			}
			
			Quaternion targetRotation = Quaternion.LookRotation (lookDirection, Vector3.up);
			Quaternion newRotation = Quaternion.Lerp (rigidbody.rotation, targetRotation, actualTurnSpeed);
			
			if (isInstant)
			{
				isTurning = false;
				rigidbody.rotation = targetRotation;
			}
			else 
			{
				isTurning = true;
				rigidbody.MoveRotation (newRotation);
				
				// Determine if character is turning left or right (courtesy of Duck: http://answers.unity3d.com/questions/26783/how-to-get-the-signed-angle-between-two-quaternion.html)
				
			    Vector3 forwardA = targetRotation * Vector3.forward;
			    Vector3 forwardB = transform.rotation * Vector3.forward;
			     
			    float angleA = Mathf.Atan2 (forwardA.x, forwardA.z) * Mathf.Rad2Deg;
			    float angleB = Mathf.Atan2 (forwardB.x, forwardB.z) * Mathf.Rad2Deg;
			     
			    float angleDiff = Mathf.DeltaAngle( angleA, angleB );

				if (angleDiff < 0f)
				{
					isTurningLeft = false;
				}
				else
				{
					isTurningLeft = true;
				}
				
				if (Quaternion.Angle (Quaternion.LookRotation (lookDirection), transform.rotation) < 3f)
				{
					isTurning = false;
				}
			}
				
		}
	}
	
	
	public void SetLookDirection (Vector3 _direction, bool isInstant)
	{
		lookDirection = _direction;
		Turn (isInstant);
	}
	
	
	public void SetMoveDirection (Vector3 _direction)
	{
		Quaternion targetRotation = Quaternion.LookRotation (_direction, Vector3.up);
		moveDirection = targetRotation * Vector3.forward;
		moveDirection.Normalize ();
		
	}
	
	
	public void SetMoveDirectionAsForward ()
	{
		moveDirection = transform.forward;
		moveDirection.Normalize ();
	}
	
	
	public void SetMoveDirectionAsBackward ()
	{
		moveDirection = -transform.forward;
		moveDirection.Normalize ();
	}
	
	
	public Vector3 GetMoveDirection ()
	{
		return moveDirection;	
	}
	
	
	private void SetNextNodes ()
	{
		int tempPrev = targetNode;
		
		if (this.GetComponent <Player>() && stateHandler.gameState == GameState.Normal)
		{
			targetNode = activePath.GetNextNode (targetNode, prevNode, true);
		}
		else
		{
			targetNode = activePath.GetNextNode (targetNode, prevNode, false);
		}
		
		prevNode = tempPrev;
		
		if (targetNode == -1)
		{
			EndPath ();
		}
	}
	
	
	public void EndPath ()
	{
		activePath = null;
		targetNode = 0;
		charState = CharState.Decelerate;
	}
	
	
	public void Halt ()
	{
		activePath = null;
		targetNode = 0;
		charState = CharState.Idle;
		moveSpeed = 0f;
	}
	
	
	protected void ReverseDirection ()
	{
		int tempPrev = targetNode;
		targetNode = prevNode;
		prevNode = tempPrev;
	}
	
	
	private void PausePath ()
	{
		charState = CharState.Decelerate;
		pausePath = true;
		pausePathTime = Time.time + activePath.nodePause;
	}
	
	
	public void SetPath (Paths pathOb, PathSpeed _speed)
	{
		activePath = pathOb;
		targetNode = 0;
		prevNode = 0;
		
		if (pathOb)
		{
			if (_speed == PathSpeed.Run)
			{
				isRunning = true;
			}
			else
			{
				isRunning = false;
			}
		}
		
		charState = CharState.Idle;
	}
	
	
	public void SetPath (Paths pathOb)
	{
		activePath = pathOb;
		targetNode = 0;
		prevNode = 0;
		
		if (pathOb)
		{
			if (pathOb.pathSpeed == PathSpeed.Run)
			{
				isRunning = true;
			}
			else
			{
				isRunning = false;
			}
		}

		charState = CharState.Idle;
	}
	
	
	public void SetPath (Paths pathOb, int _targetNode, int _prevNode)
	{
		activePath = pathOb;
		targetNode = _targetNode;
		prevNode = _prevNode;
		
		if (pathOb)
		{
			if (pathOb.pathSpeed == PathSpeed.Run)
			{
				isRunning = true;
			}
			else
			{
				isRunning = false;
			}
		}
		
		charState = CharState.Idle;
	}
	
	
	protected void CheckIfStuck ()
	{
		// Check for null movement error: if not moving on a path, end the path
		
		Vector3 newPosition = rigidbody.position;
		if (oldPosition == newPosition)
		{
			Debug.Log ("Stuck in active path - removing");
			EndPath ();
		}

		oldPosition = newPosition;
	}
	
	public Paths GetPath ()
	{
		return activePath;
	}
	
	
	public int GetTargetNode ()
	{
		return targetNode;
	}
	
	
	public int GetPrevNode ()
	{
		return prevNode;
	}
	
	
	public void MoveToPoint (Vector3 point, bool run)
	{
		List<Vector3> pointData = new List<Vector3>();
		pointData.Add (point);
		MoveAlongPoints (pointData.ToArray (), run);
	}


	public void MoveAlongPoints (Vector3[] pointData, bool run)
	{
		Paths path = GetComponent <Paths>();
		if (path)
		{
			path.BuildNavPath (pointData);
			
			if (run)
			{
				SetPath (path, PathSpeed.Run);
			}
			else
			{
				SetPath (path, PathSpeed.Walk);
			}
		}
		else
		{
			Debug.LogWarning (this.name + " cannot pathfind without a Paths component");
		}
	}
	

	public void ResetBaseClips ()
	{
		// Remove all animations except Idle, Walk, Run and Talk
		
		List <string> clipsToRemove = new List <string>();
		
		foreach (AnimationState state in animation)
		{
			if ((idleAnim == null || state.name != idleAnim.name) && (walkAnim == null || state.name != walkAnim.name) && (runAnim == null || state.name != runAnim.name))
			{
				clipsToRemove.Add (state.name);
			}
		}

		foreach (string _clip in clipsToRemove)
		{
			animation.RemoveClip (_clip);
		}
		
	}
	
	
	public void PlayIdle ()
	{
		if (idleAnim)
		{
			PlayStandardAnim (idleAnim, true);
		}
	}
	
	
	private void PlayWalk ()
	{
		if (walkAnim)
		{
			PlayStandardAnim (walkAnim, true);
		}
	}
	
	
	private void PlayRun ()
	{
		if (runAnim)
		{
			PlayStandardAnim (runAnim, true);
		}
	}
	
	
	private void PlayStandardAnim (AnimationClip clip, bool doLoop)
	{
		if (animation[clip.name] != null)
		{
			if (!animation [clip.name].enabled)
			{
				if (doLoop)
				{
					AdvGame.PlayAnimClip (this.animation, (int) AnimLayer.Base, clip, AnimationBlendMode.Blend, WrapMode.Loop, animCrossfadeSpeed, null);
				}
				else
				{
					AdvGame.PlayAnimClip (this.animation, (int) AnimLayer.Base, clip, AnimationBlendMode.Blend, WrapMode.Once, animCrossfadeSpeed, null);
				}
			}
		}
		else
		{
			if (doLoop)
			{
				AdvGame.PlayAnimClip (this.animation, (int) AnimLayer.Base, clip, AnimationBlendMode.Blend, WrapMode.Loop, animCrossfadeSpeed, null);
			}
			else
			{
				AdvGame.PlayAnimClip (this.animation, (int) AnimLayer.Base, clip, AnimationBlendMode.Blend, WrapMode.Once, animCrossfadeSpeed, null);
			}
		}
	}

}