/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"NavigationMesh.cs"
 * 
 *	This script analyses the mesh collider of the gameObject it is attached to,
 *	and calculates an array of Vector3s that describe a path to and from two given points.
 *	It is accessed by other scripts via the GetPointsArray function.
 * 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NavigationMesh : MonoBehaviour
{
	
	private bool pathFailed = false;
	private int originalLayer;
	
	private SettingsManager settingsManager;
	
	
	private void Awake ()
	{
		GetReferences ();
	}
	
	
	private void GetReferences ()
	{
		settingsManager = AdvGame.GetReferences ().settingsManager;
	}
	
	
	public void TurnOn ()
	{
		if (LayerMask.NameToLayer ("NavMesh") == -1)
		{
			Debug.LogWarning ("No 'NavMesh' layer exists - please define one in the Tags Manager.");
		}
		else
		{
			gameObject.layer = LayerMask.NameToLayer ("NavMesh");
		}
		
		if (GetComponent <Collider>() == null)
		{
			Debug.LogWarning ("A Collider component must be attached to " + this.name + " for pathfinding to work - please attach one.");
		}
	}
	
	
	public void TurnOff ()
	{
		gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
	}
	
	
	public Vector3[] GetPointsArray (Vector3 originPos, Vector3 targetPos)
	{
		List <Vector3> pointsList = new List<Vector3>();
		
		if (settingsManager == null)
		{
			GetReferences ();
		}
	
		Vector3 originalOriginPos = originPos;
		Vector3 originalTargetPos = targetPos;
		originPos = GetNearestToMesh (originPos);
		targetPos = GetNearestToMesh (targetPos);
		
		pointsList.Add (originPos);
		
		if (!IsLineClear (targetPos, originPos, false))
		{
			pointsList = FindComplexPath (originPos, targetPos, false);
			
			if (pathFailed)
			{
				Vector3 newTargetPos = GetLineBreak (pointsList [pointsList.Count - 1], targetPos);
				
				if (newTargetPos != Vector3.zero)
				{
					targetPos = newTargetPos;
					
					if (!IsLineClear (targetPos, originPos, true))
					{
						pointsList = FindComplexPath (originPos, targetPos, true);
						
						if (pathFailed)
						{
							// Couldn't find an alternative, so just clear the path
							pointsList.Clear ();
							pointsList.Add (originPos);
						}
					}
					else
					{
						// Line between origin and new target is clear
						pointsList.Clear ();
						pointsList.Add (originPos);
					}
				}
			}
		}
		
		// Finally, remove any extraneous points
		if (pointsList.Count > 2)
		{
			for (int i=0; i<pointsList.Count; i++)
			{
				for (int j=i; j<pointsList.Count; j++)
				{
					if (IsLineClear (pointsList[i], pointsList[j], false) && j > i+1)
					{
						// Point i+1 is irrelevant, remove and reset
						pointsList.RemoveRange (i+1, j-i-1);
						j=0;
						i=0;
					}
				}
			}
		}
		pointsList.Add (targetPos);
		
		if (pointsList[0] == originalOriginPos)
			pointsList.RemoveAt (0);	// Remove origin point from start
		
		// Special case where player is stuck on a collider above the mesh
		if (pointsList.Count == 1 && pointsList[0] == originPos)
		{
			pointsList[0] = originalTargetPos;
		}
		
		return pointsList.ToArray ();
	}
	
	
	private bool IsVertexImperfect (Vector3 vertex, Vector3[] blackList)
	{
		bool answer = false;
		
		foreach (Vector3 candidate in blackList)
		{
			if (vertex == candidate)
				answer = true;
		}
		
		return answer;
	}
	
	
	private float GetPathLength (List <Vector3> _pointsList, Vector3 candidatePoint, Vector3 endPos)
	{
		float length = 0f;
		
		List <Vector3> newPath = new List<Vector3>();
		foreach (Vector3 point in _pointsList)
		{
			newPath.Add (point);
		}
		newPath.Add (candidatePoint);
		newPath.Add (endPos);
		
		for (int i=1; i<newPath.Count; i++)
		{
			length += Vector3.Distance (newPath[i], newPath[i-1]);
		}
		
		return (length);
	}
	
	
	private bool IsLineClear (Vector3 startPos, Vector3 endPos, bool ignoreOthers)
	{
		// Raise positions to above mesh, so they can "look down"
		
		if (startPos.y > endPos.y)
		{
			endPos.y = startPos.y;
		}
		else
		{
			startPos.y = endPos.y;
		}
		
		Vector3 actualPos = startPos;
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray ();
			
		for (float i=0f; i<1f; i+= 0.01f)
		{
			actualPos = startPos + ((endPos - startPos) * i);
			ray = new Ray (actualPos + new Vector3 (0f, 2f, 0f), new Vector3 (0f, -1f, 0f));

			if (settingsManager && Physics.Raycast (ray, out hit, settingsManager.navMeshRaycastLength, 1 << gameObject.layer))
			{
				if (hit.collider.gameObject != this.gameObject && !ignoreOthers)
				{
					return false;
				}
			}
			else
			{
				return false;
			}

		}
		
		return true;
	}
	
	
	private Vector3 GetLineBreak (Vector3 startPos, Vector3 endPos)
	{
		// Raise positions to above mesh, so they can "look down"
		
		if (startPos.y > endPos.y)
		{
			endPos.y = startPos.y;
		}
		else
		{
			startPos.y = endPos.y;
		}
		
		Vector3 actualPos = startPos;
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray ();
			
		for (float i=0f; i<1f; i+= 0.01f)
		{
			actualPos = startPos + ((endPos - startPos) * i);
			ray = new Ray (actualPos + new Vector3 (0f, 2f, 0f), new Vector3 (0f, -1f, 0f));

			if (settingsManager && Physics.Raycast (ray, out hit, settingsManager.navMeshRaycastLength, 1 << gameObject.layer))
			{
				if (hit.collider.gameObject != this.gameObject)
				{
					return actualPos;
				}
			}
		}
		
		return Vector3.zero;
	}
	
	
	private Vector3[] CreateVertexArray (Vector3 targetPos)
	{
		Mesh mesh = transform.GetComponent <MeshCollider>().sharedMesh;
		Vector3[] _vertices = mesh.vertices;
		
		List<NavMeshData> navMeshData = new List<NavMeshData>();
		
		foreach (Vector3 vertex in _vertices)
		{
			navMeshData.Add (new NavMeshData (vertex, targetPos, this.transform));
		}
		
		navMeshData.Sort (delegate (NavMeshData a, NavMeshData b) {return a.distance.CompareTo (b.distance);});
		
		List <Vector3> vertexData = new List<Vector3>();
		foreach (NavMeshData data in navMeshData)
		{
			vertexData.Add (data.vertex);
		}
		
		return (vertexData.ToArray ());
	}
	
		
	private List<Vector3> FindComplexPath (Vector3 originPos, Vector3 targetPos, bool ignoreOthers)
	{
		targetPos = GetNearestToMesh (targetPos);
		
		pathFailed = false;
		List <Vector3> pointsList = new List<Vector3>();
		pointsList.Add (originPos);
		
		// Find nearest vertex to targetPos that originPos can also "see"
		bool pathFound = false;

		// An array of the navMesh's vertices, in order of distance from the target position
		Vector3[] vertices = CreateVertexArray (targetPos);
		
		int j=0;
		float pathLength = 0f;
		bool foundCandidate = false;
		bool foundCandidateForBoth = false;
		Vector3 candidatePoint = Vector3.zero;
		List<Vector3> imperfectCandidates = new List<Vector3>();
		
		while (!pathFound)
		{
			pathLength = 0f;
			foundCandidate = false;
			foundCandidateForBoth = false;
			
			for (int i=0; i<vertices.Length; i++)
			{
				Vector3 vertex = vertices[i];
				if (!IsVertexImperfect (vertex, imperfectCandidates.ToArray ()))
				{
					if (IsLineClear (vertex, pointsList [pointsList.Count - 1], ignoreOthers))
					{
						// Do we now have a clear path?
						if (IsLineClear (targetPos, vertex, ignoreOthers))
						{
							if (!foundCandidateForBoth)
							{								
								// Test a new candidate
								float testPathLength = GetPathLength (pointsList, vertex, targetPos);
						
								if (testPathLength < pathLength || !foundCandidate)
								{
									foundCandidate = true;
									foundCandidateForBoth = true;
									candidatePoint = vertex;
									pathLength = testPathLength;
								}
							}
							else
							{
								// Test a new candidate
								float testPathLength = GetPathLength (pointsList, vertex, targetPos);
						
								if (testPathLength < pathLength)
								{
									candidatePoint = vertex;
									pathLength = testPathLength;
								}
							}
						}
						else if (!foundCandidateForBoth)
						{
							if (!foundCandidate)
							{
								candidatePoint = vertex;
								foundCandidate = true;
								pathLength = GetPathLength (pointsList, vertex, targetPos);
							}
							else
							{
								// Test a new candidate
								float testPathLength = GetPathLength (pointsList, vertex, targetPos);
						
								if (testPathLength < pathLength)
								{
									candidatePoint = vertex;
									pathLength = testPathLength;
								}
							}
						}
					}
				}
			}
			
			if (foundCandidate)
			{
				pointsList.Add (candidatePoint);
				
				if (foundCandidateForBoth)
				{
					pathFound = true;
				}
				else
				{
					imperfectCandidates.Add (candidatePoint);
				}
			}
			
			j++;
			if (j > vertices.Length)
			{
				pathFailed = true;
				return pointsList;
			}
		}

		return pointsList;
	}
	
	
	private Vector3 GetNearestToMesh (Vector3 point)
	{
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray ();
		
		// Test to make sure starting on the collision mesh
		ray = new Ray (point + new Vector3 (0f, 2f, 0f), new Vector3 (0f, -1f, 0f));
		if (settingsManager && !Physics.Raycast (ray, out hit, settingsManager.navMeshRaycastLength, 1 << gameObject.layer))
		{
			Vector3[] vertices = CreateVertexArray (point);
			return vertices[0];
		}
		
		return (point);	
	}
	
}


public class NavMeshData
{
	
	public Vector3 vertex;
	public float distance;
	
	
	public NavMeshData (Vector3 _vertex, Vector3 _target, Transform navObject)
	{
		vertex = navObject.TransformPoint (_vertex);
		distance = Vector3.Distance (vertex, _target);
	}
	
}