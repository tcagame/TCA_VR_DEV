using UnityEngine;
using System.Collections;
using Common;                   //タイミング系enum.

public class JointAnchor_cube : MonoBehaviour {

    public Transform CTRL;
    public GameObject EnemyCube;
	public GameObject _modemanager;
	public Vector3 _scale;

	// Use this for initialization
	void Awake () {
		_modemanager = GameObject.Find( "ModeManager" );
		GetComponentInChildren< Neon >( ).enabled = false;
	}    	
	
    // Update is called once per frame
	void FixedUpdate () {
		if( _modemanager.GetComponent< ModeManager >( ).getMusicMode( ) == MUSIC_MODE.MODE_A_PART ) {
			if ( CTRL != null ) {
				//transform.position = CTRL.position;
				transform.rotation = CTRL.rotation;
			}
		}
		if( _modemanager.GetComponent< ModeManager >( ).getMusicMode( ) == MUSIC_MODE.MODE_B_PART ) {
			if( transform.localScale.x < _scale.x && transform.localScale.y < _scale.y && transform.localScale.z < _scale.z ) {
				transform.localScale += transform.localScale * Time.deltaTime;
			}
		}
		if( _modemanager.GetComponent< ModeManager >( ).getMusicMode( ) >= MUSIC_MODE.MODE_SABI ) {
			GetComponentInChildren< Neon >( ).enabled = true;
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
