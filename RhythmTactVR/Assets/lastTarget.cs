using UnityEngine;
using System.Collections;

public class lastTarget : MonoBehaviour {

	public Transform		ctrl_pos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = ctrl_pos.rotation;

	}
}
