using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine.UI;
using Common;
using UnityEngine.EventSystems;

public class RhythmViewer : MonoBehaviour {

	
	#region ステージ　クラス
	[ System.Serializable ]
	private class Stage {
		public int _stageIndex = 0;
		public int _firstTimingIndex = 0;
		public int _firstFrame = 0;
		public float _audioTime = 0f;

		/// <summary>
		/// セティング
		/// </summary>
		/// <param name="stageIndex"> ステージ番号 </param>
		/// <param name="firstTimingIndex"> ステージ最初のタイミング番号 </param>
		/// <param name="audioTime"> オーディオの時間 </param>
		public void setting( int stageIndex, int firstTimingIndex, float audioTime, int frame ) {
			_stageIndex = stageIndex;
			_firstTimingIndex = firstTimingIndex;
			_audioTime = audioTime;
			_firstFrame = frame;
		}

		/// <summary>
		/// オーディオの時間取得
		/// </summary>
		/// <returns></returns>
		public float getAudioTime( ) {
			return _audioTime;
		}
		
		/// <summary>
		/// ステージはじめのフレーム取得
		/// </summary>
		/// <returns></returns>
		public int getFirstFarme( ) {
			return _firstFrame;
		}

		/// <summary>
		/// ステージ最初のインデックスを取得
		/// </summary>
		/// <returns></returns>
		public int getFirstIndex( ) {
			return _firstTimingIndex;
		}
	}
	#endregion

	public uint _farmeScale = 1000;

	[ SerializeField ]
	private Slider _slider;

	[ SerializeField ]
	private EditRhythmManager _editRhythmManager;

	[ SerializeField ]
	private Text _text;
	
	[ SerializeField ]
	private Audio _audio;

	[ SerializeField ]
	private EditFileManager _editFileManager;

	[ SerializeField ]
	private TimingManager _timingManager;

	private int _frame = 0;
	
	private Stage _stage = new Stage( );
	
	private bool _repeat = false;

	private List< TIMING_DATA > _data;

	// Use this for initialization
	void Awake( ) {
		_stage.setting( 0, _editRhythmManager.getIndex( ), _audio.getTime( ), _editRhythmManager.getFrame( ) );
	}

	bool isErorr( ) {
		bool erorr = false;

		if ( _data == null ) {
			_data = _editFileManager.getRhythmData( );
			erorr = true;
		}

		return erorr;
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		if ( isErorr( ) ) {
			return;
		}
		updateRhythmManager( );
		updateSlider( );
		updateText( );
		updateStage( );
		updateControl( );
	}

	void updateSlider( ) {
		// 最大値と最小値の設定
		_slider.minValue = 0;
		_slider.maxValue = ( _farmeScale > 0 )? _farmeScale : 1;
		
		// 値の更新
		_slider.value = _frame % _farmeScale;
	}


	void updateRhythmManager( ) {
		// 現在のフレームを取得
		_frame = _editRhythmManager.getFrame( );
	}

	void updateText( ) {
		// フレーム数の表示
		_text.text = _editRhythmManager.getFrame( ).ToString( );  
	}

	void updateControl( ) {
		// 選択中のGameObjectを取得
		GameObject target = EventSystem.current.currentSelectedGameObject;

		// 削除
		if ( Input.GetKeyDown( KeyCode.Delete ) ) {
			try {
				TimingModule module = target.GetComponent< TimingModule >( );
				_timingManager.deleteTiming( module.getIndex( ) );
			} catch {
				Debug.LogError( "Not get TimingModule Component..." );
			}
		}

		// 追加
		if ( Input.GetKeyDown( KeyCode.Space ) ) {
			_timingManager.addArray( );
		}
	}
	
