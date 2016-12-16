using UnityEngine;
using System.Collections;

public class xevius_map : MonoBehaviour {

    public Vector3 scroll_spd;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += scroll_spd;
	}
}
