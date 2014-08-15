/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Paths.cs"
 * 
 *	This script stores a series of "nodes", which act
 *	as waypoints for character movement.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Paths : MonoBehaviour
{
	
	public List<Vector3> nodes = new List<Vector3>();
	
	public PathType pathType = PathType.ForwardOnly;
	public PathSpeed pathSpeed;
	public bool affectY;
	public float nodePause;
	
	
	void Awake ()
	{
		if (nodePause < 0f)
		{
			nodePause = 0f;
		}
	}
	
	
	public bool WillStopAtNextNode (int currentNode)
	{
		if (GetNextNode (currentNode, currentNode-1, false) == -1)
		{
			return true;
		}
		
		return false;
	}
	
	
	public void BuildNavPath (Vector3[] pointData)
	{
		if (pointData.Length > 0)
		{
			pathType = PathType.ForwardOnly;
			affectY = false;
			nodePause = 0;
			
			nodes.Clear ();
			nodes.Add (this.transform.position);
			
			foreach (Vector3 point in pointData)
			{
				nodes.Add (point);
			}
		}
	}
	
	
	public int GetNextNode (int currentNode, int prevNode, bool playerControlled)
	{
		int numNodes = nodes.Count;
		
		if (numNodes == 1)
		{
			return -1;
		}
		
		else if (playerControlled)
		{
			if (currentNode == 0)
			{
				return 1;
			}
			else if (currentNode == numNodes - 1)
			{
				return -1;
			}

			return (currentNode + 1);
		}
		
		else
		{
			if (pathType == PathType.ForwardOnly)
			{
				if (currentNode == numNodes - 1)
				{
					return -1;
				}

				return (currentNode + 1);
			}
			
			else if (pathType == PathType.Loop)
			{
				if (currentNode == numNodes-1)
				{
					return 0;
				}

				return (currentNode + 1);
			}
			
			else if (pathType == PathType.PingPong)
			{
				if (prevNode > currentNode)
				{
					// Going backwards
					if (currentNode == 0)
					{
						return 1;
					}
					else
					{
						return (currentNode - 1);
					}
				}
				else
				{
					// Going forwards
					if (currentNode == numNodes-1)
					{
						return (currentNode - 1);
					}
					
					return (currentNode + 1);
				}
			}
			
			else if (pathType == PathType.IsRandom)
			{
				if (numNodes > 0)
				{
					int randomNode = Random.Range (0, numNodes);
					
					while (randomNode == currentNode)
					{
						randomNode = Random.Range (0, numNodes);
					}
					
					return (randomNode);
				}
				
				return 0;
			}
			
			return -1;
		}
	}
	
	
	void OnDrawGizmos ()
	{
		// Draws a blue line from this transform to the target
		Gizmos.color = Color.blue;
		int i;
		int numNodes = nodes.Count;
		
		if (pathType == PathType.IsRandom && numNodes > 1)
		{
			for (i=1; i<numNodes; i++)
			{
				for (int j=0; j<numNodes; j++)
				{
					if (i != j)
					{
						ConnectNodes (i,j);
					}
				}
			}
		}
		else
		{
			if (numNodes > 1)
			{
				for (i=1; i<numNodes; i++)
				{
					Gizmos.DrawIcon (nodes[i], "", true);
					
					ConnectNodes (i, i - 1);
				}
			}
			
			if (pathType == PathType.Loop)
			{
				if (numNodes > 2)
				{
					ConnectNodes (numNodes-1, 0);
				}
			}
		}
	
	}
	
	
	void ConnectNodes (int a, int b)
	{
		Vector3 PosA  = nodes[a];
		Vector3 PosB = nodes[b];
		Gizmos.DrawLine (PosA, PosB);
	}
	
}
