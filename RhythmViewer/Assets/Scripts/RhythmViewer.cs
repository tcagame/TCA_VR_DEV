using UnityEngine;
using System.Collections;
using Common;
using UnityEngine.UI;

public class RhythmViewer : MonoBehaviour {

	public uint _farmeScale = 1000;

	[ SerializeField ]
	private Slider _slider;

	[ SerializeField ]
	private RhythmManager _rhythmManager;

	[ SerializeField ]
	private Text _text;

	private int _frame = 0;


	// Use this for initialization
	void Awake( ) {

	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		updateRhythmManager( );
		updateSlider( );
		updateText( );
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
		_frame = _rhythmManager.getFrame( );
	}

	void updateText( ) {
		// フレーム数の表示
		_text.text = _rhythmManager.getFrame( ).ToString( );  
	}

	void updateControl( ) {
		// 再生位置の取得
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
