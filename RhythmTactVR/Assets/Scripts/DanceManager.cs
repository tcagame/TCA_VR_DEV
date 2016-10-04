using UnityEngine;
using System.Collections;
using Common;
using System.Collections.Generic;

public class DanceManager : MonoBehaviour {
    
    public const float BASE_FRAME = 60.0f;

	[ SerializeField ]
    private DANCE_PART _dance_part;

    const int GROUP_NUM = 3;	// グループ数
    const int GROUP_CUBE_NUM = 15;

    const int ANIMATION_NUM = 5; //アニメーション数
    public const float DANCE_MOVE_DIS = 0.3f;        // ダンス中に動く距離

	const float LAST_CIRCLE_DIS = 4.0f;
    
    [ SerializeField ]
    private FileManager _file_manager;

    [ SerializeField ]
	private CubeManager _cube_manager;
    [ SerializeField ]
    private RhythmManager _rhythm_manager;
    [ SerializeField ]
    private ModeManager _mode_manager;
    [ SerializeField ]
    private GameObject _player;
    [ SerializeField ]
    private GameObject _last_target;

	[ SerializeField ]
	private Group[ ] _group = new Group[ GROUP_NUM ];
    private float _anim_length = 2.0f;
	bool _flag = false;

	bool _last_circle_start = false;
	int _last_count = 0;

    void Awake( ) {
        _dance_part = DANCE_PART.NO_DANCE_PART;
    }

