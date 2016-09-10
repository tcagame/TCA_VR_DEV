using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class CubeManager : MonoBehaviour {

    const int CUBE_NUM = 15;
    const float TARGET_DISTANCE = 5.0f;
    const float DANCE_FOUR_TIMES_RACQUET = 80.0f;

    /// <summary>
    /// 敵キューブの構造体
    /// </summary>
	private class ENEMY_CUBE {
        public GameObject obj;
        public RhythmManager.RHYTHM_TAG tag;
    };
    
	public class ALLIY_CUBE {
        public JointAnchor_cube obj;
		public Vector3 target;
		public Quaternion targetAngle;
		public Vector3 move;
		public float move_distance;
		public bool ok_move;
        public bool walk_flag;
        public bool line_flag;
        public int group_num;
        public int member_num;
        public RhythmManager.RHYTHM_TAG tag;
    };

    /*
    /// <summary>
    /// 味方キューブの構造体
    /// </summary>
	private struct ALLIY_CUBE_DATA {
        public JointAnchor_cube obj;
		public Vector3 target;
		public Vector3 move;
		public float move_distance;
		public bool ok_move;
        public bool walk_flag;
        public bool line_flag;
        public int group_num;
        public int member_num;
        public RhythmManager.RHYTHM_TAG tag;
    };
    */

    // enemyprefab
    public GameObject _enemy_prefab;
    // player
    public Transform _target;

    public FileManager _file_manager;
    public RhythmManager _rhythm_manager;


    // 敵cube
    private List< ENEMY_CUBE > _enemy_list = new List< ENEMY_CUBE >( );
    // 味方cube
    public List< ALLIY_CUBE > _alliy_list = new List< ALLIY_CUBE >( );

    int _create_count_main       = 0;       // 打ち出すエネミーの番号
    int _comming_main_rhyrhm_num = 0;       // 仮のリズム番号

    void Awake( ) {
		if ( !_enemy_prefab ) {
			_enemy_prefab = ( GameObject )Resources.Load( "Prefabs/vr_01_fix_02" );
		}
	}

	// Use this for initialization
	void Start( ) {
        /*
        // キューブのデータ登録
		int count = 0;
		foreach ( GameObject obj in GameObject.FindGameObjectsWithTag( "cube" ) ) {
			_alliy_list[ count ].obj = obj.GetComponent< JointAnchor_cube >();
			_alliy_list[ count ].target = obj.transform.position;
			_alliy_list[ count ].ok_move = false;
			count++;
		}*/
        
	}

	void FixedUpdate( ) {
        // 敵キューブの挙動
        enemyUpdate( );
	}

    void enemyUpdate( ) {
        /// <summary>
        /// メインリズムの処理
        /// </summary>
		if ( Input.GetMouseButtonDown( 0 ) || _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MAIN ) ) {
            if( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MAIN ) % 4 == 3 ) {
			    // Small敵キューブの移動
			    enemyMove( RhythmManager.RHYTHM_TAG.MAIN );
			    // 遅延キューブを発射させる
			    for ( int i = 0; i < _enemy_list.Count; i++ ) {
				    if ( _enemy_list[ i ].obj.GetComponent< Enemy >( ).getObjType( ) == Enemy.OBJECT_TYPE.SLOW_MIDDLE &&
					     _enemy_list[ i ].obj.GetComponent< Enemy >( ).isStart( ) == false ) {
					    _enemy_list[ i ].obj.GetComponent< Rigidbody >( ).AddForce( _enemy_list[ i ].obj.GetComponent< Enemy >( ).getDir( ) * _enemy_list[ i ].obj.GetComponent< Enemy >( ).getSpeed( ) );
					    _enemy_list[ i ].obj.GetComponent< Enemy >( ).started( );
				    }
			    }
            }
			// キューブの生成
			if ( _create_count_main < _file_manager.getRhythmCount( RhythmManager.RHYTHM_TAG.MAIN ) ) {
				if ( ( _comming_main_rhyrhm_num == _file_manager.getRhythmForNum( _create_count_main, RhythmManager.RHYTHM_TAG.MAIN ).rhythm_num )  ||
					 ( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MAIN ) == _file_manager.getRhythmForNum( _create_count_main, RhythmManager.RHYTHM_TAG.MAIN ).rhythm_num ) ) {
					enemyCreate( _create_count_main, RhythmManager.RHYTHM_TAG.MAIN );
					_create_count_main++;
				}
			}

            _comming_main_rhyrhm_num++;
		}
    }
    
	void enemyMove( RhythmManager.RHYTHM_TAG tag ) {
		int num = _enemy_list.Count;
		
		if ( num > 0 ) {
			for ( int i = 0; i < num; i++ ) {
				if ( _enemy_list[ i ].obj.GetComponent< Enemy >( ).getObjType( ) == Enemy.OBJECT_TYPE.FAST_SMALL ||
					 _enemy_list[ i ].obj.GetComponent< Enemy >( ).getObjType( ) == Enemy.OBJECT_TYPE.SLOW_SMALL ) {
					SetEnemyPos( i, tag );
				}
			}
		}
	}
    
	public void SetEnemyPos( int num, RhythmManager.RHYTHM_TAG tag ) {
		Vector3 pos = _enemy_list[ num ].obj.transform.position;
		ENEMY_GENERATOR.ENEMY_DATA enemy = _file_manager.getRhythmForNum( num, tag );
		
		_enemy_list[ num ].obj.transform.position = new Vector3( pos.x + _enemy_list[ num ].obj.GetComponent< Enemy >( ).getDir( ).x * enemy.speed,
														pos.y + _enemy_list[ num ].obj.GetComponent< Enemy >( ).getDir( ).y * enemy.speed,
														pos.z + _enemy_list[ num ].obj.GetComponent< Enemy >( ).getDir( ).z * enemy.speed );
	}

    public void enemyCreate( int count, RhythmManager.RHYTHM_TAG tag ) {
        ENEMY_CUBE enemy_cube_data = new ENEMY_CUBE( );

        enemy_cube_data.tag = tag;
		// 値の設定
		GameObject obj = ( GameObject )Instantiate( _enemy_prefab, _file_manager.getRhythmForNum( count, tag ).create_pos, Quaternion.identity );
		obj.GetComponent< Enemy >( ).setObjType( _file_manager.getRhythmForNum( count, tag ).obj_type );
		obj.GetComponent< Enemy >( ).setSpeed( _file_manager.getRhythmForNum( count, tag ).speed );
		obj.GetComponent< Enemy >( ).setTargetType( _file_manager.getRhythmForNum( count, tag ).target_type );

        // マネージャーの配下に設定
        obj.transform.parent = transform;

		// ターゲットの設定
		Vector3 pos = _target.position;
        switch ( obj.GetComponent< Enemy >( ).getTargetType( ) ) {
            case Enemy.TARGET_TYPE.NORTH:
                pos.y += TARGET_DISTANCE;
                break;
            case Enemy.TARGET_TYPE.NORTH_EAST:
                pos.x += TARGET_DISTANCE;
                pos.y += TARGET_DISTANCE;
                break;
            case Enemy.TARGET_TYPE.EAST:
                pos.x += TARGET_DISTANCE;
                break;
            case Enemy.TARGET_TYPE.SOUTH_EAST:
                pos.x += TARGET_DISTANCE;
                pos.y -= TARGET_DISTANCE;
                break;
            case Enemy.TARGET_TYPE.SOUTH:
                pos.y -= TARGET_DISTANCE;
                break;
            case Enemy.TARGET_TYPE.SOUTH_WEST:
                pos.x -= TARGET_DISTANCE;
                pos.y -= TARGET_DISTANCE;
                break;
            case Enemy.TARGET_TYPE.WEST:
                pos.x -= TARGET_DISTANCE;
                break;
            case Enemy.TARGET_TYPE.NORTH_WEST:
                pos.x -= TARGET_DISTANCE;
                pos.y += TARGET_DISTANCE;
                break;
		}
		// 打ち出す方向の設定
		Vector3 vec = pos - obj.transform.position;
		obj.GetComponent< Enemy >( ).setDir( vec );
		if ( obj.GetComponent< Enemy >( ).getObjType( ) == Enemy.OBJECT_TYPE.FAST_MIDDLE ) {
			obj.GetComponent< Rigidbody >( ).AddForce( obj.GetComponent<Enemy>( ).getDir( ) * obj.GetComponent< Enemy >( ).getSpeed( ) );
			obj.GetComponent< Enemy >( ).started( );
		}
        enemy_cube_data.obj = obj;

		_enemy_list.Add( enemy_cube_data );
	}

	public void move( ) {
		for ( int i = 0; i < _alliy_list.Count; i++ ) {
			float distance = Vector3.Distance( _alliy_list[ i ].obj.transform.position, _alliy_list[ i ].target );
			float rotation = Quaternion.Angle( _alliy_list[ i ].obj.transform.rotation, _alliy_list[ i ].targetAngle );
			// 誤差修正
			if ( distance < 0.1f ) {
				_alliy_list[ i ].obj.transform.position = _alliy_list[ i ].target;
				_alliy_list[ i ].obj.transform.rotation = _alliy_list[ i ].targetAngle;
				_alliy_list[ i ].ok_move = false;
				_alliy_list[ i ].obj.GetComponentInParent< Rigidbody >( ).velocity = Vector3.zero;
			}
		}
	}

    public void moveStart( int num ) {
        _alliy_list[ num ].obj.GetComponentInParent< Rigidbody >( ).velocity = _alliy_list[ num ].move;
    }

    public void speedDown( ) {
        for ( int i = 0; i< CUBE_NUM; i++ ) {
            _alliy_list[ i ].obj.GetComponentInParent< Rigidbody >( ).velocity = Vector3.zero;
        }
    }

    public void setTarget( int num, Vector3 pos, Quaternion angle, float time ) {
		//time = 5.0f;
		// ターゲット設定
		_alliy_list[ num ].target = pos;
		_alliy_list[ num ].targetAngle = angle;
		// 距離を測る
		float distance = Vector3.Distance( _alliy_list[ num ].obj.transform.position, _alliy_list[ num ].target );
		_alliy_list[ num ].move_distance = distance / time;
		// ベクトル
		_alliy_list[ num ].move = _alliy_list[ num ].target - _alliy_list[ num ].obj.transform.position;
		// 一フレームに移動する距離
		_alliy_list[ num ].move = _alliy_list[ num ].move.normalized * _alliy_list[ num ].move_distance;
        _alliy_list[ num ].ok_move = true;
	}

    public void setWalkAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        if ( _alliy_list[ num ].walk_flag ) {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "WalkAnimLeft" );
            _alliy_list[ num ].walk_flag = false;
        }
        else {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "WalkAnimRight" );
            _alliy_list[ num ].walk_flag = true;
        }
    }

    public void setBackAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        if ( _alliy_list[ num ].walk_flag ) {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "BackAnimLeft" );
            _alliy_list[ num ].walk_flag = false;
        }
        else {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "BackAnimRight" );
            _alliy_list[ num ].walk_flag = true;
        }
    }
	
    public void setFootAnim( int num, float base_frame, float frame, AnimationClip animationclip ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Foot" );
        
    }

    public void setLineAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        if( _alliy_list[ num ].line_flag ) {
		    _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "LineFoot1" );
            _alliy_list[ num ].line_flag = false;
        } else {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "LineFoot2" );
            _alliy_list[ num ].line_flag = true;
        }
    }

    public void setJumpAnim(int num, float base_frame, float frame, AnimationClip animationclip ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animationclip.length * base_frame ) / frame;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Jump" );
    }

    public void setGroup( int cube_num, int group_num, int member_num, GameObject parent ) {
        _alliy_list[ cube_num ].group_num  = group_num;
        _alliy_list[ cube_num ].member_num = member_num;
		_alliy_list[ cube_num ].obj.transform.parent = parent.transform;
    }

	public int cubesNum( ) {
		return _alliy_list.Count;
	}

    public int getGroupNum( int num ) {
        return _alliy_list[ num ].group_num;
    }
    
    public int getMemberNum( int num ) {
        return _alliy_list[ num ].member_num;
    }

    public void alliyAdd( JointAnchor_cube cube, RhythmManager.RHYTHM_TAG tag ) {
        ALLIY_CUBE alliy = new ALLIY_CUBE( );

        alliy.obj = cube;
        alliy.tag = tag;

        _alliy_list.Add( alliy );
    }
}
