using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class RhythmRecoder : MonoBehaviour {

	// エディター設定
	[ SerializeField ]
	private RhythmManager _rhythmMgr;

	[ SerializeField ]
	private AudioSource _audioSource;

	[ SerializeField ]
	private FFTWindow _FFTWinMode = FFTWindow.Hamming;	// フーリエ変換モード

	public int _resolution = 256;	// 解像度(2のべき乗で設定)

	public bool _drawFrequency = false;	// 周波数データの表示
	
	

	// 変数
	private uint _frame = 0;
	private bool _requestRecord = false;	// 記録のリクエストフラグ
	private List< FILE_DATA > _list = new List< FILE_DATA >( );
	private List< FILE_DATA > _loadFileData = new List< FILE_DATA >( );

	// Use this for initialization
	void Awake( ) {
		// 2のべき乗の確認
		if ( ( _resolution & _resolution - 1 ) != 0 ) {
            _resolution = 256;
			Debug.LogError( "解像度を2のべき乗に設定してください。" );
        }

		FileManager.loadFile( _audioSource.clip.name, ref _loadFileData );
	}
	
	// Update is called once per frame
	void Update( ) {
		// 入力
		if ( Input.GetKeyDown( KeyCode.Space ) ) {
			_requestRecord = true;
		}

		// ファイル書き出し
		if ( Input.GetKeyDown( KeyCode.F1 ) ) {
			FileManager.saveFile( _audioSource.clip.name, ref _list );
			Debug.Log( "Save" );
		}

		// ファイルのロード
		if ( Input.GetKeyDown( KeyCode.F2 ) ) {
			FileManager.loadFile( _audioSource.clip.name, ref _loadFileData );
			Debug.Log( "Load" );
        }

	}

	void FixedUpdate( ) {
		// フレーム更新
		if ( _rhythmMgr.isPlay( ) ) {
			_frame++;
		} else {
			_frame = 0;
		}
		
		// 記録
		if ( _requestRecord ) {
			// データに記録
			Common.FILE_DATA data;
			data.index = _list.Count;
			data.frame = _frame;

			// 追加
			_list.Add( data );

			Debug.Log( data );	// debug

			// フラグ解除
			_requestRecord = false;
		}

		// 周波数の表示
		if ( _drawFrequency ) {
			float[ ] frequency = _audioSource.GetSpectrumData( _resolution, 0, _FFTWinMode );
			drawFrequency( ref frequency );
		}
	}


	/// <summary>
	/// 周波数の表示
	/// </summary>
	/// <param name="data"> 周波数のデータ配列 </param>
	void drawFrequency( ref float[ ] data ) {
		int scale = 1;	// 表示スケール
		scale = ( scale < 1 )? 1 : scale;
		for ( int i = 1; i < data.Length / scale - 1; i++ ) {
            Debug.DrawLine(
                    new Vector3( i - 1, data[ i ] + 10, 0 ), 
                    new Vector3( i, data[ i + 1 ] + 10, 0 ), 
                    Color.red );
            Debug.DrawLine(
                    new Vector3( i - 1, Mathf.Log( data[ i - 1 ] ) + 10, 2 ), 
                    new Vector3( i, Mathf.Log( data[ i ] ) + 10, 2 ), 
                    Color.cyan );
            Debug.DrawLine(
                    new Vector3( Mathf.Log( i - 1 ), data[ i - 1 ] - 10, 1 ), 
                    new Vector3( Mathf.Log( i ), data[ i ] - 10, 1 ), 
                    Color.green );
            Debug.DrawLine(
                    new Vector3( Mathf.Log( i - 1 ), Mathf.Log( data[ i - 1 ] ), 3 ), 
                    new Vector3( Mathf.Log( i ), Mathf.Log( data[ i ] ), 3 ), 
                    Color.yellow );
        }
	}
}