	// Use this for initialization
	void Start( ) {
        _dance_part = DANCE_PART.NO_DANCE_PART;
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
        // ダンスパート変更処理
        dancePartChange( );

        // 集合処理
		if ( Input.GetKeyDown( KeyCode.F1 ) ||
             ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) &&
               _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.A_PART_FINISH ) ) {
			getherCube( );
		}

        _cube_manager.move( );

        // さびのときメインリズムに合わせたダンスを行う　・　ＥＤＭのときサブリズムに合わせたダンスを行う
        if( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MAIN ) && _dance_part != DANCE_PART.NO_DANCE_PART && _mode_manager.getMusicMode( ) != MUSIC_MODE.MODE_C_PART ) {
			for ( int i = 0; i < _group.Length; i++ ) {
				if ( _group[ i ].isFinishDancePart( ) == false ) {
					dancePartUpdate( i );
				}
			}
		}

		if ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) && _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.SABI_FINISH ) {
			finalGather( );
		}


	}

    void dancePartChange( ) {
        if ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.GROUP_ANIM ) ) {
            switch( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.GROUP_ANIM ) ) {
				case 0:
					_dance_part = DANCE_PART.PART_ONE;
					break;
                case 1:
                    _dance_part = DANCE_PART.PART_TWO;
                    break;
                case 2:
                    _dance_part = DANCE_PART.PART_THREE;
                    break;
                case 3:
                    _dance_part = DANCE_PART.PART_FOUR;
                    break;
                case 4:
                    _dance_part = DANCE_PART.PART_FIVE;
                    break;
                case 5:
                    _dance_part = DANCE_PART.PART_SIX;
                    break;
            }
					
			for ( int i = 0; i < _group.Length; i++ ) {
				_group[ i ].setDanceFinish( true );
				_group[ i ].resetPartCount( );
			}
        }
    }

	void getherCube( ) {
		int count = 1;
		int pos_num = 0;

        // 最初の５個を真ん中のグループに設定
        for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
            if( _cube_manager.cubesNum( ) > i ) {
			    // 集まるposを取得
			    float time = _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MODE_CHANGE );
                time = 5.0f;    // デバッグタイム
			    _cube_manager.setTarget( i, _group[ 0 ].getMemberPos( pos_num ).position, time );
                // グループタイプ・グループ番号・メンバー番号を登録
                _group[ 0 ].setGroupType( GROUP_TYPE.GROUP_A );
				_cube_manager.resetRotate( i );
					_cube_manager.setGroup( i, 0, pos_num, _group[ 0 ].gameObject, _group[ 0 ].getGroupType( ) );
                _group[ 0 ].setMember( i, _cube_manager.getAlliyCube( i ) );
                _cube_manager.moveStart( i );

                pos_num++;
            }
        }

        pos_num = 0;

		_group[ 1 ].setGroupType( GROUP_TYPE.GROUP_B );
		_group[ 2 ].setGroupType( GROUP_TYPE.GROUP_C );

        // 残りの味方キューブを一個ずつ順に登録していく
        if ( _cube_manager.cubesNum( ) > 5 ) {
		    for ( int i = Group.MEMBER_NUM; i < GROUP_CUBE_NUM; i++ ) {			    // 集まるposを取得
				if ( i < _cube_manager.cubesNum( ) ) {
					float time = _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MODE_CHANGE );
					time = 5.0f;    // デバッグタイム
					_cube_manager.setTarget( i, _group[ count ].getMemberPos( pos_num ).position, time );
					// グループタイプ・グループ番号・メンバー番号を登録
					_cube_manager.resetRotate( i );
					_cube_manager.setGroup( i, count, pos_num, _group[ count ].gameObject, _group[ count ].getGroupType( ) );
					_group[ count ].setMember( pos_num, _cube_manager.getAlliyCube( i ) );
					_cube_manager.moveStart( i );
					count++;
					if ( count >= GROUP_NUM ) {
						pos_num++;
						count = 1;
					}
				}
            }
		}

        // あまりキューブをバックダンサーとして登録
        if ( _cube_manager.cubesNum( ) > GROUP_CUBE_NUM ) {
		    for ( int i = GROUP_CUBE_NUM; i < _cube_manager.cubesNum( ); i++ ) {
			    // 集まるposを取得
			    float time = _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MODE_CHANGE );
                time = 5.0f;    // デバッグタイム
			    _cube_manager.setTarget( i, new Vector3( 0, 500, 0 ), time );
				_cube_manager.resetRotate( i );
				_cube_manager.setNoneGroup( i, 4, 0 );
                //_group[ count ].setMember( pos_num, _cube_manager.getAlliyCube( i ) );
                _cube_manager.moveStart( i );
            }
		}
	}

    void dancePartUpdate( int group_num ) {
        // ダンスパート比較
        if ( _dance_part == _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].dance_part ) {
            // グループタイプ比較
            switch( _group[ group_num ].getGroupType( ) ) {
                case GROUP_TYPE.GROUP_A:
                    {
                        // ダンスタイプ登録
                        if ( _group[ group_num ].isFinishDance( ) == true ) {
							int part_count = _group[ group_num ].getPartCount( );
							DANCE_TYPE dance_type;
							int dance_type_num = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_A ].dance_type.Count;

							if ( part_count >= dance_type_num ) {
								dance_type = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_A ].dance_type[ dance_type_num - 1 ];
							} else {
								dance_type = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_A ].dance_type[ part_count ];
							}

							
							if ( _group[ group_num ].getDanceType( ) == DANCE_TYPE.DANCE_TEN_ONE && dance_type == DANCE_TYPE.DANCE_NINE_THREE ) {
								int a = 0;
								Debug.Log( "a" );
							}
							/*
							// タイプが同じでかつ継続ダンスだったら継続
							if ( dance_type == _group[ i ].getDanceType( ) && ( dance_type == DANCE_TYPE.DANCE_TEN_ONE )  ) {
								_group[ i ].setDanceContinu( true );
							}
							*/ 
							// ダンスタイプが変わったらダンスカウントリセット
							_group[ group_num ].resetDanceCount( );
							if ( dance_type != DANCE_TYPE.DANCE_NONE ) {
								_group[ group_num ].setDanceType( dance_type );
							} else if ( dance_type == DANCE_TYPE.DANCE_NONE ) {
								//Debug.Log( _group[ group_num ].getPartCount( ) );
								//Debug.Log( _group[ group_num ].getDanceCount( ) );
							}
						}
                    }
                    break;
                case GROUP_TYPE.GROUP_B:
                    {
                        // ダンスタイプ登録
                        if ( _group[ group_num ].isFinishDance( ) == true ) {
							int part_count = _group[ group_num ].getPartCount( );
							DANCE_TYPE dance_type;
							int dance_type_num = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_B ].dance_type.Count;

							if ( part_count >= dance_type_num ) {
								dance_type = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_B ].dance_type[ dance_type_num - 1 ];
							} else {
								dance_type = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_B ].dance_type[ part_count ];
							}
							
							if ( _group[ group_num ].getDanceType( ) == DANCE_TYPE.DANCE_TEN_ONE && dance_type == DANCE_TYPE.DANCE_NINE_THREE ) {
								int a = 0;
								Debug.Log( "b" );
							}
							/*
							// タイプが同じでかつ継続ダンスだったら継続
							if ( dance_type == _group[ i ].getDanceType( ) && ( dance_type == DANCE_TYPE.DANCE_TEN_ONE )  ) {
								_group[ i ].setDanceContinu( true );
							}
							*/ 
							// ダンスタイプが変わったらダンスカウントリセット
                            _group[ group_num ].resetDanceCount( );
							if ( dance_type != DANCE_TYPE.DANCE_NONE ) {
								_group[ group_num ].setDanceType( dance_type );
							} else if ( dance_type == DANCE_TYPE.DANCE_NONE ) {
								//Debug.Log( _group[ group_num ].getPartCount( ) );
								//Debug.Log( _group[ group_num ].getDanceCount( ) );
							}
                        }
                    }
                    break;
                case GROUP_TYPE.GROUP_C:
                    {
                        // ダンスタイプ登録
                        if ( _group[ group_num ].isFinishDance( ) == true ) {
							int part_count = _group[ group_num ].getPartCount( );
							DANCE_TYPE dance_type;
							int dance_type_num = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_C ].dance_type.Count;

							if ( part_count >= dance_type_num ) {
								dance_type = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_C ].dance_type[ dance_type_num - 1 ];
							} else {
								dance_type = _file_manager.getDanceData( ).dance_part[ ( int )_dance_part - 1 ].group_data[ ( int )GROUP_TYPE.GROUP_C ].dance_type[ part_count ];
							}

							/*
							// タイプが同じでかつ継続ダンスだったら継続
							if ( dance_type == _group[ i ].getDanceType( ) && ( dance_type == DANCE_TYPE.DANCE_TEN_ONE )  ) {
								_group[ i ].setDanceContinu( true );
							}
							*/ 
							// ダンスタイプが変わったらダンスカウントリセット
                            _group[ group_num ].resetDanceCount( );
							if ( dance_type != DANCE_TYPE.DANCE_NONE ) {
								_group[ group_num ].setDanceType( dance_type );
							} else if ( dance_type == DANCE_TYPE.DANCE_NONE ) {
								//Debug.Log( _group[ group_num ].getPartCount( ) );
								//Debug.Log( _group[ group_num ].getDanceCount( ) );
							}
                        }
                    }
                    break;
            }

            dance( group_num, _group[ group_num ].getDanceType( ) );
        }
    }


    void dance( int group_num, DANCE_TYPE type ) {
        switch( type ) {
            case DANCE_TYPE.DANCE_ONE_ONE:
                danceOneOne( group_num );
                break;
            case DANCE_TYPE.DANCE_ONE_TWO:
                danceOneTwo( group_num );
                break;
            case DANCE_TYPE.DANCE_ONE_THREE:
                danceOneThree( group_num );
                break;
            case DANCE_TYPE.DANCE_TWO_ONE:
                danceTwoOne( group_num );
                break;
            case DANCE_TYPE.DANCE_THREE_ONE:
                danceThreeOne( group_num );
                break;
            case DANCE_TYPE.DANCE_THREE_TWO:
                danceThreeTwo( group_num );
                break;
            case DANCE_TYPE.DANCE_FOUR_A_ONE:
                danceFourAOne( group_num );
                break;
            case DANCE_TYPE.DANCE_FOUR_A_TWO:
                danceFourATwo( group_num );
                break;
            case DANCE_TYPE.DANCE_FOUR_B_ONE:
                danceFourBOne( group_num );
                break;
            case DANCE_TYPE.DANCE_FOUR_B_TWO:
                danceFourBTwo( group_num );
                break;
            case DANCE_TYPE.DANCE_FIVE_ONE:
                danceFiveOne( group_num );
                break;
            case DANCE_TYPE.DANCE_FIVE_TWO:
                danceFiveTwo( group_num );
                break;
            case DANCE_TYPE.DANCE_FIVE_THREE:
                danceFiveThree( group_num );
                break;
            case DANCE_TYPE.DANCE_FIVE_FOUR:
                danceFiveFour( group_num );
                break;
            case DANCE_TYPE.DANCE_SIX_ONE:
                danceSixOne( group_num );
                break;
            case DANCE_TYPE.DANCE_SEVEN_A_ONE:
                danceSevenAOne( group_num );
                break;
            case DANCE_TYPE.DANCE_SEVEN_A_TWO:
                danceSevenATwo( group_num );
                break;
            case DANCE_TYPE.DANCE_SEVEN_B_ONE:
                danceSevenBOne( group_num );
                break;
            case DANCE_TYPE.DANCE_SEVEN_B_TWO:
                danceSevenBTwo( group_num );
                break;
            case DANCE_TYPE.DANCE_EUGHT_ONE:
                danceEightOne( group_num );
                break;
            case DANCE_TYPE.DANCE_EIGHT_TWO:
                danceEightTwo( group_num );
                break;
            case DANCE_TYPE.DANCE_NINE_ONE:
                danceNineOne( group_num );
                break;
            case DANCE_TYPE.DANCE_NINE_TWO:
                danceNineTwo( group_num );
                break;
            case DANCE_TYPE.DANCE_NINE_THREE:
                danceNineThree( group_num );
                break;
            case DANCE_TYPE.DANCE_NINE_FOUR:
                danceNineFour( group_num );
                break;
            case DANCE_TYPE.DANCE_TEN_ONE:
                danceTenOne( group_num );
                break;
        }

		if ( _group[ group_num ].isFinishDancePart( ) == false ) {
			for ( int i = 0; i < GROUP_CUBE_NUM; i++ ) {
				// グループ比較
				if ( i < _cube_manager.cubesNum( ) ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						int center_num     = 0;
						int right_num      = 1;
						int left_num       = 2;
						int right_edge_num = 3;
						int left_edge_num  = 4;

						float time =  _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 5.0f / 3.0f / BASE_FRAME;

						// ポジション比較し、合致すればターゲットをセット
						if ( _cube_manager.getMemberNum( i ) == center_num ) {
							_cube_manager.setTarget( i, _group[ group_num ].getMemberPos( center_num ).position, time );
						} else if ( _cube_manager.getMemberNum( i ) == right_num ) {
							_cube_manager.setTarget( i, _group[ group_num ].getMemberPos( right_num ).position, time );
						} else if ( _cube_manager.getMemberNum( i ) == left_num ) {
							_cube_manager.setTarget( i, _group[ group_num ].getMemberPos( left_num ).position, time );
						} else if ( _cube_manager.getMemberNum( i ) == right_edge_num ) {
							_cube_manager.setTarget( i, _group[ group_num ].getMemberPos( right_edge_num ).position, time );
						} else if ( _cube_manager.getMemberNum( i ) == left_edge_num ) {
							_cube_manager.setTarget( i, _group[ group_num ].getMemberPos( left_edge_num ).position, time );
						}
					}
				}
			}
		}
		move( group_num );
    }

    void danceOneOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;

                    // 前進アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setWalkAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                // 前進アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setWalkAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 2:
                // 前進アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setWalkAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 3:
                // 前進アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setWalkAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceOneTwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                // 足踏みアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setFootAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
            case 2:
                // 足踏みアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setFootAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
				break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceOneThree( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 後退アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setBackAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                // 後退アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setBackAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 2:
                // 後退アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setBackAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 3:
                // 後退アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setBackAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceTwoOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                // ラインアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setLineAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
            case 2:
                // ラインアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setLineAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceThreeOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
            case 2:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }
    
    void danceThreeTwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 1:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
            case 3:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }
    
    void danceFourAOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x += DANCE_MOVE_DIS;
                    pos[ 1 ].x += DANCE_MOVE_DIS;
                    pos[ 2 ].x += DANCE_MOVE_DIS;
                    pos[ 3 ].x += DANCE_MOVE_DIS;
                    pos[ 4 ].x += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x += DANCE_MOVE_DIS;
                    pos[ 1 ].x += DANCE_MOVE_DIS;
                    pos[ 2 ].x += DANCE_MOVE_DIS;
                    pos[ 3 ].x += DANCE_MOVE_DIS;
                    pos[ 4 ].x += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                //ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
				break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }
    
    void danceFourATwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x -= DANCE_MOVE_DIS;
                    pos[ 1 ].x -= DANCE_MOVE_DIS;
                    pos[ 2 ].x -= DANCE_MOVE_DIS;
                    pos[ 3 ].x -= DANCE_MOVE_DIS;
                    pos[ 4 ].x -= DANCE_MOVE_DIS;
                    // 転がるアニメ( 逆回転
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x -= DANCE_MOVE_DIS;
                    pos[ 1 ].x -= DANCE_MOVE_DIS;
                    pos[ 2 ].x -= DANCE_MOVE_DIS;
                    pos[ 3 ].x -= DANCE_MOVE_DIS;
                    pos[ 4 ].x -= DANCE_MOVE_DIS;
                    // 転がるアニメ( 逆回転
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                //ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceFourBOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
             case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x += DANCE_MOVE_DIS;
                    pos[ 1 ].x += DANCE_MOVE_DIS;
                    pos[ 2 ].x += DANCE_MOVE_DIS;
                    pos[ 3 ].x += DANCE_MOVE_DIS;
                    pos[ 4 ].x += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x += DANCE_MOVE_DIS;
                    pos[ 1 ].x += DANCE_MOVE_DIS;
                    pos[ 2 ].x += DANCE_MOVE_DIS;
                    pos[ 3 ].x += DANCE_MOVE_DIS;
                    pos[ 4 ].x += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 傾きアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setCantLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }
    
    void danceFourBTwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x -= DANCE_MOVE_DIS;
                    pos[ 1 ].x -= DANCE_MOVE_DIS;
                    pos[ 2 ].x -= DANCE_MOVE_DIS;
                    pos[ 3 ].x -= DANCE_MOVE_DIS;
                    pos[ 4 ].x -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].x -= DANCE_MOVE_DIS;
                    pos[ 1 ].x -= DANCE_MOVE_DIS;
                    pos[ 2 ].x -= DANCE_MOVE_DIS;
                    pos[ 3 ].x -= DANCE_MOVE_DIS;
                    pos[ 4 ].x -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 傾きアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setCantRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }
    
    void danceFiveOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollAdvance( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollAdvance( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 傾きアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setCantRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceFiveTwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollAdvance( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollAdvance( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 傾きアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setCantLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceFiveThree( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollBack( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollBack( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 傾きアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setCantRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceFiveFour( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollBack( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setRollBack( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 傾きアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setCantLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceSixOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 歩きアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setWalkAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 歩きアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setWalkAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 歩きアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFastWalk( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 3:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 歩きアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFastWalk( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }

					_group[ group_num ].setDanceFinish( true );
					_group[ group_num ].updatePartCount( );
                }
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }
    void danceSevenAOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsA( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsA( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                 //ジャンプアニメ
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
						}
                    }
                break;
			case 3: 
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceSevenATwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 回転アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
							SpiningJumpA( i, _group[ group_num ].getDanceType( ) );
					}
                }
                break;
			case 3: 
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceSevenBOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                //ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
			case 3: 
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceSevenBTwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsA( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsA( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                // 回転アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						SpiningJumpB( i, _group[ group_num ].getDanceType( ) );
					}
                }
                break;
			case 3: 
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
				break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceEightOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
                }
                break;
            case 3:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
                    // 転がるアニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							RollDanceXsB( i, _group[ group_num ].getDanceType( ) );
							pos_num++;
						}
                    }
					_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                }
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }
    
    void danceEightTwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 1 ||
						 _cube_manager.getGroupNum( i ) == 2 ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
            case 1:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 0 ||
						 _cube_manager.getGroupNum( i ) == 3 ||
						 _cube_manager.getGroupNum( i ) == 4 ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
            case 2:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 1 ||
						 _cube_manager.getGroupNum( i ) == 2 ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
                break;
            case 3:
                // ジャンプアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 0 ||
						 _cube_manager.getGroupNum( i ) == 3 ||
						 _cube_manager.getGroupNum( i ) == 4 ) {
						_cube_manager.setJumpAnim( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 2, _anim_length );
					}
                }
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceNineOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 0 ||
						 _cube_manager.getGroupNum( i ) == 3 ||
						 _cube_manager.getGroupNum( i ) == 4 ) {
						_cube_manager.setBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 1:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 1 ||
						 _cube_manager.getGroupNum( i ) == 2 ) {
						_cube_manager.setBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 2:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 0 ||
						 _cube_manager.getGroupNum( i ) == 3 ||
						 _cube_manager.getGroupNum( i ) == 4 ) {
						_cube_manager.setBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 3:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 1 ||
						 _cube_manager.getGroupNum( i ) == 2 ) {
						_cube_manager.setBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceNineTwo( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 0 ||
						 _cube_manager.getGroupNum( i ) == 3 ||
						 _cube_manager.getGroupNum( i ) == 4 ) {
						_cube_manager.setFastBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 1:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 1 ||
						 _cube_manager.getGroupNum( i ) == 2 ) {
						_cube_manager.setFastBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 2:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 0 ||
						 _cube_manager.getGroupNum( i ) == 3 ||
						 _cube_manager.getGroupNum( i ) == 4 ) {
						_cube_manager.setFastBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
                break;
            case 3:
                // 鼓動アニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == 1 ||
						 _cube_manager.getGroupNum( i ) == 2 ) {
						_cube_manager.setFastBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
					}
                }
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

     void danceNineThree( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 3:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
                    for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z -= DANCE_MOVE_DIS;
                    pos[ 1 ].z += DANCE_MOVE_DIS;
                    pos[ 2 ].z += DANCE_MOVE_DIS;
                    pos[ 3 ].z -= DANCE_MOVE_DIS;
                    pos[ 4 ].z -= DANCE_MOVE_DIS;
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }

					_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                }
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceNineFour( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
					for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
					
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 1:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
					for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
					
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 2:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
					for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
					
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }
                }
                break;
            case 3:
                {
                    // 座標移動
                    Vector3[ ] pos = new Vector3[ Group.MEMBER_NUM ];
					for ( int i = 0; i < Group.MEMBER_NUM; i++ ) {
                        pos[ i ] = _group[ group_num ].getMemberPos( i ).localPosition;
                    }
                    pos[ 0 ].z += DANCE_MOVE_DIS;
                    pos[ 1 ].z -= DANCE_MOVE_DIS;
                    pos[ 2 ].z -= DANCE_MOVE_DIS;
                    pos[ 3 ].z += DANCE_MOVE_DIS;
                    pos[ 4 ].z += DANCE_MOVE_DIS;
					
					// 四回鼓動アニメ
					int pos_num = 0;
                    for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
						if ( _cube_manager.getGroupNum( i ) == group_num ) {
							_group[ group_num ].setMemberPos( pos_num, pos[ pos_num ] );
							_cube_manager.setFasterBeat( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
							pos_num++;
						}
                    }

					_group[ group_num ].setDanceFinish( true );
					_group[ group_num ].updatePartCount( );
                }
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceTenOne( int group_num ) {
        // ダンス処理( 4拍子 )
		switch ( _group[ group_num ].getDanceCount( ) ) {
            case 0:
                // 伸びるアニメ
                for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
					if ( _cube_manager.getGroupNum( i ) == group_num ) {
						_cube_manager.setExtendDeparture( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ) * 7, _anim_length );
					}
                }
                break;
            case 6:
				_group[ group_num ].setDanceFinish( true );
				_group[ group_num ].updatePartCount( );
                _group[ group_num ].finishDancePart( );
				_group[ group_num ].setDanceType( DANCE_TYPE.DANCE_FIN_ONE );
				if ( _group[ group_num ].getGroupType( ) == GROUP_TYPE.GROUP_A ) {
					 scatteredCube( );
				}
                break;
        }
        // ダンスカウントを１増やす
        _group[ group_num ].updateDanceCount( );
    }

    void danceFinTwo( int group_num ) {

    }

    /// <summary>
    /// 味方キューブがはじけ飛ぶ処理
    /// </summary>
    void scatteredCube( ) {
        for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
            // プレイヤーへのベクトル
            Vector3 vec = _player.transform.position - _cube_manager.getAlliyCube( i ).transform.position;
            float time = _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN );
            time = 1.0f;    // デバッグタイム

            float distance = Vector3.Distance( _player.transform.position, _cube_manager.getAlliyCube( i ).transform.position );
            vec = vec.normalized * distance * 2.0f;
            _cube_manager.setTarget( i, vec, time );
			_cube_manager.scatteredStart( i );
			Debug.Log( vec );
        }
    }

    void finalGather( ) {
		// 集合する場所
		Vector3[ ] gether_pos = new Vector3[ _cube_manager.cubesNum( ) ];
		float angle = 360 / _cube_manager.cubesNum( ) * Mathf.Deg2Rad;
		
        for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
			// ターゲットと同期
			gether_pos[ i ] = _last_target.transform.position;
			// 集まる座標を極座標で求める
			gether_pos[ i ].x += LAST_CIRCLE_DIS * Mathf.Cos( angle * i );
			gether_pos[ i ].y += LAST_CIRCLE_DIS * Mathf.Sin( angle * i );
            float time = _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) / 60f;
            time = 5.0f;   

            float distance = Vector3.Distance( gether_pos[ i ], _cube_manager.getAlliyCube( i ).transform.position );
            _cube_manager.setTarget( i, gether_pos[ i ], time );
			_cube_manager.moveStart( i );
        }
    }


    private void RollDanceXsA( int i, DANCE_TYPE type ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす場合
                if ( ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) ) {
                    _cube_manager.setRollBack( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
                // 3個動かす場合
                else if ( ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) ) {
                    _cube_manager.setRollAdvance( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
            }
        }
    }

    private void RollDanceXsB( int i, DANCE_TYPE type ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす場合
                if ( ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) ) {
                    _cube_manager.setRollAdvance( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
                // 3個動かす場合
                else if ( ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) ) {
                    _cube_manager.setRollBack( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
            }
        }
    }

    private void SpiningJumpA( int i, DANCE_TYPE type ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす場合
                if ( ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) ) {
                    _cube_manager.setSpinJumpRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
                // 3個動かす場合
                else if ( ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) ) {
                    _cube_manager.setSpinJumpLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
            }
        }
    }

    private void SpiningJumpB( int i, DANCE_TYPE type ) {
        for ( int j = 0; j < GROUP_NUM; j++ ) {
            if ( _cube_manager.getGroupNum( i ) == j ) {
                // 2個動かす場合
                if ( ( _cube_manager.getMemberNum( i ) == 1 || _cube_manager.getMemberNum( i ) == 2 ) ) {
                    _cube_manager.setSpinJumpLeft( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
                // 3個動かす場合
                else if ( ( _cube_manager.getMemberNum( i ) == 0 || _cube_manager.getMemberNum( i ) == 3 || 
                       _cube_manager.getMemberNum( i ) == 4 ) ) {
                    _cube_manager.setSpinJumpRight( i, BASE_FRAME, _rhythm_manager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN ), _anim_length );
                }
            }
        }
    }

    private void addForce( int i, int group_num ) {
        if ( _cube_manager.getGroupNum( i ) == group_num ) {
            _cube_manager.moveStart( i );
        }
    }

	private void move( int group_num ){
		//加速処理
		if ( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MAIN ) % 1 == 0 ) {
            for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
                addForce( i, group_num );
			}
        }
		
		//減速処理
        if ( _rhythm_manager.getNextBetweenFrameMain( ) % 4 == ( int )( _rhythm_manager.getNextBetweenFrameMain( ) / 3.0f ) &&
             _group[ group_num ].getDanceType( ) != DANCE_TYPE.DANCE_NONE ) {
            for ( int i = 0; i < _cube_manager.cubesNum( ); i++ ) {
                _cube_manager.speedDown( );
			}
		}
		 

	}
}
