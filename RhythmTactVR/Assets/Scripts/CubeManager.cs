using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class CubeManager : MonoBehaviour {

    const int CUBE_NUM = 15;
    const float TARGET_DISTANCE = 0.5f;
    const float DANCE_FOUR_TIMES_RACQUET = 80.0f;
	const float SCATTERED_SPEED = 2.0f;

    /// <summary>
    /// 敵キューブの構造体
    /// </summary>
	private class ENEMY_CUBE {
        public GameObject obj;
        public RhythmManager.RHYTHM_TAG tag;
    };
    
	private class ALLIY_CUBE {
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
        public GROUP_TYPE group_type;

		public ALLIY_CUBE( )  {
			this.group_num = 4;
		}
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

    public Color _enemy_color;
	public Color _enemy_hit_color;


    // 敵cube
    private List< ENEMY_CUBE > _enemy_list = new List< ENEMY_CUBE >( );
    // 味方cube
    private List< ALLIY_CUBE > _alliy_list = new List< ALLIY_CUBE >( );

    int _create_count_main       = 0;       // 打ち出すエネミーの番号
    int _comming_main_rhyrhm_num = 0;       // 仮のリズム番号

    void Awake( ) {
		if ( !_enemy_prefab ) {
			_enemy_prefab = ( GameObject )Resources.Load( "Prefabs/vr_01_fix_02" );
		}
	}

	// Use this for initialization
	void Start( ) {

	}

	void FixedUpdate( ) {
        if ( isError( ) ) {
            return;
        }
        // 敵キューブの挙動
        enemyUpdate( );
	}

    bool isError( ) {
        bool error = false;

        if ( !_file_manager ) {
            try {
                error = true;
                _file_manager = FileManager.getInstance( );
            } catch {
                Debug.LogError( "ファイルマネージャーのインスタンスが取得できませんでした。" );
            }
        }

        return error;
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
        Enemy obj_enemy = _enemy_list[ num ].obj.GetComponent< Enemy >( );
		
		_enemy_list[ num ].obj.transform.position = new Vector3( pos.x + obj_enemy.getDir( ).x * enemy.speed,
														pos.y + obj_enemy.getDir( ).y * enemy.speed,
														pos.z + obj_enemy.getDir( ).z * enemy.speed );
	}

    public void enemyCreate( int count, RhythmManager.RHYTHM_TAG tag ) {
        ENEMY_CUBE enemy_cube_data = new ENEMY_CUBE( );

        enemy_cube_data.tag = tag;
		// 値の設定
		GameObject obj = ( GameObject )Instantiate( _enemy_prefab, _file_manager.getRhythmForNum( count, tag ).create_pos, Quaternion.identity );
        Enemy obj_enemy = obj.GetComponent< Enemy >( );
		obj_enemy.setObjType( _file_manager.getRhythmForNum( count, tag ).obj_type );
		obj_enemy.setSpeed( _file_manager.getRhythmForNum( count, tag ).speed );
		obj_enemy.setTargetType( _file_manager.getRhythmForNum( count, tag ).target_type );
        obj_enemy.setNeonColor( _enemy_color );
        obj_enemy.setNeonHitColor( _enemy_hit_color );

        // マネージャーの配下に設定
        obj.transform.parent = transform;

		// ターゲットの設定
		Vector3 pos = _target.position;
        switch ( obj_enemy.getTargetType( ) ) {
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
		obj_enemy.setDir( vec );
		if ( obj_enemy.getObjType( ) == Enemy.OBJECT_TYPE.FAST_MIDDLE ) {
			obj.GetComponent< Rigidbody >( ).AddForce( obj.GetComponent<Enemy>( ).getDir( ) * obj_enemy.getSpeed( ) );
			obj_enemy.started( );
		}
        enemy_cube_data.obj = obj;

		_enemy_list.Add( enemy_cube_data );
	}

	public void move( ) {
		for ( int i = 0; i < _alliy_list.Count; i++ ) {
			float distance = Vector3.Distance( _alliy_list[ i ].obj.transform.position, _alliy_list[ i ].target );
			// 誤差修正
			if ( distance < 0.1f ) {
				_alliy_list[ i ].obj.transform.position = _alliy_list[ i ].target;
				_alliy_list[ i ].ok_move = false;
				_alliy_list[ i ].obj.GetComponentInParent< Rigidbody >( ).velocity = Vector3.zero;
			}
		}
	}

    public void moveStart( int num ) {
        _alliy_list[ num ].obj.GetComponentInParent< Rigidbody >( ).velocity = _alliy_list[ num ].move;
    }

	public void scatteredStart( int num ) {
        _alliy_list[ num ].obj.GetComponentInParent< Rigidbody >( ).velocity = _alliy_list[ num ].move * SCATTERED_SPEED;
    }

    public void speedDown( ) {
        for ( int i = 0; i< CUBE_NUM; i++ ) {
            _alliy_list[ i ].obj.GetComponentInParent< Rigidbody >( ).velocity = Vector3.zero;
        }
    }

    public void setTarget( int num, Vector3 pos, float time ) {
		//time = 5.0f;
		// ターゲット設定
		_alliy_list[ num ].target = pos;
		// 距離を測る
		float distance = Vector3.Distance( _alliy_list[ num ].obj.transform.position, _alliy_list[ num ].target );
		_alliy_list[ num ].move_distance = distance / time;
		// ベクトル
		_alliy_list[ num ].move = _alliy_list[ num ].target - _alliy_list[ num ].obj.transform.position;
		// 一フレームに移動する距離
		_alliy_list[ num ].move = _alliy_list[ num ].move.normalized * _alliy_list[ num ].move_distance;
        _alliy_list[ num ].ok_move = true;
	}

	public void resetRotate( int num ) {
		_alliy_list[ num ].obj.gameObject.transform.rotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );
	}

	/// <summary>
	/// 足踏み
	/// </summary>
    public void setFootAnim( int num, float base_frame, float frame, float animlength ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Foot", 0, 0.0f );
    }

	/// <summary>
	/// 歩き前進
	/// </summary>
    public void setWalkAnim(int num, float base_frame, float frame, float animlength ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        if ( _alliy_list[ num ].walk_flag ) {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "WalkAnimLeft", 0, 0.0f );
            _alliy_list[ num ].walk_flag = false;
        }
        else {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "WalkAnimRight", 0, 0.0f );
            _alliy_list[ num ].walk_flag = true;
        }

    }

	/// <summary>
	/// 歩き後進
	/// </summary>
    public void setBackAnim(int num, float base_frame, float frame, float animlength ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        if ( _alliy_list[ num ].walk_flag ) {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "BackAnimLeft", 0, 0.0f );
            _alliy_list[ num ].walk_flag = false;
        }
        else {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "BackAnimRight", 0, 0.0f );
            _alliy_list[ num ].walk_flag = true;
        }

    }

	/// <summary>
	/// ラインダンス
	/// </summary>
    public void setLineAnim(int num, float base_frame, float frame, float animlength ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        if( _alliy_list[ num ].line_flag ) {
		    _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "LineFootLeft", 0, 0.0f );
            _alliy_list[ num ].line_flag = false;
        } else {
            _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "LineFootRight", 0, 0.0f );
            _alliy_list[ num ].line_flag = true;
        }

    }

	/// <summary>
	/// ジャンプ
	/// </summary>
    public void setJumpAnim(int num, float base_frame, float frame, float animlength ) {
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.8f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Jump", 0, 0.0f );

    }

	/// <summary>
	/// 鼓動
	/// </summary>
	public void setBeat(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Beat", 0, 0.0f );

	}

	/// <summary>
	/// 早い鼓動
	/// </summary>
	public void setFastBeat(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "FastBeat", 0, 0.0f );

	}

	/// <summary>
	/// 転がり前進
	/// </summary>
	public void setRollAdvance(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "RollAdvance", 0, 0.0f );

	}

	/// <summary>
	/// 転がり後進
	/// </summary>
	public void setRollBack(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "RollBack", 0, 0.0f );

	}

	/// <summary>
	/// 転がり左進
	/// </summary>
	public void setRollLeft(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "RollLeft", 0, 0.0f );

	}

	/// <summary>
	/// 転がり右進
	/// </summary>
	public void setRollRight(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "RollRight", 0, 0.0f );

	}

	/// <summary>
	/// 傾け左
	/// </summary>
	public void setCantLeft(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "CantLeft", 0, 0.0f );

	}

	/// <summary>
	/// 傾け右
	/// </summary>
	public void setCantRight(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "CantRight", 0, 0.0f );

	}

	/// <summary>
	/// 左回転ジャンプ
	/// </summary>
	public void setSpinJumpLeft(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "SpinJumpLeft", 0, 0.0f );

	}

	/// <summary>
	/// 右回転ジャンプ
	/// </summary>
	public void setSpinJumpRight(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "SpinJumpRight", 0, 0.0f );

	}

    /// <summary>
	/// 超speed!?鼓動
	/// </summary>
	public void setFasterBeat(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "FasterBeat", 0, 0.0f );

	}

    /// <summary>
	/// 競歩
	/// </summary>
	public void setFastWalk(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "FastWalkAnim", 0, 0.0f );

	}

	/// <summary>
	/// 打ち出し
	/// </summary>
	public void setExtendDeparture(int num, float base_frame, float frame, float animlength ) {
		_alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).speed = ( animlength * 2 * base_frame ) / frame * 1.5f;
        _alliy_list[ num ].obj.GetComponentInChildren< Animator >( ).Play( "Extend_Departure", 0, 0.0f );

		
	}

    public void setGroup( int cube_num, int group_num, int member_num, GameObject parent, GROUP_TYPE group_type ) {
        _alliy_list[ cube_num ].group_num  = group_num;
        _alliy_list[ cube_num ].member_num = member_num;
		_alliy_list[ cube_num ].obj.transform.parent = parent.transform;
    }

	public void setNoneGroup( int cube_num, int group_num, int member_num ) {
        _alliy_list[ cube_num ].group_num  = group_num;
        _alliy_list[ cube_num ].member_num = member_num;
		_alliy_list[ cube_num ].group_type = GROUP_TYPE.GROUP_NONE;
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

    public GameObject getAlliyCube( int num ) {
        return _alliy_list[ num ].obj.gameObject;
    }

    public GROUP_TYPE getGroupType( int num ) {
        return _alliy_list[ num ].group_type;
    }
}
