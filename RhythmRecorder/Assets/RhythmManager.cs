using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

[ RequireComponent( typeof( AudioSource ), typeof( Music ) ) ]
public class RhythmManager : MonoBehaviour {

	// エディター設定
	[ SerializeField ]
	private AudioClip _audioClip;

	[ SerializeField ]
	private bool _awakeStart = false;	// 起動時に開始フラグ

	// インスタンス
	private AudioSource _audioSource;

	// 変数
	private int _index = 0;		// タイミングのインデックス
	private int _frame = 0;		// フレーム数
	private bool _play = false;
	private bool _fileReady = false;	// ファイル読み込みのフラグ
	private bool _timing = false;
	private List< FILE_DATA > _data = new List< FILE_DATA >( );

	void Awake( ) {
		_audioSource = GetComponent< AudioSource >( );
		_audioSource.clip = _audioClip;
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		if ( isError( ) ) {
			return;
		}
		updateRhythm( );
		updateMusic( );

		// 起動時に開始
		if ( isFileLoad( ) ) {
			if ( _awakeStart ) {
				_awakeStart = false;
				_audioSource.Play( );
			}
		}
		
		// debug
		if ( isTiming( ) ) {
			Debug.Log( "index : " + _data[ _index ].index + " frame : " + _data[ _index ].frame );
			Debug.Log( "next frame : " + getNextBetweenFrame( ) + " current frame : " + _frame );
		}
	}

	void updateRhythm( ) {
		// 実行確認
		if ( !isPlay( ) ) {
			return;
		}
        /*
		// タイミングの確認
		if ( _data[ _index ].frame == _frame ) {
			_timing = true;
			_index += ( _index < _data.Count - 1 )? 1 : 0;	// インデックスのオーバーの抑制
		} else {
			_timing = false;
		}
        */
		
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

		// ファイルのロード
		if ( !_fileReady ) {
			_fileReady = FileManager.loadFile( _audioClip.name, ref _data );
			error = ( _fileReady )? false : true; 
		}
		return error;
	}

	/// <summary>
	/// ファイルの読み込み確認
	/// </summary>
	/// <returns></returns>
	private bool isFileLoad( ) {
		return _fileReady;
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
	/// 次のタイミングフレーム数と返す
	/// </summary>
	public int getTiming( ) {
		return ( int )_data[ _index ].frame;
	}

	/// <summary>
	/// 次のタイミングまでのフレーム数を返す
	/// </summary>
	/// <returns> 1以上( true )　0は失敗( false ) </returns>
	public int getNextBetweenFrame( ) {
		int frame = 0;
		if ( _index < _data.Count - 1 ) {
			frame = ( int )( _data[ _index + 1 ].frame - _data[ _index ].frame );
		}
		return frame;
	}

	/// <summary>
	/// 前のタイミングフレームを返す
	/// </summary>
	/// <returns> 1以上( true ), 0失敗( false ) </returns>
	public int gatBeforeTiming( ) {
		if ( _index < 1 || _index > _data.Count - 1 ) {
			return 0;
		}
		return ( int )_data[ _index - 1 ].frame;
	}

	/// <summary>
	/// タイミングの確認
	/// </summary>
	/// <returns></returns>
	public bool isTiming( ) {
		return _timing;
	}
}
