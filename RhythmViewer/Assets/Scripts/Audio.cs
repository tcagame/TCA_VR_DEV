using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour {

	// エディター設定
	[ SerializeField ]
	private MODE _mode;
	[ SerializeField ]
	private bool _playOnAwake = false;
	[ SerializeField ]
	private bool _loop = false;

	// インスタンス
	[ SerializeField ]
	protected AudioSource _target;

	public bool _drawFrequency = false;

	public FFTWindow _FFTWinMode = FFTWindow.Hamming;	// フーリエ変換モード

	[ SerializeField ]
	private int _resolution = 256;	// 解像度(2のべき乗で設定)

	private bool _pause = false;	// フェーズフラグ

	protected enum MODE {
		AUDIO_3D,
		AUDIO_2D,
	}


	void Awake( ) {
		// オーディオの設定
		_target.spatialBlend = ( _mode == MODE.AUDIO_3D )? 1f : 0f; // 3D&2D
		_target.playOnAwake = _playOnAwake;

		// ループのセット
		_target.loop = _loop;

		// 継承先の初期化
		init( );
	}

	void FixedUpdate( ) {
		// 周波数の表示
		if ( _drawFrequency ) {
			float[ ] frequency = _target.GetSpectrumData( _resolution, 0, _FFTWinMode );
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
		Vector3 pos = transform.position;
		for ( int i = 1; i < data.Length / scale - 1; i++ ) {
            Debug.DrawLine(
                    new Vector3( i - 1, data[ i ] + 10, 0 ) + pos, 
                    new Vector3( i, data[ i + 1 ] + 10, 0 ) + pos, 
                    Color.red );
            Debug.DrawLine(
                    new Vector3( i - 1, Mathf.Log( data[ i - 1 ] ) + 10, 2 ) + pos, 
                    new Vector3( i, Mathf.Log( data[ i ] ) + 10, 2 ) + pos, 
                    Color.cyan );
            Debug.DrawLine(
                    new Vector3( Mathf.Log( i - 1 ), data[ i - 1 ] - 10, 1 ) + pos, 
                    new Vector3( Mathf.Log( i ), data[ i ] - 10, 1 ) + pos, 
                    Color.green );
            Debug.DrawLine(
                    new Vector3( Mathf.Log( i - 1 ), Mathf.Log( data[ i - 1 ] ), 3 ) + pos, 
                    new Vector3( Mathf.Log( i ), Mathf.Log( data[ i ] ), 3 ) + pos, 
                    Color.yellow );
        }
	}

	// 継承先でのAwake関数のかわり
	protected virtual void init( ){ }	

	/// <summary>
	/// 再生確認
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public bool isPlay( ) {
		return _target.isPlaying;
	}

	/// <summary>
	/// 再生速度の設定
	/// </summary>
	public void setPitch( float value ) {
		_target.pitch = value;
	}

	/// <summary>
	/// 音量の設定
	/// </summary>
	/// <param name="value"></param>
	/// <param name="type"></param>
	public void setVolume( float value ) {
		_target.volume = value;
	}

	/// <summary>
	/// 再生の実行
	/// </summary>
	/// <param name="index"> クリップの番号 </param>
	/// <param name="type"> 再生タイプ </param>
	public void play( ) {
		// 停止か確認
		if ( _pause ) {
			return;
		}
		// 2重再生の確認
		if ( isPlay(  ) ) {
			return;
		}
		_target.Play( );
	}

	/// <summary>
	/// 再生の停止
	/// </summary>
	/// <param name="type"></param>
	public void stop( ) {
		_target.Stop( );
	}

	/// <summary>
	/// 一時停止の切り替え（再度呼ぶことで再開）
	/// </summary>
	public void pause( ) {
		_pause = ( _pause )? false: true;  
		if ( _pause ) {
			_target.Pause( );	// 一時停止
		} else {
			_target.UnPause( );	//　一時停止の解除
		}
	}

	/// <summary>
	/// 1回再生の実行
	/// </summary>
	/// <param name="index"> クリップ番号 </param>
	/// <param name="value"> 音量 </param>
	public void playOneShot( AudioClip clip , float volume = 1f ) {
		_target.PlayOneShot( clip, volume );
	}

	/// <summary>
	/// ミュートの設定
	/// </summary>
	/// <param name="enable"></param>
	public void setMute( bool enable ) {
		_target.mute = enabled;
	}

	/// <summary>
	/// 再生時間の取得
	/// </summary>
	/// <returns></returns>
	public float getTime( ) {
		return _target.time;
	}

	/// <summary>
	///  タイムサンプルの取得
	/// </summary>
	/// <returns></returns>
	public int getTimeSamples( ) {
		return _target.timeSamples;
	}

	/// <summary>
	/// 再生時間の指定
	/// </summary>
	public void setTime( float time ) {
		_target.time = time;
	}
}
