using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimingModule : MonoBehaviour {

	// インスタンス
	private Slider _slider;			// 自身のスライダー
	private RhythmViewer _rhythmViewer;		// リズムビューワー
	private Text _text;				// テキスト

	// 変数
	private int _index = 0;				// タイミングのインデックス
	private int _originFrame = 0;		// オリジナルのフレーム
	private float _maxSliderValue = 0f;
	private float _minSliderValue = 0f;

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="frame"> フレーム数 </param>
	/// <param name="maxSliderValue"> 最大のスライダー値 </param>
	/// <param name="minSliderValue"> 最小のスライダー値 </param>
	public void initialize( int index, int frame, float maxSliderValue, float minSliderValue, RhythmViewer rhythmViewer ) {
		_index = index;
		_originFrame = frame;
		_maxSliderValue = maxSliderValue;
		_minSliderValue = minSliderValue;
		_rhythmViewer = rhythmViewer;
	}

	// Use this for initialization
	void Start( ) {
		// スライダーの取得
		_slider = GetComponent< Slider >( );
		_slider.maxValue = _maxSliderValue;
		_slider.minValue = _minSliderValue;
		_slider.value = _originFrame % _maxSliderValue;
		_slider.enabled = true;
		
		// テキストの取得
		_text = GetComponentInChildren< Text >( );
		_text.text = _originFrame.ToString( );
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		updateSlider( );
		updateText( ); 
	}

	void updateSlider( ) {
		// 最大値の更新
		_maxSliderValue = _rhythmViewer.getFrameScale( );
	}

	void updateText( ) {
		// フレーム数の表示
		_text.text = getCurrentFrame( ).ToString( );
	}

	/// <summary>
	/// 現在のフレームを取得
	/// </summary>
	/// <returns></returns>
	public int getCurrentFrame( ) {
		return Mathf.RoundToInt( _originFrame + _slider.value - _originFrame % _slider.maxValue );
	}

	/// <summary>
	/// オリジナルのフレーム取得
	/// </summary>
	/// <returns></returns>
	public int getOriginalFrame( ) {
		return _originFrame;
	}

	/// <summary>
	/// インデックスの取得
	/// </summary>
	/// <returns></returns>
	public int getIndex( ) {
		return _index;
	}
}
