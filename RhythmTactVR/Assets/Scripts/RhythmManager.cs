using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

[ RequireComponent( typeof( AudioSource ), typeof( Music ) ) ]
public class RhythmManager : MonoBehaviour {

    // リズムタグ
    public enum RHYTHM_TAG { 
        MAIN,
        SUB,
        VOCAL,
        MODE_CHANGE,
        GROUP_ANIM,
        RHYTM_NUM
    }; 

	#region DebugView Class 
	[ System.Serializable ]
	public class DebugView {

		private RhythmManager _rhythmMgr;

		public DebugView( RhythmManager rhythmMgr ) {
			_rhythmMgr = rhythmMgr;
		}

		/// <summary>
		/// 周波数の表示
		/// </summary>
		/// <param name="data"> 周波数のデータ配列 </param>
		public void drawFrequency( ref TIMING_DATA[ ] data, int index, int frame ) {
			int scale = 100;
			int barHight = 1;
			float width = 0.2f;
			for ( int i = 0; i < scale; i++ ) {
				for ( int j = index; j < data.Length / 2; j++ ) {
					// タイミングのライン
					if ( i + frame  == data[ index ].frame ) {
						Debug.DrawLine(
								new Vector3( i * width, barHight, 0 ), 
								new Vector3( i * width, -barHight, 0 ), 
								Color.red );
					}
				}
				// 下地のライン
				Debug.DrawLine(
						new Vector3( i * width , 0, 0 ), 
						new Vector3( ( i + 1 ) * width, 0, 0 ), 
						Color.cyan );
			}
		}

	}
	#endregion

	// エディター設定
	[ SerializeField ]
	private bool _awakeStart = false;	// 起動時に開始フラグ
	[ SerializeField ]
	private bool _debugDraw = false;	// デバッグ表示

	// インスタンス
	private AudioSource _audioSource;

	private DebugView _debugView;

	// 変数
	protected int[ ] _index = new int[ ( int )RHYTHM_TAG.RHYTM_NUM ] { 0, 0, 0, 0, 0 };		// タイミングのインデックス
	protected int _frame = 0;		// フレーム数
	protected bool _play = false;
	protected bool[ ] _timing = new bool[ ( int )RHYTHM_TAG.RHYTM_NUM ] { false, false, false, false, false };
	protected bool[ ] _finish = new bool[ ( int )RHYTHM_TAG.RHYTM_NUM ] { false, false, false, false, false };
	protected int[ ] _last_index = new int [ ( int )RHYTHM_TAG.RHYTM_NUM ] { 0, 0, 0, 0, 0 };	
	protected FILE_DATA.RHYTHM _data;

