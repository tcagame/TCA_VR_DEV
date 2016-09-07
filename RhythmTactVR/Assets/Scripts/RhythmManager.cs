using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

[ RequireComponent( typeof( AudioSource ), typeof( Music ) ) ]
public class RhythmManager : MonoBehaviour {

    // リズムタグ
    public enum RHYTHM_TAG { 
        MELODY,
        BEAT,
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
	private int[ ] _index = new int[ ( int )RHYTHM_TAG.RHYTM_NUM ] { 0, 0 };		// タイミングのインデックス
	private int _frame = 0;		// フレーム数
	private bool _play = false;
	private bool[ ] _timing = new bool[ ( int )RHYTHM_TAG.RHYTM_NUM ] { false, false };
	private FILE_DATA.RHYTHM _data;

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
			if ( isTiming( RHYTHM_TAG.MELODY ) ) {
				Debug.Log( "index : " + _data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] ].index + " frame : " + _data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( RHYTHM_TAG.MELODY ) + " current frame : " + _frame );
			} 
			_debugView.drawFrequency( ref _data.md, getIndex( RHYTHM_TAG.MELODY ), getFrame( ) );

            if ( isTiming( RHYTHM_TAG.BEAT ) ) {
				Debug.Log( "index : " + _data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] ].index + " frame : " + _data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( RHYTHM_TAG.BEAT ) + " current frame : " + _frame );
			} 
			_debugView.drawFrequency( ref _data.be, getIndex( RHYTHM_TAG.BEAT ), getFrame( ) );
		}

		// 起動時に開始
		if ( _awakeStart ) {
			_awakeStart = false;
			_audioSource.Play( );
		}
		
	}

	void updateRhythm( ) {
		// 実行確認
		if ( !isPlay( ) ) {
			return;
		}

		// タイミングの確認
		try {
			if ( _data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] ].frame == ( uint )_frame ) {
				_timing[ ( int )RHYTHM_TAG.MELODY ] = true;
				_index[ ( int )RHYTHM_TAG.MELODY ] += ( _index[ ( int )RHYTHM_TAG.MELODY ] < _data.md.Length - 1 ) ? 1 : 0;	// インデックスのオーバーの抑制
			} else {
				_timing[ ( int )RHYTHM_TAG.MELODY ] = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}

        try {
			if ( _data.md[ _index[ ( int )RHYTHM_TAG.BEAT ] ].frame == ( uint )_frame ) {
				_timing[ ( int )RHYTHM_TAG.BEAT ] = true;
				_index[ ( int )RHYTHM_TAG.BEAT ] += ( _index[ ( int )RHYTHM_TAG.BEAT ] < _data.be.Length - 1 ) ? 1 : 0;	// インデックスのオーバーの抑制
			} else {
				_timing[ ( int )RHYTHM_TAG.MELODY ] = false;
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
	private bool isError( ) {
		bool error = false;

		// データの取得
		if ( _data.md == null ) {
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
        if ( tag == RHYTHM_TAG.MELODY ) {
            frame = ( int )_data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] ].frame;
        }
        else if ( tag == RHYTHM_TAG.BEAT ) {
            frame = ( int )_data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] ].frame;
        }
		return frame;
	}

	/// <summary>
	/// 次のタイミングまでのフレーム数を返す
	/// </summary>
	/// <returns> 1以上( true )　0は失敗( false ) </returns>
	public int getNextBetweenFrame( RHYTHM_TAG tag ) {
		int frame = 0;

        if ( tag == RHYTHM_TAG.MELODY ) {
		    if ( _index[ ( int )RHYTHM_TAG.MELODY ] < _data.md.Length - 1 ) {
			    frame = ( int )( _data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] + 1 ].frame - _data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] ].frame );
		    }
		    if ( isTiming( tag ) ) {
			    frame = ( int )( _data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] ].frame - _data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] - 1 ].frame );
		    }
        } else if ( tag == RHYTHM_TAG.BEAT ) {
		    if ( _index[ ( int )RHYTHM_TAG.BEAT ] < _data.be.Length - 1 ) {
			    frame = ( int )( _data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] + 1 ].frame - _data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] ].frame );
		    }
		    if ( isTiming( tag ) ) {
			    frame = ( int )( _data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] ].frame - _data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] - 1 ].frame );
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

        if ( tag == RHYTHM_TAG.MELODY ) {
		    if ( _index[ ( int )RHYTHM_TAG.MELODY ] < 1 || _index[ ( int )RHYTHM_TAG.MELODY ] > _data.md.Length - 1 ) {
			    frame = 0;
		    }
            else if ( tag == RHYTHM_TAG.MELODY ) {
                frame = ( int )_data.md[ _index[ ( int )RHYTHM_TAG.MELODY ] - 1 ].frame;
            }
        } else if ( tag == RHYTHM_TAG.BEAT ) {
		    if ( _index[ ( int )RHYTHM_TAG.BEAT ] < 1 || _index[ ( int )RHYTHM_TAG.BEAT ] > _data.be.Length - 1 ) {
			    frame = 0;
		    }
            else if ( tag == RHYTHM_TAG.BEAT ) {
                frame = ( int )_data.be[ _index[ ( int )RHYTHM_TAG.BEAT ] - 1 ].frame;
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

        if ( tag == RHYTHM_TAG.MELODY ) {
            timing = _timing[ ( int )RHYTHM_TAG.MELODY ];
        } else if ( tag == RHYTHM_TAG.BEAT ) {
            timing = _timing[ ( int )RHYTHM_TAG.BEAT ];
        }
		return timing;
	}

	/// <summary>
	/// インデックス番号を取得
	/// </summary>
	/// <returns></returns>
	public int getIndex( RHYTHM_TAG tag ) {
        int index = 0;

		// タイミング時の確認
		if ( isTiming( tag ) ) {
            if ( tag == RHYTHM_TAG.MELODY ) {
			    index = _index[ ( int )RHYTHM_TAG.MELODY ] - 1;	// 自動くり上げの都合で	-1をする。
            } else if ( tag == RHYTHM_TAG.BEAT ) {
			    index = _index[ ( int )RHYTHM_TAG.BEAT ] - 1;	// 自動くり上げの都合で	-1をする。
            }
		} else {
			if ( tag == RHYTHM_TAG.MELODY ) {
			    index = _index[ ( int )RHYTHM_TAG.MELODY ];	// 自動くり上げの都合で	-1をする。
            } else if ( tag == RHYTHM_TAG.BEAT ) {
			    index = _index[ ( int )RHYTHM_TAG.BEAT ];	// 自動くり上げの都合で	-1をする。
            }
		}

        return index;
	}
}
