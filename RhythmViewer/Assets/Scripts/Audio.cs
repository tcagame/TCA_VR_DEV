using UnityEngine;
using System.Collections;

[ RequireComponent( typeof( AudioSource ), typeof( AudioReverbZone ) ) ]
public class Audio : MonoBehaviour {

	// エディター設定
	[ SerializeField ]
	private MODE _mode;

	protected enum MODE {
		AUDIO_3D,
		AUDIO_2D,
	}

	// 変数
	protected AudioSource _audioSouce;

	// 定数
	protected const float MIN_NOISE_DISTANCE = 1f;	// 音の最小距離
	protected const float MAX_NOISE_DISTANCE = 80f;	// 音の最大距離
	protected const AudioRolloffMode _rolloffMode = AudioRolloffMode.Logarithmic;	// 音の減衰モード
	protected const float MIN_REVERBERATION_DISTANCE = 15f;	// 反響音（最小距離）
	protected const float MAX_REVERBERATION_DISTANCE = 1000f;	// 反響音（最大距離）
	protected const AudioReverbPreset _zoneType = AudioReverbPreset.StoneCorridor;	// 環境タイプ

	void Awake( ) {
		// オーディオソースの取得
		_audioSouce = GetComponent< AudioSource >( );

		// オーディオの設定
		_audioSouce.spatialBlend = ( _mode == MODE.AUDIO_3D )? 1f : 0f; // 3D&2D
		_audioSouce.minDistance = MIN_NOISE_DISTANCE;		
		_audioSouce.maxDistance = MAX_NOISE_DISTANCE;
		_audioSouce.rolloffMode = _rolloffMode;		// 減数モード
		_audioSouce.playOnAwake = false;

		// ゾーンの設定
		AudioReverbZone zone = GetComponent< AudioReverbZone >( );
		zone.minDistance = MIN_REVERBERATION_DISTANCE;
		zone.maxDistance = MAX_REVERBERATION_DISTANCE;
		zone.reverbPreset = _zoneType;

		// ループのセット
		_audioSouce.loop = true;

		// 継承先の初期化
		init( );
	}

	// 継承先でのAwake関数のかわり
	protected virtual void init( ){ }	

	/// <summary>
	/// オーディオソースにセットされているクリップのが等しい確認
	/// </summary>
	/// <param name="type"> タイプ </param>
	/// <param name="index"> クリップ番号 </param>
	/// <returns></returns>
    public bool isClip( int index ) {
        return getClip( index ) == _audioSouce.clip;
    }

	/// <summary>
	/// クリップの取得
	/// </summary>
	/// <param name="index"> クリップの番号 </param>
	/// <returns></returns>
    public AudioClip getClip( int index ) {
        return AudioManager.getInstance( ).getClip( ( AudioManager.SE )index );
    }

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
	public void play( int index ) {
		// 2重再生の確認
		if ( isPlay(  ) && _audioSouce.clip == getClip( index ) ) {
			return;
		}
		_audioSouce.clip = getClip( index );
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
	public void playOneShot( int index , float volume = 1f ) {
		_audioSouce.PlayOneShot( getClip( index ), volume );
	}

	/// <summary>
	/// ミュートの設定
	/// </summary>
	/// <param name="enable"></param>
	public void setMute( bool enable ) {
		_audioSouce.mute = enabled;
	}
}
