using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class RhythmAnimCube : MonoBehaviour {

	public GameObject _rhythmmanager;
	public GameObject _modemanager;
    public AnimationClip animationclip;
    private int base_frame = 60;

	// Use this for initialization
	void Start () {
		_rhythmmanager = GameObject.Find( "RhythmManager" );
		_modemanager = GameObject.Find( "ModeManager" );
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if( _modemanager.GetComponent< ModeManager >( ).getMusicMode( ) == MUSIC_MODE.MODE_A_PART ){
			if ( _rhythmmanager.GetComponent< RhythmManager >( ).isTiming( RhythmManager.RHYTHM_TAG.VOCAL ) ) {
				Speed( _rhythmmanager.GetComponent< RhythmManager >( ).getNextBetweenFrame( RhythmManager.RHYTHM_TAG.VOCAL ) );
				this.GetComponent< Animator >( ).Play( "Song" );
			}
		}
    }

    //アニメーションのスピード変更
    void Speed( int nextframe )
    {
		if( nextframe > 1000 ) {
			this.GetComponent< Animator >( ).speed = 1f;
			return;
		}
        if ( nextframe != 0 ) {
			this.GetComponent< Animator >( ).speed = ( animationclip.length * base_frame ) / (float)nextframe * 2.0f;
        }
	}
}
