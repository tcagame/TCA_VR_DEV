using UnityEngine;
using System.Collections;

public class CubeManager : MonoBehaviour {

	const int CUBE_NUM = 15;

	struct CUBE_DATA {
		public GameObject obj;
		public Vector3 target;
		public Vector3 move;
		public float move_distance;
		public bool ok_move;
        public bool walk_flag;
        public bool line_flag;
        public int group_num;
        public int member_num;
	};

	CUBE_DATA[ ] _cubes = new CUBE_DATA[ CUBE_NUM ];
	// Use this for initialization
	void Start( ) {
		int count = 0;
		foreach ( GameObject obj in GameObject.FindGameObjectsWithTag( "cube" ) ) {
			_cubes[ count ].obj = obj;
			_cubes[ count ].target = obj.transform.position;
			_cubes[ count ].ok_move = false;
			count++;
		}
	}

	public void move( ) {
		for ( int i = 0; i < CUBE_NUM; i++ ) {
			float distance = Vector3.Distance( _cubes[ i ].obj.transform.position, _cubes[ i ].target );
			// 誤差修正
			if ( distance < 0.1f ) {
				_cubes[ i ].obj.transform.position = _cubes[ i ].target;
				_cubes[ i ].ok_move = false;
				_cubes[ i ].obj.GetComponentInParent< Rigidbody >( ).velocity = Vector3.zero;
			}
		}
	}

    public void moveStart( int num ) {
        _cubes[ num ].obj.GetComponentInParent< Rigidbody >( ).velocity = _cubes[ num ].move;
    }

    public void speedDown( ) {
        for ( int i = 0; i< CUBE_NUM; i++ ) {
            _cubes[ i ].obj.GetComponentInParent< Rigidbody >( ).velocity = Vector3.zero;
        }
    }

    public void setTarget(int num, Vector3 pos, float time)
    {
		//time = 5.0f;
		// ターゲット設定
		_cubes[ num ].target = pos;
		// 距離を測る
		float distance = Vector3.Distance( _cubes[ num ].obj.transform.position, _cubes[ num ].target );
		_cubes[ num ].move_distance = distance / time;
		// ベクトル
		_cubes[ num ].move = _cubes[ num ].target - _cubes[ num ].obj.transform.position;
		// 一フレームに移動する距離
		_cubes[ num ].move = _cubes[ num ].move.normalized * _cubes[ num ].move_distance;
        _cubes[ num ].ok_move = true;
	}

    public void setFootAnim( int num, float base_frame, float frame, AnimationClip animationclip ) {
        _cubes[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Foot" );
        
    }

    public void setWalkAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _cubes[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        if ( _cubes[ num ].walk_flag ) {
            _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "WalkAnimLeft" );
            _cubes[ num ].walk_flag = false;
        }
        else {
            _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "WalkAnimRight" );
            _cubes[ num ].walk_flag = true;
        }
    }

    public void setBackAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _cubes[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        if ( _cubes[ num ].walk_flag ) {
            _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "BackAnimLeft" );
            _cubes[ num ].walk_flag = false;
        }
        else {
            _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "BackAnimRight" );
            _cubes[ num ].walk_flag = true;
        }
    }

    public void setLineAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _cubes[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        if( _cubes[ num ].line_flag ) {
		    _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "LineFoot1" );
            _cubes[ num ].line_flag = false;
        } else {
            _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "LineFoot2" );
            _cubes[ num ].line_flag = true;
        }
    }

    public void setJumpAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _cubes[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        _cubes[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Jump" );
    }

    public void setGroup( int cube_num, int group_num, int member_num, GameObject parent ) {
        _cubes[ cube_num ].group_num  = group_num;
        _cubes[ cube_num ].member_num = member_num;
		_cubes[ cube_num ].obj.transform.parent = parent.transform;
    }

	public int cubesNum( ) {
		return _cubes.Length;
	}

    public int getGroupNum( int num ) {
        return _cubes[ num ].group_num;
    }
    
    public int getMemberNum( int num ) {
        return _cubes[ num ].member_num;
    }
}