	void Awake( ) {
		_audioSource = GetComponent< AudioSource >( );
		
		_debugView = new DebugView( GetComponent< RhythmManager >( ) );

	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		if ( isError( ) ) {
			return;
		}
		
		updateRhythm( );
		updateMusic( );

		// デバッグ表示
		if ( _debugDraw ) {
			// debug
			if ( isTiming( RHYTHM_TAG.MAIN ) ) {
				Debug.Log( "index : " + _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].index + " frame : " + _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( RHYTHM_TAG.MAIN ) + " current frame : " + _frame );
			} 
			_debugView.drawFrequency( ref _data.ma, getIndex( RHYTHM_TAG.MAIN ), getFrame( ) );

            if ( isTiming( RHYTHM_TAG.SUB ) ) {
				Debug.Log( "index : " + _data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] ].index + " frame : " + _data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( RHYTHM_TAG.SUB ) + " current frame : " + _frame );
			} 
			_debugView.drawFrequency( ref _data.sb, getIndex( RHYTHM_TAG.SUB ), getFrame( ) );
            
            if ( isTiming( RHYTHM_TAG.VOCAL ) ) {
				Debug.Log( "index : " + _data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] ].index + " frame : " + _data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( RHYTHM_TAG.VOCAL ) + " current frame : " + _frame );
			} 
			_debugView.drawFrequency( ref _data.vo, getIndex( RHYTHM_TAG.VOCAL ), getFrame( ) );
            
            if ( isTiming( RHYTHM_TAG.MODE_CHANGE ) ) {
				Debug.Log( "index : " + _data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] ].index + " frame : " + _data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( RHYTHM_TAG.MODE_CHANGE ) + " current frame : " + _frame );
			} 
			_debugView.drawFrequency( ref _data.md, getIndex( RHYTHM_TAG.MODE_CHANGE ), getFrame( ) );
            
            if ( isTiming( RHYTHM_TAG.GROUP_ANIM ) ) {
				Debug.Log( "index : " + _data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] ].index + " frame : " + _data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( RHYTHM_TAG.GROUP_ANIM ) + " current frame : " + _frame );
			} 
			_debugView.drawFrequency( ref _data.ga, getIndex( RHYTHM_TAG.GROUP_ANIM ), getFrame( ) );
		}

		// 起動時に開始
		if ( _awakeStart ) {
			_awakeStart = false;
			_audioSource.Play( );
		}
		
	}

	protected virtual void updateRhythm( ) {
		// 実行確認
		if ( !isPlay( ) ) {
			return;
		}

		// タイミングの確認
		try {
			if ( _finish[ ( int )RHYTHM_TAG.MAIN ] == false ) {
				if ( _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].frame == ( uint )_frame ) {
					_timing[ ( int )RHYTHM_TAG.MAIN ] = true;
					_index[ ( int )RHYTHM_TAG.MAIN ] += ( _index[ ( int )RHYTHM_TAG.MAIN ] < _data.ma.Length ) ? 1 : 0;	// インデックスのオーバーの抑制
					// 終了のタイミング
					if ( _index[ ( int )RHYTHM_TAG.MAIN ] == _data.ma.Length ) {
						_finish[ ( int )RHYTHM_TAG.MAIN ] = true;
						_last_index[ ( int )RHYTHM_TAG.MAIN ] = _data.ma.Length - 1;
					}
				} else {
					_timing[ ( int )RHYTHM_TAG.MAIN ] = false;
				}
			} else {
				_timing[ ( int )RHYTHM_TAG.MAIN ] = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}

        try {
			if ( _finish[ ( int )RHYTHM_TAG.SUB ] == false ) {
				if ( _data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] ].frame == ( uint )_frame ) {
					_timing[ ( int )RHYTHM_TAG.SUB ] = true;
					_index[ ( int )RHYTHM_TAG.SUB ] += ( _index[ ( int )RHYTHM_TAG.SUB ] < _data.sb.Length ) ? 1 : 0;	// インデックスのオーバーの抑制
					// 終了のタイミング
					if ( _index[ ( int )RHYTHM_TAG.SUB ] == _data.sb.Length ) {
						_finish[ ( int )RHYTHM_TAG.SUB ] = true;
						_last_index[ ( int )RHYTHM_TAG.SUB ] = _data.sb.Length - 1;
					}
				} else {
					_timing[ ( int )RHYTHM_TAG.SUB ] = false;
				}
			} else {
				_timing[ ( int )RHYTHM_TAG.SUB ] = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}
        
        try {
			if ( _finish[ ( int )RHYTHM_TAG.VOCAL ] == false ) {
				if ( _data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] ].frame == ( uint )_frame ) {
					_timing[ ( int )RHYTHM_TAG.VOCAL ] = true;
					_index[ ( int )RHYTHM_TAG.VOCAL ] += ( _index[ ( int )RHYTHM_TAG.VOCAL ] < _data.vo.Length ) ? 1 : 0;	// インデックスのオーバーの抑制
					// 終了のタイミング
					if ( _index[ ( int )RHYTHM_TAG.VOCAL ] == _data.vo.Length ) {
						_finish[ ( int )RHYTHM_TAG.VOCAL ] = true;
						_last_index[ ( int )RHYTHM_TAG.VOCAL ] = _data.vo.Length - 1;
					}
				} else {
					_timing[ ( int )RHYTHM_TAG.VOCAL ] = false;
				}
			} else {
				_timing[ ( int )RHYTHM_TAG.VOCAL ] = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}
        
        try {
			if ( _finish[ ( int )RHYTHM_TAG.MODE_CHANGE ] == false ) {
				if ( _data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] ].frame == ( uint )_frame ) {
					_timing[ ( int )RHYTHM_TAG.MODE_CHANGE ] = true;
					_index[ ( int )RHYTHM_TAG.MODE_CHANGE ] += ( _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] < _data.md.Length ) ? 1 : 0;	// インデックスのオーバーの抑制
					// 終了のタイミング
					if ( _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] == _data.md.Length ) {
						_finish[ ( int )RHYTHM_TAG.MODE_CHANGE ] = true;
						_last_index[ ( int )RHYTHM_TAG.MODE_CHANGE ] = _data.md.Length - 1;
					}
				} else {
					_timing[ ( int )RHYTHM_TAG.MODE_CHANGE ] = false;
				}
			} else {
				_timing[ ( int )RHYTHM_TAG.MODE_CHANGE ] = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}
		
        
        try {
			if ( _finish[ ( int )RHYTHM_TAG.GROUP_ANIM ] == false ) {
				if ( _data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] ].frame == ( uint )_frame ) {
					_timing[ ( int )RHYTHM_TAG.GROUP_ANIM ] = true;
					_index[ ( int )RHYTHM_TAG.GROUP_ANIM ] += ( _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] < _data.ga.Length ) ? 1 : 0;	// インデックスのオーバーの抑制
					// 終了のタイミング
					if ( _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] == _data.ga.Length ) {
						_finish[ ( int )RHYTHM_TAG.GROUP_ANIM ] = true;
						_last_index[ ( int )RHYTHM_TAG.GROUP_ANIM ] = _data.ga.Length - 1;
					}
				} else {
					_timing[ ( int )RHYTHM_TAG.GROUP_ANIM ] = false;
				}
			} else {
				_timing[ ( int )RHYTHM_TAG.GROUP_ANIM ] = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}
		// フレームのカウント
		_frame++;
	}

	/// <summary>
	/// ミュージック（スクリプト）の更新
	/// </summary>
	void updateMusic( ) {
		// 再生フラグ
		if ( Music.MusicalTime > 0f ) {
			_play = true;
		} else {
			_play = false;
		}
	}

	/// <summary>
	/// エラーのチェック
	/// </summary>
	/// <returns></returns>
	protected virtual bool isError( ) {
		bool error = false;

		// データの取得
		if ( _data.ma == null || _data.sb == null || _data.vo == null || _data.md == null || _data.ga == null ) {
			_data = FileManager.getInstance( ).getRhythmData( );
			error = true;
		}
		return error;
	}

	/// <summary>
	/// 再生の確認
	/// </summary>
	/// <returns></returns>
	public bool isPlay( ) {
		return _play;
	}

	/// <summary>
	/// 現在のフレームを取得
	/// </summary>
	/// <returns></returns>
	public int getFrame( ) {
		return _frame;
	}

	/// <summary>
	/// 次のタイミングまでのフレーム数を返す
	/// </summary>
	public int getTiming( RHYTHM_TAG tag ) {
        int frame = 0;
        if ( tag == RHYTHM_TAG.MAIN ) {
            frame = ( int )_data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].frame;
        }
        else if ( tag == RHYTHM_TAG.SUB ) {
            frame = ( int )_data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] ].frame;
        }
        else if ( tag == RHYTHM_TAG.VOCAL ) {
            frame = ( int )_data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] ].frame;
        } 
        else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
            frame = ( int )_data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] ].frame;
        }
        else if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
            frame = ( int )_data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] ].frame;
        }
		return frame;
	}

	/// <summary>
	/// 次のタイミングまでのフレーム数を返す
	/// </summary>
	/// <returns> 1以上( true )　0は失敗( false ) </returns>
	public int getNextBetweenFrame( RHYTHM_TAG tag ) {
		int frame = 0;

        if ( tag == RHYTHM_TAG.MAIN ) {
		    if ( _index[ ( int )RHYTHM_TAG.MAIN ] < _data.ma.Length - 1 ) {
				if ( isTiming( tag ) ) {
					frame = ( int )( _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].frame - _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] - 1 ].frame );
				} else {
					frame = ( int )( _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] + 1 ].frame - _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].frame );
				}
		    }
        } else if ( tag == RHYTHM_TAG.SUB ) {
		    if ( _index[ ( int )RHYTHM_TAG.SUB ] < _data.sb.Length - 1 ) {
				if ( isTiming( tag ) ) {
					frame = ( int )( _data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] ].frame - _data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] - 1 ].frame );
				} else {
					frame = ( int )( _data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] + 1 ].frame - _data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] ].frame );
				}
			}
        } else if ( tag == RHYTHM_TAG.VOCAL ) {
		    if ( _index[ ( int )RHYTHM_TAG.VOCAL ] < _data.vo.Length - 1 ) {
				if ( isTiming( tag ) ) {
					frame = ( int )( _data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] ].frame - _data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] - 1 ].frame );
				} else {
					frame = ( int )( _data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] + 1 ].frame - _data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] ].frame );
				}
		    }
		    
        } else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
		    if ( _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] < _data.md.Length - 1 ) {
				if ( isTiming( tag ) ) {
					frame = ( int )( _data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] ].frame - _data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] - 1 ].frame );
				} else {
					frame = ( int )( _data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] + 1 ].frame - _data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] ].frame );
				}
		    }
        } else if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
		    if ( _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] < _data.ga.Length - 1 ) {
				if ( isTiming( tag ) ) {
					frame = ( int )( _data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] ].frame - _data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] - 1 ].frame );
				} else {
					frame = ( int )( _data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] + 1 ].frame - _data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] ].frame );
				}
		    }
        }

		return frame;
	}

    /// <summary>
	/// 4拍子あとのタイミングまでのフレーム数を返す
	/// </summary>
	/// <returns> 1以上( true )　0は失敗( false ) </returns>
	public int getNextBetweenFrameMain( ) {
		int frame = 0;

		if ( _index[ ( int )RHYTHM_TAG.MAIN ] < _data.ma.Length - 4 ) {
			frame = ( int )( _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] + 4 ].frame - _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].frame );
		} else if ( _index[ ( int )RHYTHM_TAG.MAIN ] < _data.ma.Length - 1 ) {
			frame = ( int )( _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] - 1 ].frame - _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ]  ].frame );
		}

		if ( isTiming( RHYTHM_TAG.MAIN ) ) {
			if ( _index[ ( int )RHYTHM_TAG.MAIN ] - 1 < _data.ma.Length - 5 ) {
				frame = ( int )( _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] + 3 ].frame - _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] - 1 ].frame );
			} else if ( _index[ ( int )RHYTHM_TAG.MAIN ] < _data.ma.Length ) {
				frame = ( int )( _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] ].frame - _data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] - 1 ].frame );
			}
		}

		return frame;
	}

	/// <summary>
	/// 前のタイミングフレームを返す
	/// </summary>
	/// <returns> 1以上( true ), 0失敗( false ) </returns>
	public int gatBeforeTiming( RHYTHM_TAG tag ) {
        int frame = 0;

        if ( tag == RHYTHM_TAG.MAIN ) {
		    if ( _index[ ( int )RHYTHM_TAG.MAIN ] < 1 || _index[ ( int )RHYTHM_TAG.MAIN ] > _data.ma.Length - 1 ) {
			    frame = 0;
		    }
            else {
                frame = ( int )_data.ma[ _index[ ( int )RHYTHM_TAG.MAIN ] - 1 ].frame;
            }
        } else if ( tag == RHYTHM_TAG.SUB ) {
		    if ( _index[ ( int )RHYTHM_TAG.SUB ] < 1 || _index[ ( int )RHYTHM_TAG.SUB ] > _data.sb.Length - 1 ) {
			    frame = 0;
		    }
            else {
                frame = ( int )_data.sb[ _index[ ( int )RHYTHM_TAG.SUB ] - 1 ].frame;
            }
        }  else if ( tag == RHYTHM_TAG.VOCAL ) {
		    if ( _index[ ( int )RHYTHM_TAG.VOCAL ] < 1 || _index[ ( int )RHYTHM_TAG.VOCAL ] > _data.vo.Length - 1 ) {
			    frame = 0;
		    }
            else {
                frame = ( int )_data.vo[ _index[ ( int )RHYTHM_TAG.VOCAL ] - 1 ].frame;
            }
        } else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
		    if ( _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] < 1 || _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] > _data.md.Length - 1 ) {
			    frame = 0;
		    }
            else {
                frame = ( int )_data.md[ _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] - 1 ].frame;
            }
        } else if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
		    if ( _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] < 1 || _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] > _data.ga.Length - 1 ) {
			    frame = 0;
		    }
            else {
                frame = ( int )_data.ga[ _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] - 1 ].frame;
            }
        }
		return frame;
	}

	/// <summary>
	/// タイミングの確認
	/// </summary>
	/// <returns></returns>
	public bool isTiming( RHYTHM_TAG tag ) {
        bool timing = false;

        if ( tag == RHYTHM_TAG.MAIN ) {
            timing = _timing[ ( int )RHYTHM_TAG.MAIN ];
        } else if ( tag == RHYTHM_TAG.SUB ) {
            timing = _timing[ ( int )RHYTHM_TAG.SUB ];
        } else if ( tag == RHYTHM_TAG.VOCAL ) {
            timing = _timing[ ( int )RHYTHM_TAG.VOCAL ];
        } else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
            timing = _timing[ ( int )RHYTHM_TAG.MODE_CHANGE ];
        } else  if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
            timing = _timing[ ( int )RHYTHM_TAG.GROUP_ANIM ];
        } 
		return timing;
	}

	/// <summary>
	/// 終了確認
	/// </summary>
	/// <returns></returns>
	public bool isFinish( RHYTHM_TAG tag ) {
        bool finish = false;

        if ( tag == RHYTHM_TAG.MAIN ) {
            finish = _finish[ ( int )RHYTHM_TAG.MAIN ];
        } else if ( tag == RHYTHM_TAG.SUB ) {
            finish = _finish[ ( int )RHYTHM_TAG.SUB ];
        } else if ( tag == RHYTHM_TAG.VOCAL ) {
            finish = _finish[ ( int )RHYTHM_TAG.VOCAL ];
        } else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
            finish = _finish[ ( int )RHYTHM_TAG.MODE_CHANGE ];
        } else  if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
            finish = _finish[ ( int )RHYTHM_TAG.GROUP_ANIM ];
        } 
		return finish;
	}

	/// <summary>
	/// インデックス番号を取得
	/// </summary>
	/// <returns></returns>
	public int getIndex( RHYTHM_TAG tag ) {
        int index = 0;

		// タイミング時の確認
		if ( isTiming( tag ) ) {
			if ( isFinish( tag ) ) {
				if ( tag == RHYTHM_TAG.MAIN ) {
					index = _last_index[ ( int )RHYTHM_TAG.MAIN ];
				} else if ( tag == RHYTHM_TAG.SUB ) {
					index = _last_index[ ( int )RHYTHM_TAG.SUB ];
				} else if ( tag == RHYTHM_TAG.VOCAL ) {
					index = _last_index[ ( int )RHYTHM_TAG.VOCAL ];
				} else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
					index = _last_index[ ( int )RHYTHM_TAG.MODE_CHANGE ];
				} else if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
					index = _last_index[ ( int )RHYTHM_TAG.GROUP_ANIM ];
				}

			} 
			else {
				if ( tag == RHYTHM_TAG.MAIN ) {
					index = _index[ ( int )RHYTHM_TAG.MAIN ] - 1;	// 自動くり上げの都合で	-1をする。
				} else if ( tag == RHYTHM_TAG.SUB ) {
					index = _index[ ( int )RHYTHM_TAG.SUB ] - 1;	// 自動くり上げの都合で	-1をする。
				} else if ( tag == RHYTHM_TAG.VOCAL ) {
					index = _index[ ( int )RHYTHM_TAG.VOCAL ] - 1;	// 自動くり上げの都合で	-1をする。
				} else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
					index = _index[ ( int )RHYTHM_TAG.MODE_CHANGE ] - 1;	// 自動くり上げの都合で	-1をする。
				} else if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
					index = _index[ ( int )RHYTHM_TAG.GROUP_ANIM ] - 1;	// 自動くり上げの都合で	-1をする。
				}
			}
		} else {
			if ( tag == RHYTHM_TAG.MAIN ) {
			    index = _index[ ( int )RHYTHM_TAG.MAIN ];	// 自動くり上げの都合で	-1をする。
            } else if ( tag == RHYTHM_TAG.SUB ) {
			    index = _index[ ( int )RHYTHM_TAG.SUB ];	// 自動くり上げの都合で	-1をする。
            } else if ( tag == RHYTHM_TAG.VOCAL ) {
			    index = _index[ ( int )RHYTHM_TAG.VOCAL ];	// 自動くり上げの都合で	-1をする。
            } else if ( tag == RHYTHM_TAG.MODE_CHANGE ) {
			    index = _index[ ( int )RHYTHM_TAG.MODE_CHANGE ];	// 自動くり上げの都合で	-1をする。
            } else if ( tag == RHYTHM_TAG.GROUP_ANIM ) {
			    index = _index[ ( int )RHYTHM_TAG.GROUP_ANIM ];	// 自動くり上げの都合で	-1をする。
            }
		}

        return index;
	}
}
