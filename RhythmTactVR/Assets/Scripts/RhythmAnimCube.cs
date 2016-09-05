using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class RhythmAnimCube : MonoBehaviour {

	public RhythmManager rhythmmanager;
    public GameObject[] obj;
    public Animator[] animator;
    public AnimationClip[] animationclip;

    private float _animspeed;
    private bool WalkFlag = true;
    private int base_frame = 60;

	// Use this for initialization
	void Start () {

       for(int i = 0; i < obj.Length; i++)
            animator[ i ] = obj[ i ].GetComponent (typeof(Animator)) as Animator;
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if ( rhythmmanager.isTiming() )
		{
            Speed( rhythmmanager.getNextBetweenFrame( ) );
			animator[0].SetTrigger("OnceTrigger");
			if( WalkFlag )
			{
				animator[1].SetTrigger("LeftTrigger");
				WalkFlag = false;
			}
			else {
				animator[1].SetTrigger("RightTrigger");
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
					animator[ i ].speed = 2.0f;
				} else {
					animator[ i ].speed = ( animationclip[ i ].length * base_frame ) / (float)nextframe;
				}
			}
        }
	}

}
