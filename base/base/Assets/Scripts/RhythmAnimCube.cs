using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class RhythmAnimCube : MonoBehaviour {

	public RhythmManager rhythmmanager;
    public GameObject[] obj;
    public AnimationClip[] animationclip;

    private bool WalkFlag = true;
    private int base_frame = 60;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if ( rhythmmanager.isTiming( RhythmManager.RHYTHM_TAG.MAIN ) )
		{
            Speed( rhythmmanager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) );
			obj[ 0 ].GetComponent< Animator >( ).SetTrigger("OnceTrigger");
			if( WalkFlag )
			{
				obj[ 1 ].GetComponent< Animator >( ).SetTrigger("LeftTrigger");
				WalkFlag = false;
			}
			else {
				obj[ 1 ].GetComponent< Animator >( ).SetTrigger("RightTrigger");
				WalkFlag = true;
			}
		}
    }

    //アニメーションのスピード変更
    void Speed( int nextframe )
    {
		
		if( nextframe > 500 ) {
			return;
		}
        if ( nextframe != 0 ) {
            for (int i = 0; i < obj.Length; i++){
				if( nextframe < 35 ) {
					obj[ i ].GetComponent< Animator >( ).speed = 2.0f;
				} else {
					obj[ i ].GetComponent< Animator >( ).speed = ( animationclip[ i ].length * base_frame ) / (float)nextframe;
				}
			}
        }
	}

}