	void updateStage( ) {
		// ステージインデックスの確認
		int stageIndex = _editRhythmManager.getFrame( ) / getFrameScale( );
		if ( _stage._stageIndex != stageIndex ) {
			// リピートの確認
			if ( _repeat ) {
				repeat( );
			} else {
				// 更新 
				_stage.setting( stageIndex, _editRhythmManager.getIndex( ) + _timingManager.getAddToDiff( ), _audio.getTime( ), _editRhythmManager.getFrame( ) );
			}
		}
	}

	void repeat( ) {
		_audio.setTime( _stage.getAudioTime( ) );
		_editRhythmManager.setFrame( _stage.getFirstFarme( ) );
		_editRhythmManager.setIndex( _stage.getFirstIndex( ) );
	}

	// ステージ最初のタイミング番号取得
	public int getStageFirstTimingIndex( ) {
		return _stage._firstTimingIndex;
	}

	// ステージ最初のフレーム数を取得
	public int getStageFirstFrame( ) {
		return _stage._firstFrame;
	}

	/// <summary>
	/// ステージの番号取得
	/// </summary>
	/// <returns></returns>
	public int getStageIndex( ) {
		return _stage._stageIndex;
	}

	/// <summary>
	/// タイミングデータの取得
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public TIMING_DATA getData( int index ) {
		// アクセス外の確認
		if ( index >= _data.Count  ) {
			return new TIMING_DATA( );
		}
		return _data[ index ];
	}

	/// <summary>
	/// フレームスケール
	/// </summary>
	/// <returns></returns>
	public int getFrameScale( ) {
		return ( int )_farmeScale;
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

	/// <summary>
	/// データのセーブ
	/// </summary>
	/// <param name="index"></param>
	/// <param name="frame"></param>
	public void setArrayDataFrame( int index, int frame ) {
		if ( _data.Count < index || index < 0 ) {
			return;
		}
		TIMING_DATA data = _data[ index ];
		data.frame = ( uint )frame;
		_data[ index ] = data;
	}

	/// <summary>
	/// リピートの切り替え
	/// </summary>
	public void setRepeat( ) {
		_repeat = ( _repeat )? false : true;
	}

	/// <summary>
	/// ファイルにセーブ
	/// </summary>
	public void saveFlie( ) {
		_editFileManager.saveRhythm( _data );
		_editRhythmManager.setData( _data.ToArray( ) );
		//_editRhythmManager.reloadData( );	// リズムマネージャーのデータをファイルから読み込む
		//_editFileManager.loadFile( );	// ファイルのロード
	}

	/// <summary>
	/// 配列の追加
	/// </summary>
	/// <param name="num"> 個数 </param>
	/// <param name="index"> 割り込みする配列番号 </param>
	public void addArrayData( int num, int index ) {
		for ( int i = 0; i < num; i++ ) {
			_data.Insert( index + i, new TIMING_DATA( ) );
		}
		
		// 整頓
		setOderlinessIndex( );

		// リズムマネージャーのインデックスを繰り上げ
		_editRhythmManager.addIndex( num );
	}

	public void deleteArrayDataRange( int num, int index ) {
		for ( int i = 0; i < num; i++ ) {
			_data.RemoveAt( i + index );
		}
		
		// 整頓
		setOderlinessIndex( );

		
		// リズムマネージャーのインデックスを繰り上げ
		_editRhythmManager.addIndex( -num );
	}

	/// <summary>
	/// インデックス番号の整列化
	/// </summary>
	private void setOderlinessIndex( ) {
		for ( int i = 0; i < _data.Count; i++ ) {
			TIMING_DATA data = _data[ i ];
			data.index = i;
			_data[ i ] = data;
		}

	}

	private void addStageFirstIndex( int add ) {
		_stage._firstTimingIndex += add;
	}

	public TIMING_DATA[ ] getRhythmData( ) {
		return _data.ToArray( );
	}

	/// <summary>
	/// リズムマネージャーにデータを更新させる
	/// </summary>
	public void setDataToRhythmManager( ) {
		_editRhythmManager.setData( _data.ToArray( ) );
	}
}
