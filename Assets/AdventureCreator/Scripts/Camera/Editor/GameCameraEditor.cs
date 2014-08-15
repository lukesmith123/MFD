using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameCamera))]

public class GameCameraEditor : Editor
{
	
	public override void OnInspectorGUI()
	{
		
		GameCamera _target = (GameCamera) target;
		
		EditorGUILayout.BeginVertical ("Button");
			EditorGUILayout.LabelField ("Cursor influence", EditorStyles.boldLabel);
			_target.followCursor = EditorGUILayout.Toggle ("Follow cursor?", _target.followCursor);
			if (_target.followCursor)
			{
				_target.cursorInfluence = EditorGUILayout.Vector2Field ("Panning factor", _target.cursorInfluence);
			}
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.BeginVertical ("Button");
			EditorGUILayout.LabelField ("X-axis movement", EditorStyles.boldLabel);
			
			_target.lockXLocAxis = EditorGUILayout.Toggle ("Lock?", _target.lockXLocAxis);
			
			if (!_target.lockXLocAxis)
			{
				
				_target.xLocConstrainType = (CameraLocConstrainType) EditorGUILayout.EnumPopup ("Affected by:", _target.xLocConstrainType);
				
				EditorGUILayout.BeginVertical ("Button");
					_target.xGradient = EditorGUILayout.FloatField ("Influence:", _target.xGradient);
					_target.xOffset = EditorGUILayout.FloatField ("Offset:", _target.xOffset);
				EditorGUILayout.EndVertical ();
	
				_target.limitX = EditorGUILayout.BeginToggleGroup ("Constrain?", _target.limitX);
				
				EditorGUILayout.BeginVertical ("Button");
					_target.constrainX[0] = EditorGUILayout.FloatField ("Minimum:", _target.constrainX[0]);
					_target.constrainX[1] = EditorGUILayout.FloatField ("Maximum:", _target.constrainX[1]);
				EditorGUILayout.EndVertical ();
	
				EditorGUILayout.EndToggleGroup ();
				
			}
			
		EditorGUILayout.EndVertical ();
		EditorGUILayout.Space ();
		
		EditorGUILayout.BeginVertical ("Button");
			EditorGUILayout.LabelField ("Y-axis rotation", EditorStyles.boldLabel);
	
			_target.lockYRotAxis = EditorGUILayout.Toggle ("Lock?", _target.lockYRotAxis);
			
			if (!_target.lockYRotAxis)
			{
				
				_target.yRotConstrainType = (CameraRotConstrainType) EditorGUILayout.EnumPopup ("Affected by:", _target.yRotConstrainType);
				
				if (_target.yRotConstrainType != CameraRotConstrainType.LookAtTarget)
				{
					
					EditorGUILayout.BeginVertical ("Button");
						_target.yGradient = EditorGUILayout.FloatField ("Influence:", _target.yGradient);
						_target.yOffset = EditorGUILayout.FloatField ("Offset:", _target.yOffset);
					EditorGUILayout.EndVertical ();
		
					_target.limitY = EditorGUILayout.BeginToggleGroup ("Constrain?", _target.limitY);
				
					EditorGUILayout.BeginVertical ("Button");
						_target.constrainY[0] = EditorGUILayout.FloatField ("Minimum:", _target.constrainY[0]);
						_target.constrainY[1] = EditorGUILayout.FloatField ("Maximum:", _target.constrainY[1]);
					EditorGUILayout.EndVertical ();
		
					EditorGUILayout.EndToggleGroup ();
					
				}
				else
				{
					_target.targetHeight = EditorGUILayout.FloatField ("Target height offset:", _target.targetHeight);
				}
	
			}
			
		EditorGUILayout.EndVertical ();
		EditorGUILayout.Space ();
		
		EditorGUILayout.BeginVertical ("Button");
			EditorGUILayout.LabelField ("Z-axis movement", EditorStyles.boldLabel);
	
			_target.lockZLocAxis = EditorGUILayout.Toggle ("Lock?", _target.lockZLocAxis);
			
			if (!_target.lockZLocAxis)
			{
	
				_target.zLocConstrainType = (CameraLocConstrainType) EditorGUILayout.EnumPopup ("Affected by:", _target.zLocConstrainType);
				
				EditorGUILayout.BeginVertical ("Button");
					_target.zGradient = EditorGUILayout.FloatField ("Influence:", _target.zGradient);
					_target.zOffset = EditorGUILayout.FloatField ("Offset:", _target.zOffset);
				EditorGUILayout.EndVertical ();
	
				_target.limitZ = EditorGUILayout.BeginToggleGroup ("Constrain?", _target.limitZ);
				
				EditorGUILayout.BeginVertical ("Button");
					_target.constrainZ[0] = EditorGUILayout.FloatField ("Minimum:", _target.constrainZ[0]);
					_target.constrainZ[1] = EditorGUILayout.FloatField ("Maximum:", _target.constrainZ[1]);
				EditorGUILayout.EndVertical ();
	
				EditorGUILayout.EndToggleGroup ();
				
			}
		
		EditorGUILayout.EndVertical ();
		EditorGUILayout.Space ();

		if (!_target.lockXLocAxis || !_target.lockYRotAxis || !_target.lockZLocAxis)
		{
			EditorGUILayout.BeginVertical ("Button");
				EditorGUILayout.LabelField ("Target object to control camera movement", EditorStyles.boldLabel);
				
				_target.targetIsPlayer = EditorGUILayout.Toggle ("Target is player?", _target.targetIsPlayer);
				
				if (!_target.targetIsPlayer)
				{
					_target.target = (Transform) EditorGUILayout.ObjectField ("Target:", _target.target, typeof(Transform), true);
				}
				
				_target.dampSpeed = EditorGUILayout.FloatField ("Follow speed", _target.dampSpeed);
			EditorGUILayout.EndVertical ();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty (_target);
		}
		
	}
}