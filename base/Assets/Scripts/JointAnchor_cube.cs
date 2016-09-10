using UnityEngine;
using System.Collections;
using Common;

public class JointAnchor_cube : MonoBehaviour {

    public Transform CTRL;
    public GameObject EnemyCube;
	public GameObject _modemanager;

	// Use this for initialization
	void Start () {
		_modemanager = GameObject.Find( "ModeManager" );
	}
	
	// Update is called once per frame
	void Update () {
		if( _modemanager.GetComponent< ModeManager >( ).getMusicMode( ) == MUSIC_MODE.MODE_A_PART ) {
			if ( CTRL != null ) {
				//transform.position = CTRL.position;
				transform.rotation = CTRL.rotation;
			}
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
