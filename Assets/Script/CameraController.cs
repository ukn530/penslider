using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject penguin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.x = penguin.transform.position.x;
		transform.position = pos;
	}
}
