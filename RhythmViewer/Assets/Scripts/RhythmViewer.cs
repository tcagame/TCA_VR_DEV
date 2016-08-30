using UnityEngine;
using System.Collections;
using Common;
using UnityEngine.UI;
using Common;

public class RhythmViewer : MonoBehaviour {

	
	#region ステージ　クラス
	[ System.Serializable ]
	private class Stage {
		public int _stageIndex = 0;
		public int _firstTimingIndex = 0;
		public float _audioTime = 0f;

		/// <summary>
		/// セティング
		/// </summary>
		/// <param name="stageIndex"> ステージ番号 </param>
		/// <param name="firstTimingIndex"> ステージ最初のタイミング番号 </param>
		/// <param name="audioTime"> オーディオの時間 </param>
		public void setting( int stageIndex, int firstTimingIndex, float audioTime ) {
			_stageIndex = stageIndex;
			_firstTimingIndex = firstTimingIndex;
			_audioTime = audioTime;
		}
	}
	#endregion

	public uint _farmeScale = 1000;

	[ SerializeField ]
	private Slider _slider;

	[ SerializeField ]
	private RhythmManager _rhythmManager;

	[ SerializeField ]
	private Text _text;
	
	[ SerializeField ]
	private Audio _audio;
	
	[ SerializeField ]
	private FileManager _fileManager;

	private int _frame = 0;
	
	private Stage _stage = new Stage( );
	

	private FILE_DATA.RHYTHM _data;

	// Use this for initialization
	void Awake( ) {
		_stage.setting( 0, _rhythmManager.getIndex( ), _audio.getTime( ) );
	}

	void Start( ) {
		_data = _fileManager.getRhythmData( FileManager.TAG.MELODY );
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		updateRhythmManager( );
		updateSlider( );
		updateText( );
		updateControl( );
		updateStage( );
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
		_frame = _rhythmManager.getFrame( );
	}

	void updateText( ) {
		// フレーム数の表示
		_text.text = _rhythmManager.getFrame( ).ToString( );  
	}

	void updateControl( ) {
		// 再生位置の取得
	}
	
	void updateStage( ) {
		// ステージインデックスの確認
		int stageIndex = _rhythmManager.getFrame( ) / getFrameScale( );
		if ( _stage._stageIndex != stageIndex ) {
			// 更新
			_stage.setting( stageIndex, _rhythmManager.getIndex( ), _audio.getTime( ) );
		}
	}

	// ステージ最初のタイミング番号取得
	public int getStageFirstTimingIndex( ) {
		return _stage._firstTimingIndex;
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
		if ( index >= _data.array.Length  ) {
			return new TIMING_DATA( );
		}
		return _data.array[ index ];
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

}
