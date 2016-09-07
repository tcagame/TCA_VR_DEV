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
	
    [ SerializeField ]
    RhythmManager.RHYTHM_TAG _rhythm_tag = RhythmManager.RHYTHM_TAG.MELODY;

	// 変数
	private bool _requestRecord = false;	// 記録のリクエストフラグ
	private List< TIMING_DATA > _melody_list = new List< TIMING_DATA >( );
	private List< TIMING_DATA > _beat_list   = new List< TIMING_DATA >( );

	// Use this for initialization
	void Awake( ) {
		// 2のべき乗の確認
		if ( ( _resolution & _resolution - 1 ) != 0 ) {
            _resolution = 256;
			Debug.LogError( "解像度を2のべき乗に設定してください。" );
        }
	}
	
	// Update is called once per frame
	void Update( ) {
		// 入力
		if ( Input.GetKeyDown( KeyCode.Space ) ) {
			_requestRecord = true;
		}

		// ファイル書き出し
		if ( Input.GetKeyDown( KeyCode.F1 ) ) {
			saveFile( _audioSource.clip.name, ref _melody_list, ref _beat_list );
			Debug.Log( "Save" );
		}

        // モードチェンジ
        if ( Input.GetKeyDown( KeyCode.F2 ) ) {
			switch ( _rhythm_tag ) {
                case RhythmManager.RHYTHM_TAG.MELODY:
                    _rhythm_tag = RhythmManager.RHYTHM_TAG.BEAT;
                    break;
                case RhythmManager.RHYTHM_TAG.BEAT:
                    _rhythm_tag = RhythmManager.RHYTHM_TAG.MELODY;
                    break;
            }
		}

	}

	void FixedUpdate( ) {
		// 記録
		if ( _requestRecord ) {
            switch ( _rhythm_tag ) {
                case RhythmManager.RHYTHM_TAG.MELODY:
                    {
			            // データに記録
			            TIMING_DATA data;
			            data.index = _melody_list.Count;
			            data.frame = ( uint )_rhythmMgr.getFrame( );

			            // 追加
			            _melody_list.Add( data );

			            Debug.Log( data );	// debug
                    }
                    break;
                case RhythmManager.RHYTHM_TAG.BEAT:
                    {
			            // データに記録
			            TIMING_DATA data;
			            data.index = _beat_list.Count;
			            data.frame = ( uint )_rhythmMgr.getFrame( );

			            // 追加
			            _beat_list.Add( data );

			            Debug.Log( data );	// debug
                    }
                    break;
            }
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

	/// <summary>
	/// ファイルのセーブ
	/// </summary>
	/// <param name="fileName"> ファイルの名前 </param>
	/// <param name="list"> ファイルデータ型のリスト </param>
	public bool saveFile( string fileName, ref List< TIMING_DATA > melody_list, ref List< TIMING_DATA > beat_list ) {
		try {
			StreamWriter sw = new StreamWriter( Application.dataPath + "/" + fileName + ".csv", false );

			// 個数の書き込み
			sw.Write( melody_list.Count );
			sw.Write( "," );
			sw.Write( "," );
			sw.WriteLine( beat_list.Count );

            int length = 0;
            // 大きいほうを設定
            if ( melody_list.Count > beat_list.Count ) {
                length = melody_list.Count;
            } else if ( beat_list.Count > melody_list.Count ) {
                length = beat_list.Count;
            }

			for ( int i = 0; i < length; i++ ) {
                // タイミングデータを書き込み
                if ( melody_list.Count < length && i >= melody_list.Count ) {
                    sw.Write( "," );
                    sw.Write( "," );
                    sw.Write( beat_list[ i ].index );
                    sw.Write( "," );
                    sw.WriteLine( beat_list[ i ].frame );
                } else if ( beat_list.Count < length && i >= beat_list.Count ) {
                    sw.Write( beat_list[ i ].index );
                    sw.Write( "," );
                    sw.Write( beat_list[ i ].frame );
                    sw.Write( "," );
                    sw.WriteLine( "," );
                } else {
				    sw.Write( melody_list[ i ].index );
				    sw.Write( "," );
				    sw.Write( melody_list[ i ].frame );
				    sw.Write( "," );
				    sw.Write( beat_list[ i ].index );
				    sw.Write( "," );
				    sw.WriteLine( beat_list[ i ].frame );
                }
			}
			sw.Close( );

			return true;
		} catch {
			Debug.LogError( "Missing Save File..." );
			return false;
		}
	}
}
