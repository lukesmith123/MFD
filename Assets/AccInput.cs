using UnityEngine;
using System.Collections;

public class AccInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x+120.1f, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
	}
}
