using UnityEngine;
using System.Collections;

public class JointAnchor_cube : MonoBehaviour {

    public Transform CTRL;
    public GameObject EnemyCube;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if ( CTRL != null ) {
            //transform.position = CTRL.position;
            transform.rotation = CTRL.rotation;
        }
	}

    public void Set_CTRL(Transform tr)
    {
        CTRL = tr;
    }

    public void Set_Enemy(GameObject ec)
    {
        EnemyCube = ec;
    }

}
