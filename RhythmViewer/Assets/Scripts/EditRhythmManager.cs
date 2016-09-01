using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class EditRhythmManager : RhythmManager {

	[ SerializeField ]
	private EditFileManager _editFileManager;

	protected new List< TIMING_DATA > _data;
	
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
		if ( _data == null ) {
			_data = _editFileManager.getRhythmData( );
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
			if ( _data[ _index ].frame == ( uint )_frame ) {
				_timing = true;
				_index += ( _index < _data.Count - 1 )? 1 : 0;	// インデックスのオーバーの抑制
			} else {
				_timing = false;
			}
		} catch {
			Debug.LogError( "Not Rhythm Data" );
		}
		
		// フレームのカウント
		_frame++;
	}
 }
