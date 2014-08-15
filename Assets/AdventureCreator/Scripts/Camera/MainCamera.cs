/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"MainCamera.cs"
 * 
 *	This is attached to the Main Camera, and must be tagged as "MainCamera" to work.
 *	Only one Main Camera should ever exist in the scene.
 * 
 */

using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
	
	public Texture2D fadeTexture;
	public _Camera attachedCamera;

	public _Camera lastNavCamera;
	[HideInInspector] public bool isSmoothChanging;
	
	private bool cursorAffectsRotation;

	private float timeToFade = 0.5f;
	private int drawDepth = -1000;
	private float alpha = 1.0f; 
	private FadeType fadeType;
	private float fadeStartTime;
	
	private MoveMethod moveMethod;
	private float changeTime;
	
	private	Vector3 startPosition;
	private	Quaternion startRotation;
	private float startFOV;
	private	float startTime;
	
	private Transform LookAtPos;
	private Vector2 lookAtAmount;
	private float LookAtZ;
	private Vector3 lookAtTarget;
	
	private SettingsManager settingsManager;
	private StateHandler stateHandler;
	private PlayerInput playerInput;
	
	
	private void Awake()
	{
		if (GameObject.FindWithTag (Tags.gameEngine) && GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>())
		{
			playerInput = GameObject.FindWithTag (Tags.gameEngine).GetComponent <PlayerInput>();
		}
		
		if (AdvGame.GetReferences () && AdvGame.GetReferences ().settingsManager)
		{
			settingsManager = AdvGame.GetReferences ().settingsManager;
		}
	}	

	
	private void Start()
	{
		if (GameObject.FindWithTag (Tags.persistentEngine) && GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>())
		{
			stateHandler = GameObject.FindWithTag (Tags.persistentEngine).GetComponent <StateHandler>();
		}
		
		if (attachedCamera)
		{
			SnapToAttached();
		}
	
		foreach (Transform child in transform)
		{
			LookAtPos = child;
		}
		LookAtZ = LookAtPos.localPosition.z;
	}
	
	
	private void Update ()
	{
		if (stateHandler)
		{
			if (stateHandler.gameState == GameState.Normal)
			{
				SetFirstPerson ();
			}
			
			if (this.GetComponent <AudioListener>())
			{
				if (stateHandler.gameState == GameState.Paused)
				{
					AudioListener.pause = true;
				}
				else
				{
					AudioListener.pause = false;
				}
			}
		}
		
	}

	
	public void SetFirstPerson ()
	{
		if (settingsManager)
		{
			if (settingsManager.controlStyle == ControlStyle.FirstPerson)
			{
				attachedCamera = GameObject.FindWithTag (Tags.firstPersonCamera).GetComponent <_Camera>();
				SnapToAttached ();
			}
		}
		
		if (attachedCamera)
		{
			lastNavCamera = attachedCamera;
		}
	}
	
	
	private void OnGUI()
	{
		alpha = (Time.time - fadeStartTime) / timeToFade;
		
		if (fadeType == FadeType.fadeIn)
		{
			alpha = 1 - alpha;
		}
		
		alpha = Mathf.Clamp01(alpha);
		
		if (alpha > 0.0f)
		{
			Color tempColor = GUI.color;
			tempColor.a = alpha;
			GUI.color = tempColor;
			GUI.depth = drawDepth;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeTexture);
		}
	}

	
	private void LateUpdate ()
	{
		if (attachedCamera)
		{
			if (!isSmoothChanging)
			{
				GetComponent <Camera>().fieldOfView = attachedCamera.GetComponent <Camera>().fieldOfView;
				transform.rotation = attachedCamera.transform.rotation;
				transform.position = attachedCamera.transform.position;
				
				if (cursorAffectsRotation)
				{
					SetLookAtPosition ();
					transform.LookAt (LookAtPos);
				}
			}
			else
			{
				// Move from one GameCamera to another
				if (Time.time < startTime + changeTime)
				{
					if (moveMethod == MoveMethod.Linear)
					{
						transform.position = Vector3.Lerp (startPosition, attachedCamera.transform.position, AdvGame.LinearTimeFactor (startTime, changeTime)); 
						transform.rotation = Quaternion.Lerp (startRotation, attachedCamera.transform.rotation, AdvGame.LinearTimeFactor (startTime, changeTime));
						GetComponent <Camera>().fieldOfView = Mathf.Lerp (startFOV, attachedCamera.GetComponent <Camera>().fieldOfView, AdvGame.LinearTimeFactor (startTime, changeTime));
					}
					else if (moveMethod == MoveMethod.Smooth)
					{
						transform.position = Vector3.Lerp (startPosition, attachedCamera.transform.position, AdvGame.SmoothTimeFactor (startTime, changeTime)); 
						transform.rotation = Quaternion.Lerp (startRotation, attachedCamera.transform.rotation, AdvGame.SmoothTimeFactor (startTime, changeTime));
						GetComponent <Camera>().fieldOfView = Mathf.Lerp (startFOV, attachedCamera.GetComponent <Camera>().fieldOfView, AdvGame.SmoothTimeFactor (startTime, changeTime));
					}
					else
					{
						// Don't slerp y position as this will create a "bump" effect
						Vector3 newPosition = Vector3.Slerp (startPosition, attachedCamera.transform.position, AdvGame.SmoothTimeFactor (startTime, changeTime)); 
						newPosition.y = Mathf.Lerp (startPosition.y, attachedCamera.transform.position.y, AdvGame.SmoothTimeFactor (startTime, changeTime));
						transform.position = newPosition;
						
						transform.rotation = Quaternion.Slerp (startRotation, attachedCamera.transform.rotation, AdvGame.SmoothTimeFactor (startTime, changeTime));
						GetComponent <Camera>().fieldOfView = Mathf.Lerp (startFOV, attachedCamera.GetComponent <Camera>().fieldOfView, AdvGame.SmoothTimeFactor (startTime, changeTime));
					}
				}
				else
				{
					LookAtCentre ();
					isSmoothChanging = false;
				}
			}
			
			if (cursorAffectsRotation)
			{
				LookAtPos.localPosition = Vector3.Lerp (LookAtPos.localPosition, lookAtTarget, Time.deltaTime * 3f);	
			}
		}
	}

	
	private void LookAtCentre ()
	{
		if (LookAtPos)
		{
			lookAtTarget = new Vector3 (0, 0, LookAtZ);
		}
	}
	

	private void SetLookAtPosition ()
	{
		if (stateHandler.gameState == GameState.Normal)
		{
			Vector2 mouseOffset = new Vector2 (playerInput.mousePosition.x / (Screen.width / 2) - 1, playerInput.mousePosition.y / (Screen.height / 2) - 1);
			float distFromCentre = mouseOffset.magnitude;
	
			if (distFromCentre < 1.4f)
			{
				lookAtTarget = new Vector3 (mouseOffset.x * lookAtAmount.x, mouseOffset.y * lookAtAmount.y, LookAtZ);
			}
		}
	}
	
	
	public void SnapToAttached ()
	{
		if (attachedCamera.GetComponent <Camera>())
		{
			LookAtCentre ();
			isSmoothChanging = false;
			
			GetComponent <Camera>().fieldOfView = attachedCamera.GetComponent <Camera>().fieldOfView;
			transform.position = attachedCamera.transform.position;
			transform.rotation = attachedCamera.transform.rotation;
		}
	}
	
	
	public void SmoothChange (float _changeTime, MoveMethod method)
	{
		LookAtCentre ();
		moveMethod = method;
		isSmoothChanging = true;
		
		startTime = Time.time;
		changeTime = _changeTime;
		
		startPosition = transform.position;
		startRotation = transform.rotation;
		startFOV = GetComponent <Camera>().fieldOfView;
	}
	
	
	public void SetGameCamera (_Camera _camera)
	{
		attachedCamera = _camera;
		
		if (attachedCamera is GameCamera)
		{
			GameCamera gameCam = (GameCamera) attachedCamera;
			cursorAffectsRotation = gameCam.followCursor;
			lookAtAmount = gameCam.cursorInfluence;
		}
	}
	
	
	public void FadeIn (float _timeToFade)
	{
		timeToFade = _timeToFade;
		alpha = 1f;
		fadeType = FadeType.fadeIn;
		fadeStartTime = Time.time;
	}

	
	public void FadeOut (float _timeToFade)
	{
		timeToFade = _timeToFade;
		alpha = 0f;
		fadeType = FadeType.fadeOut;
		fadeStartTime = Time.time;
	}
	
	
	public bool isFading ()
	{
		if (fadeType == FadeType.fadeOut && alpha < 1f)
		{
			return true;
		}
		else if (fadeType == FadeType.fadeIn && alpha > 0f)
		{
			return true;
		}

		return false;
	}

	
	public void OnDeserializing ()
	{
		FadeIn (0.5f);
	}
	
	
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
	
}
