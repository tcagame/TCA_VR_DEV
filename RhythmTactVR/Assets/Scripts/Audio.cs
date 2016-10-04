using UnityEngine;
using System.Collections;

[ RequireComponent( typeof( AudioSource ) ) ]
public class Audio : MonoBehaviour {

	// 変数
	protected AudioSource _audioSouce;

	void Awake( ) {
		// オーディオソースの取得
		_audioSouce = GetComponent< AudioSource >( );

		// ループのセット
		_audioSouce.loop = true;

		// 継承先の初期化
		init( );
	}

	// 継承先でのAwake関数のかわり
	protected virtual void init( ){ }	

	/// <summary>
	/// 再生確認
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public bool isPlay( ) {
		return _audioSouce.isPlaying;
	}

	/// <summary>
	/// 再生速度の設定
	/// </summary>
	public void setPitch( float value ) {
		_audioSouce.pitch = value;
	}

	/// <summary>
	/// 音量の設定
	/// </summary>
	/// <param name="value"></param>
	/// <param name="type"></param>
	public void setVolume( float value ) {
		_audioSouce.volume = value;
	}

	/// <summary>
	/// 再生の実行
	/// </summary>
	/// <param name="index"> クリップの番号 </param>
	/// <param name="type"> 再生タイプ </param>
	public virtual void play( AudioClip clip ) {
		_audioSouce.clip = clip;
		_audioSouce.Play( );
	}

	/// <summary>
	/// 再生の停止
	/// </summary>
	/// <param name="type"></param>
	public void stop( ) {
		_audioSouce.Stop( );
	}

	/// <summary>
	/// 一時停止の切り替え
	/// </summary>
	/// <param name="enable"> true : 停止  false : 解除 </param>
	/// <param name="type"></param>
	public void pause( bool enable ) {
		if ( enable ) {
			_audioSouce.Pause( );	// 一時停止
		} else {
			_audioSouce.UnPause( );	//　一時停止の解除
		}
	}

	/// <summary>
	/// 1回再生の実行
	/// </summary>
	/// <param name="index"> クリップ番号 </param>
	/// <param name="value"> 音量 </param>
	public void playOneShot( AudioClip clip , float volume = 1f ) {
		_audioSouce.PlayOneShot( clip, volume );
	}

	/// <summary>
	/// ミュートの設定
	/// </summary>
	/// <param name="enable"></param>
	public void setMute( bool enable ) {
		_audioSouce.mute = enabled;
	}
}
