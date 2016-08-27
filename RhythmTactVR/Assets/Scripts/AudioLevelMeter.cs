using UnityEngine;
using System.Collections;

[ RequireComponent( typeof( AudioSource ) ) ]
public class AudioLevelMeter : MonoBehaviour {
	
	//-----------------------------------------------//
	// エディター設定
	//-----------------------------------------------//

	// 解像度
	[ SerializeField ]
	private int _resolution = 1024;

	// 周波数域のしきい値
    public float _lowFreqThreshold = 14700f;
	public float _midFreqThreshold = 29400f;
	public float _highFreqThreshold = 44100f;

	// 数値の倍率
    public float _lowEnhance = 1f;
	public float _midEnhance = 10f;
	public float _highEnhance = 100f;

	// デバッグ
	public bool _debug = false;
	
	[ SerializeField ]
	private Transform _lowMeter;
	[ SerializeField ]
	private Transform _midMeter;
	[ SerializeField ]
	private Transform _highMeter;

	//-----------------------------------------------//

	// コンポーネント
    private AudioSource _source;

	// 変数
    private float _low = 0f;
	private float _mid = 0f;
	private float _high = 0f;
	private float[ ] _freqData;	// 周波数データ

    void Awake( ) {
		// 配列確保
		_freqData = new float[ _resolution ];

		// コンポーネント取得
        _source = GetComponent< AudioSource >( );
    }

    void FixedUpdate( ) {
		// データ取得
        _freqData = _source.GetSpectrumData( _resolution, 0, FFTWindow.Hamming );
        
		// 平均の周波数
        int deltaFreq = AudioSettings.outputSampleRate / _resolution;	
        

		// 周波数域で数値の加算
		_low = 0f;
        _mid = 0f;
        _high = 0f;
        for ( int i = 0; i < _resolution; i++ ) {
            int freq = deltaFreq * i;
            if ( freq <= _lowFreqThreshold ) {
				_low  += _freqData[ i ];
			} else if ( freq <= _midFreqThreshold ) {
				_mid  += _freqData[ i ];
			} else if ( freq <= _highFreqThreshold ) {
				_high += _freqData[ i ];
			}
        }

		// 数値を高める
        _low  *= _lowEnhance;
        _mid  *= _midEnhance;
        _high *= _highEnhance;

		// デバッグ
		if ( _debug ) {
			DrawLineGraph( _low, _mid, _high, ref _freqData ); //線グラフの表示
		}
    }

	/// <summary>
	/// 線グラフの表示
	/// </summary>
	/// <param name="low"> 低音 </param>
	/// <param name="mid"> 中音   </param>
	/// <param name="high"> 高音  </param>
	/// <param name="spectrum"> 周波数データ </param>
	void DrawLineGraph( float low, float mid, float high, ref float[ ] data ) { 
		// ログ
		//Debug.Log( "Low : " + low + " Mid : " + mid + " High : " + high );
		
		// スケール
		if ( _lowMeter ) {
			_lowMeter.localScale  = new Vector3( _lowMeter.localScale.x,  low,  _lowMeter.localScale.z );
		}
		if ( _midMeter ) {
			_midMeter.localScale  = new Vector3( _midMeter.localScale.x,  mid,  _midMeter.localScale.z );
		}
		if ( _highMeter ) {
			_highMeter.localScale = new Vector3( _highMeter.localScale.x, high, _highMeter.localScale.z );
		}

		int scale = 4;	// 表示スケール
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
