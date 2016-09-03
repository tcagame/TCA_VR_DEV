using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class RhythmAnimCube : MonoBehaviour {

	public RhythmManager rhythmmanager;

    public GameObject[] obj;

    public Animator[] animator;

    private float _animspeed;

	private bool WalkFlag = true;

    public int _speed;

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
			_animspeed = framespeed( rhythmmanager.getNextBetweenFrame() );
		    Speed( _animspeed );
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

    //インデックスに要素があるかの確認
    public bool IsDefinedAt<T>( IList<T> self, int index )
    {
        return index < self.Count;
    }
    
    //リズム配列からスピードを計算
    public float framespeed ( int nextframe )
	{
		if(nextframe != 0)
			return _speed / (float)nextframe;
		else
			return _animspeed;
    }

    //アニメーションのスピード変更
    void Speed( float speed )
    {
        for(int i = 0; i < obj.Length; i++)
            animator[ i ].speed = speed;
	}

    /*//再生停止の設定
    void Stop( int array )
    {
        for(int i = 0; i < obj.Length; i++) {
            if( !IsDefinedAt<bool>( flag, array ) )
                flag.Add( true );
            else
            {
                if( !flag[ array ] )
                {
                    animator[ i ].Stop();
                    animator[ i ].enabled = false;
                }
                else if( flag[ array ] )
                {
                
                    animator[ i ].enabled = true;
                }
            }
            
        }
    }*/

}
