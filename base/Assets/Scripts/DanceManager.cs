using UnityEngine;
using System.Collections;
using Common;

public class DanceManager : MonoBehaviour {

	const float DANCE_FOUR_TIMES_RACQUET = 92.0f;   // ダンス１の４拍子フレーム数
	const float DANCE_TWO_TIMES_RACQUET  = 46.0f;    // ダンス１の2拍子フレーム数
	const float DANCE_RACQUET            = 23.0f;    // ダンス１の１拍子フレーム数
    
    const float BASE_FRAME = 60.0f;

    enum DANCE_TYPE { 
        DANCE_NONE,
        DANCE_ONE,
		DANCE_TWO,
    };

    enum DANCE_ONE_MODE {
        DANCE_ONE_MODE_NONE,
        DANCE_ONE_MODE_FOOT_DANCE,
        DANCE_ONE_MODE_LINE_DANCE,
        DANCE_ONE_MODE_TWO_WALK,
        DANCE_ONE_MODE_TWO_BACK,
        DANCE_ONE_MODE_THREE_WALK,
        DANCE_ONE_MODE_THREE_BACK,
    };

	enum DANCE_TWO_MODE {
        DANCE_TWO_MODE_NONE,
		DANCE_TWO_MODE_EXCHANGE_1,
		DANCE_TWO_MODE_EXCHANGE_2,
		DANCE_TWO_MODE_JUMP,
	};

    DANCE_ONE_MODE _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_NONE;
	DANCE_TWO_MODE _dance_two_mode = DANCE_TWO_MODE.DANCE_TWO_MODE_NONE;
    DANCE_TYPE _dance_type = DANCE_TYPE.DANCE_NONE;

    const int GROUP_NUM = 3;	// グループ数
    const int ANIMATION_NUM = 5; //アニメーション数
    public const float DANCE_MOVE_DIS = 2.0f;

    public const int MAX_DANCE_ONE_COUNT = 8;   // ダンス１の振り付け数
    public const int MAX_DANCE_TWO_COUNT = 4;   // ダンス１の振り付け数

    [ SerializeField ]
	private CubeManager _cube_manager;
    [ SerializeField ]
    private RhythmManager _rhythm_manager;
    [ SerializeField ]
    private ModeManager _mode_manager;

	[ SerializeField ]
	private Group[ ] _group = new Group[ GROUP_NUM ];
    [ SerializeField ]
    private Transform _player;
    [ SerializeField ]
    private AnimationClip[ ] animtionclip = new AnimationClip[ ANIMATION_NUM ];
	[ SerializeField ]
	private GameObject target;

	int _dance_count = 0;

	Transform parent ;

