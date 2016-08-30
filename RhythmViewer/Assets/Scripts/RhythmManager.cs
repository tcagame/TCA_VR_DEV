using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

[ RequireComponent( typeof( AudioSource ), typeof( Music ) ) ]
public class RhythmManager : MonoBehaviour {

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
	private int _index = 0;		// タイミングのインデックス
	private int _frame = 0;		// フレーム数
	private bool _play = false;
	private bool _timing = false;
	private FILE_DATA.RHYTHM _melodyData;

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
			if ( isTiming( ) ) {
				Debug.Log( "index : " + _melodyData.array[ _index ].index + " frame : " + _melodyData.array[ _index ].frame );
				Debug.Log( "next frame : " + getNextBetweenFrame( ) + " current frame : " + _frame );
			}
			_debugView.drawFrequency( ref _melodyData.array, getIndex( ), getFrame( ) );
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
			if ( _melodyData.array[ _index ].frame == ( uint )_frame ) {
				_timing = true;
				_index += ( _index < _melodyData.array.Length - 1 )? 1 : 0;	// インデックスのオーバーの抑制
			} else {
				_timing = false;
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
		if ( Music.IsPlaying ) {
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
		if ( _melodyData.array == null ) {
			_melodyData = FileManager.getInstance( ).getRhythmData( FileManager.TAG.MELODY );
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
	public int getTiming( ) {
		return ( int )_melodyData.array[ _index ].frame;
	}

	/// <summary>
	/// 次のタイミングまでのフレーム数を返す
	/// </summary>
	/// <returns> 1以上( true )　0は失敗( false ) </returns>
	public int getNextBetweenFrame( ) {
		int frame = 0;
		if ( _index < _melodyData.array.Length - 1 ) {
			frame = ( int )( _melodyData.array[ _index + 1 ].frame - _melodyData.array[ _index ].frame );
		}
		return frame;
	}

	/// <summary>
	/// 前のタイミングフレームを返す
	/// </summary>
	/// <returns> 1以上( true ), 0失敗( false ) </returns>
	public int gatBeforeTiming( ) {
		if ( _index < 1 || _index > _melodyData.array.Length - 1 ) {
			return 0;
		}
		return ( int )_melodyData.array[ _index - 1 ].frame;
	}

	/// <summary>
	/// タイミングの確認
	/// </summary>
	/// <returns></returns>
	public bool isTiming( ) {
		return _timing;
	}

	/// <summary>
	/// インデックス番号を取得
	/// </summary>
	/// <returns></returns>
	public int getIndex( ) {
		// タイミング時の確認
		if ( isTiming( ) ) {
			return _index - 1;	// 自動くり上げの都合で	-1をする。
		} else {
			return _index;
		}
	}

	/// <summary>
	/// リセット
	/// </summary>
	public void reset( ) {
		_index = 0;		// タイミングのインデックス
		_frame = 0;		// フレーム数
		_play = false;
		_timing = false;
	}
}
