using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Common;

public class EditRhythmManager : RhythmManager {

	[ SerializeField ]
	private RhythmViewer _rhythmViewer;

	//protected new TIMING_DATA[ ] _data;
	
	public void setFrame( int frame ) {
		_frame = frame;
	}

	public void setIndex( int index ) {
		_index = index;
	}

	public void stop( ) {
		setFrame( 0 );
		setIndex( 0 );
	 }

	protected override bool isError( ) {
		bool error = false;

		// データの取得
		if ( _data.md == null ) {
			_data.md = _rhythmViewer.getRhythmData( );
			error = true;
		}
		return error;
	}

	protected override void updateRhythm( ) {
		// 実行確認
		if ( !isPlay( ) ) {
			return;
		}

		// タイミングの確認
		try {
			if ( _data.md[ _index ].frame == ( uint )_frame ) {
				_timing = true;
				_index += ( _index < _data.md.Length - 1 )? 1 : 0;	// インデックスのオーバーの抑制
			} else {
				_timing = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}
		
		// フレームのカウント
		_frame++;
	}

	// リロード
	public void reloadData( ) {
		Array.Clear( _data.md, 0, _data.md.Length );
	}

	// データのセット
	public void setData( TIMING_DATA[ ] data ) {
		_data.md = new TIMING_DATA[ data.Length ];
		for ( int i = 0; i < data.Length; i++ ) {
			_data.md[ i ] = data[ i ];
		}
	}

	public int getIndex( ) {
		// タイミング時の確認
		if ( isTiming( ) ) {
			return _index - 1;	// 自動くり上げの都合で	-1をする。
		} else {
			return _index;
		}
	}

	/// <summary>
	/// データの総数
	/// </summary>
	/// <returns></returns>
	public int getDataCount( ) {
		return _data.md.Length;
	}

	/// <summary>
	/// インデックスの加算
	/// </summary>
	/// <param name="value"></param>
	public void addIndex( int value ) {
		_index += value;
	}
    
    /// <summary>
    /// タイミングデータを取得
    /// </summary>
    /// <param name="index"> インデックス </param>
    /// <returns> 成功：データ情報 失敗：null </returns>
    public TIMING_DATA getTimingData( int index ) {
        TIMING_DATA data = new TIMING_DATA( );

        // 範囲外
        if ( index < 0 || index >= _data.md.Length ) {
            return data;
        }

        data = _data.md[ index ];
        return data;
    }
 }