	bool _anim_play = false;
	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
        // 集合処理
		if ( Input.GetKeyDown( KeyCode.F5 ) ||
             ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) &&
              _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.A_PART_FINISH ) ) {
			getherCube( );
		}
        
        // 散らばる処理
		if ( Input.GetKeyDown( KeyCode.F6 ) ||
             ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) &&
               _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.SABI_FINISH ) ) {
			scatteredCube( );
		}

        // 通常モード
        if ( Input.GetKeyDown( KeyCode.F2 ) ) {
            _dance_type = DANCE_TYPE.DANCE_NONE;
        }
        // ダンス１モード
        if ( Input.GetKeyDown( KeyCode.F3 ) ||
			( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) &&
              _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.SABI_START )) {
            _dance_type = DANCE_TYPE.DANCE_ONE;
        }
        // ダンス１モード
        if ( Input.GetKeyDown( KeyCode.F4 ) ) {
            _dance_type = DANCE_TYPE.DANCE_TWO;
			_dance_count = 0;
        }
        
        _cube_manager.move( );

        switch ( _dance_type ) {
            case DANCE_TYPE.DANCE_ONE:
                danceOne( );
                break;
            case DANCE_TYPE.DANCE_TWO:
                danceTwo( );
                break;
        }

	}

	void getherCube( ) {
		
		int count = 0;
		int pos_num = 0;
		for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
			// 集まるposを取得
			float time = _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN );
            time = 5.0f;
			_cube_manager.setTarget( i, _group[ count ].getMemberPos( pos_num ).position, _group[ count ].getMemberPos( pos_num ).localRotation, time );
            // グループ番号メンバー番号を登録
            _cube_manager.setGroup( i, count, pos_num, _group[ count ].gameObject );
            _cube_manager.moveStart( i );
			pos_num++;
			if ( pos_num >= 5 ) {
				count++;
				pos_num = 0;
			}
		}
	}

    void scatteredCube( ) {

    }

    private void danceOne( ) { 
		if ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MAIN ) ) {
			// ダンス処理( 4拍子 )
			if ( Input.GetMouseButtonDown( 0 ) || _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MAIN ) % 4 == 0 ) {
				_dance_count++;
				for ( int i = 0; i < GROUP_NUM; i++ ) {
					danceOneModeChange( i );
				}

				if ( _dance_count >= MAX_DANCE_ONE_COUNT ) {
					_dance_count = 0;
				}
			}
			// アニメーション処理
			for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
				switch ( _dance_one_mode ) {
					case DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_WALK:
						danceOneWalkAnim( i, _dance_one_mode );
						break;
					case DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_BACK:
						danceOneBackAnim( i, _dance_one_mode );
						break;
					case DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_WALK:
						danceOneWalkAnim( i, _dance_one_mode );
						break;
					case DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_BACK:
						danceOneBackAnim( i, _dance_one_mode );
						break;
				}
			}
        
			// アニメーション処理
			if ( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MAIN ) % 2 == 0 ) {
				for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					switch ( _dance_one_mode ) {
						case DANCE_ONE_MODE.DANCE_ONE_MODE_FOOT_DANCE:
							_cube_manager.setFootAnim( i, BASE_FRAME, DANCE_TWO_TIMES_RACQUET, animtionclip[ 2 ] );
							break; 
						case DANCE_ONE_MODE.DANCE_ONE_MODE_LINE_DANCE:
							_cube_manager.setLineAnim( i, BASE_FRAME, DANCE_TWO_TIMES_RACQUET, animtionclip[ 3 ] );
							break;
					}
				}
			}
		}
		move( );
    }

    private void danceOneModeChange( int group_num ) {
        Vector3[ ] pos = new Vector3[ 5 ];
        for ( int i = 0; i < 5; i++ ) {
            pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
        }
		switch ( _dance_count ) {
			case 1:
                pos[ 1 ].z -= DANCE_MOVE_DIS;
                pos[ 2 ].z -= DANCE_MOVE_DIS;
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_WALK;
				break;
            case 2:
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_FOOT_DANCE;
				break;
            case 3:
                pos[ 1 ].z += DANCE_MOVE_DIS;
                pos[ 2 ].z += DANCE_MOVE_DIS;
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_BACK;
				break;
            case 4:
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_LINE_DANCE;
				break;
			case 5:
                pos[ 0 ].z -= DANCE_MOVE_DIS;
                pos[ 3 ].z -= DANCE_MOVE_DIS;
                pos[ 4 ].z -= DANCE_MOVE_DIS;
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_WALK;
				break;
            case 6:
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_FOOT_DANCE;
                break;
            case 7:
                pos[ 0 ].z += DANCE_MOVE_DIS;
                pos[ 3 ].z += DANCE_MOVE_DIS;
                pos[ 4 ].z += DANCE_MOVE_DIS;
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_BACK;
				break;
            case 8:
                _dance_one_mode = DANCE_ONE_MODE.DANCE_ONE_MODE_LINE_DANCE;
                break;
		}
        
        for ( int i = 0; i < 5; i++ ) {
            _group[ group_num ].setMemberPos( i, pos[ i ] );
        }

        for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
            // グループ比較
            if ( _cube_manager.getGroupNum( i ) == group_num ) {
				int center_num     = 0;
                int right_num      = 1;
                int left_num       = 2;
				int right_edge_num = 3;
				int left_edge_num  = 4;

                float time =  _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 5 / 3 / BASE_FRAME;

                // ポジション比較し、合致すればターゲットをセット
                if ( _cube_manager.getMemberNum( i ) == center_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( center_num ).position, _group[ group_num ].getMemberPos( center_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == right_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( right_num ).position, _group[ group_num ].getMemberPos( right_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == left_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( left_num ).position, _group[ group_num ].getMemberPos( left_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == right_edge_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( right_edge_num ).position, _group[ group_num ].getMemberPos( right_edge_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == left_edge_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( left_edge_num ).position, _group[ group_num ].getMemberPos( left_edge_num ).localRotation, time );
                }
            }
        }
	}

    private void danceOneWalkAnim( int i, DANCE_ONE_MODE mode ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす場合
                if ( mode == DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_WALK &&
                     ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) ) {
                    _cube_manager.setWalkAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 0 ] );
                }
                // 3個動かす場合
                else if ( mode == DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_WALK &&
                     ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) ) {
                    _cube_manager.setWalkAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 0 ] );
                }
            }
        }
    }

    private void danceOneBackAnim( int i, DANCE_ONE_MODE mode ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす場合
                if ( mode == DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_BACK &&
                     ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) ) {
                    _cube_manager.setBackAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 1 ] );
                }
                // 3個動かす場合
                else if ( mode == DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_BACK &&
                     ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) ) {
                    _cube_manager.setBackAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 1 ] );
                }
            }
        }
    }

	private void danceTwo( ) { 
		// ダンス処理( 4拍子 )
		if ( Input.GetMouseButtonDown( 0 ) || _rhythm_manager.getFrame( ) % DANCE_FOUR_TIMES_RACQUET == 0 ) {
			_dance_count++;
			for ( int i = 0; i < GROUP_NUM; i++ ) {
				danceTwoModeChange( i );
			}

            if ( _dance_count >= MAX_DANCE_TWO_COUNT ) {
                _dance_count = 0;
            }
		}

        // ウォークアニメーション処理
		if ( _rhythm_manager.getFrame( ) % DANCE_RACQUET == 0 ) {
            for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
                switch ( _dance_two_mode ) {
                    case DANCE_TWO_MODE.DANCE_TWO_MODE_EXCHANGE_1:
                        danceTwoExchangeAnim1( i, _dance_two_mode );
                        break;
                    case DANCE_TWO_MODE.DANCE_TWO_MODE_EXCHANGE_2:
                        danceTwoExchangeAnim2( i, _dance_two_mode );
                        break;
                }
			}
		}
        
        // ジャンプアニメーション処理
		if ( _dance_two_mode == DANCE_TWO_MODE.DANCE_TWO_MODE_JUMP ) {
			if ( _rhythm_manager.getFrame( ) % DANCE_FOUR_TIMES_RACQUET == 0 ||
				 _rhythm_manager.getFrame( ) % DANCE_FOUR_TIMES_RACQUET == DANCE_RACQUET * 2 ) {
				for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 4 ] );
					}
				}
			} else if ( _rhythm_manager.getFrame( ) % DANCE_FOUR_TIMES_RACQUET == DANCE_RACQUET ||
				 _rhythm_manager.getFrame( ) % DANCE_FOUR_TIMES_RACQUET == DANCE_RACQUET * 3 ) {
				for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 4 ] );
					}
				}
			}
		}
		move( );
    }

	private void danceTwoModeChange( int group_num ) {
        Vector3[ ] pos = new Vector3[ 5 ];
        for ( int i = 0; i < 5; i++ ) {
            pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
        }
		switch ( _dance_count ) {
			case 0:
                pos[ 1 ].z += DANCE_MOVE_DIS;
                pos[ 2 ].z += DANCE_MOVE_DIS;
				
                pos[ 0 ].z -= DANCE_MOVE_DIS;
                pos[ 3 ].z -= DANCE_MOVE_DIS;
                pos[ 4 ].z -= DANCE_MOVE_DIS;
                _dance_two_mode = DANCE_TWO_MODE.DANCE_TWO_MODE_EXCHANGE_1;
				break;
			case 1:
                pos[ 1 ].z += DANCE_MOVE_DIS;
                pos[ 2 ].z += DANCE_MOVE_DIS;
				
                pos[ 0 ].z -= DANCE_MOVE_DIS;
                pos[ 3 ].z -= DANCE_MOVE_DIS;
                pos[ 4 ].z -= DANCE_MOVE_DIS;
                _dance_two_mode = DANCE_TWO_MODE.DANCE_TWO_MODE_EXCHANGE_1;
				break;
            case 2:
                _dance_two_mode = DANCE_TWO_MODE.DANCE_TWO_MODE_JUMP;
				break;
            case 3:
                pos[ 1 ].z -= DANCE_MOVE_DIS;
                pos[ 2 ].z -= DANCE_MOVE_DIS;
				
                pos[ 0 ].z += DANCE_MOVE_DIS;
                pos[ 3 ].z += DANCE_MOVE_DIS;
                pos[ 4 ].z += DANCE_MOVE_DIS;
                _dance_two_mode = DANCE_TWO_MODE.DANCE_TWO_MODE_EXCHANGE_2;
				break;
            case 4:
                _dance_two_mode = DANCE_TWO_MODE.DANCE_TWO_MODE_JUMP;
				break;
		}
        
        for ( int i = 0; i < 5; i++ ) {
            _group[ group_num ].setMemberPos( i, pos[ i ] );
        }

        for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
            // グループ比較
            if ( _cube_manager.getGroupNum( i ) == group_num ) {
				int center_num     = 0;
                int right_num      = 1;
                int left_num       = 2;
				int right_edge_num = 3;
				int left_edge_num  = 4;

                float time = _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 5.0f / 3.0f / BASE_FRAME;

                // ポジション比較し、合致すればターゲットをセット
                if ( _cube_manager.getMemberNum( i ) == center_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( center_num ).position, _group[ group_num ].getMemberPos( center_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == right_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( right_num ).position, _group[ group_num ].getMemberPos( right_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == left_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( left_num ).position, _group[ group_num ].getMemberPos( left_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == right_edge_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( right_edge_num ).position, _group[ group_num ].getMemberPos( right_edge_num ).localRotation, time );
                } else if ( _cube_manager.getMemberNum( i ) == left_edge_num ) {
                    _cube_manager.setTarget( i, _group[ group_num ].getMemberPos( left_edge_num ).position, _group[ group_num ].getMemberPos( left_edge_num ).localRotation, time );
                }
            }
        }
	}

	private void danceTwoExchangeAnim1( int i, DANCE_TWO_MODE mode ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす方
                if ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) {
                    _cube_manager.setBackAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 1 ] );
                }
                // 3個動かす場方
                else if ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) {
                    _cube_manager.setWalkAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 0 ] );
                }
            }
        }
    }

    private void danceTwoExchangeAnim2( int i, DANCE_TWO_MODE mode ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす方
                if ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) {
                    _cube_manager.setWalkAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 0 ] );
                }
                // 3個動かす場方
                else if ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) {
                    _cube_manager.setBackAnim( i, BASE_FRAME, DANCE_RACQUET, animtionclip[ 1 ] );
                }
            }
        }
    }

    private void addForce( int i ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす場合
                if ( ( _dance_one_mode == DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_WALK ||
                       _dance_one_mode == DANCE_ONE_MODE.DANCE_ONE_MODE_TWO_BACK ) &&
                     ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) ) {
                    _cube_manager.moveStart( i );
                }
                // 3個動かす場合
                else if ( ( _dance_one_mode == DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_WALK ||
                            _dance_one_mode == DANCE_ONE_MODE.DANCE_ONE_MODE_THREE_BACK ) &&
                     ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) ) {
                    _cube_manager.moveStart( i );
                }
                // パターン2のmove
                else if ( _dance_two_mode == DANCE_TWO_MODE.DANCE_TWO_MODE_EXCHANGE_1 ||
                            _dance_two_mode == DANCE_TWO_MODE.DANCE_TWO_MODE_EXCHANGE_2 ) {
                    _cube_manager.moveStart( i );
                }
            }
        }

    }

	private void move( ){
		//加速処理
		if ( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MAIN ) % 1 == 0 ) {
            for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
                addForce( i );
			}
        }
		//減速処理
        if ( _rhythm_manager.getFrame( ) % _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN )
			== (int)( _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * ( 2.0f / 3.0f ) ) &&
             _dance_type != DANCE_TYPE.DANCE_NONE ) {
            for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
                _cube_manager.speedDown( );
			}
		}

	}
}
