using UnityEngine;
using System.Collections;

public class SoundCube : MonoBehaviour {

    public Vector3 RollVec = Vector3.up;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, RollVec, 20 * Time.deltaTime);
    }
}
