using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ RequireComponent( typeof( AudioSource ) ) ]
public class AudioManager : Manager< AudioManager > {

    // SEタグ
    public enum SE {
		// 【ゲーム】
		SE_001, // 隙間風
		SE_002, // 地鳴り
		SE_003, // 銃構え
		SE_004, // 銃撃音
		SE_005,	// プレイヤーダメージ
		SE_006,	// カーソル判定音
		SE_007,	// イベント発生音
		SE_008,	// ★が入る演出
		SE_009,	// ファンファーレ１
		SE_010,	// ファンファーレ２

		// 【オブジェクト】
		SE_101,	// セグウェイ移動音
		SE_102,	// 丸岩出現音
		SE_103,	// 丸岩転がる音
		SE_104,	// 歯車音
		SE_105,	// 松明が燃える音

		// 【エネミー】
		SE_201,	// モンスター鳴き声
		SE_202,	// モンスターやられ

        MAX_SE,
		NONE,
    }

    // BGMタグ
    public enum BGM {
		BGM_1,	// 待機
		BGM_2,	// STAGE1
		BGM_3,	// STAGE2
		BGM_4,	// STAGE3 宝取得中
		BGM_5,	// STAGE3 ゲーム中
		BGM_6,	// リザルト
        MAX_BGM,
		NONE,
    }

    private enum TYPE {
        SE,
        BGM,
        MAX_TYPE,
    }

    [ SerializeField ]
    private AudioClip[ ] _audioSE = new AudioClip[ ( int )SE.MAX_SE ];
    
    [ SerializeField ]
    private AudioClip[ ] _audioBGM = new AudioClip[ ( int )BGM.MAX_BGM ];

    private AudioSource _sorce;

    /// <summary>
    /// スーパークラスのAwake関数内で実行
    /// </summary>
    protected override void initialize( ) {
        // オーディオソースの取得
        _sorce = gameObject.GetComponent< AudioSource >( );

        // BGM用オーディオにループ再生を指定
        _sorce.loop = true;
    }

	/// <summary>
	/// 1回再生
	/// </summary>
	/// <param name="tag"></param>
	private void playSE( SE tag ) {
		_sorce.PlayOneShot( _audioSE[ ( int )tag ] );
	}

	/// <summary>
	/// BGMの再生
	/// </summary>
	/// <param name="tag"></param>
	private void playBGM( BGM tag ) {
		// 2重再生の確認
		if ( _sorce.clip == _audioBGM[ ( int )tag ] ) {
			return;
		}
		_sorce.clip = _audioBGM[ ( int )tag ];
		_sorce.Play( );
	}

	/// <summary>
	/// オーディオクリップの取得（SE）
	/// </summary>
	/// <param name="tag"></param>
	/// <returns></returns>
	public AudioClip getClip( SE tag ) {
		return _audioSE[ ( int )tag ];
	}

	/// <summary>
	/// オーディオクリップの取得（BGM）
	/// </summary>
	/// <param name="tag"></param>
	/// <returns></returns>
	public AudioClip getClip( BGM tag ) {
		return _audioBGM[ ( int )tag ];
	}

	/// <summary>
	/// 再生 (SE)
	/// </summary>
	/// <param name="tag"></param>
	public void  play( SE tag ) {
		playSE( tag );
	}

	/// <summary>
	/// 再生 (BGM)
	/// </summary>
	/// <param name="tag"></param>
	public void play( BGM tag ) {
		playBGM( tag );
	}


	/// <summary>
	/// ミュートの設定
	/// </summary>
	/// <param name="enable"></param>
	public void setMute( bool enable ) {
		_sorce.mute = enabled;
	}

	/// <summary>
	/// 一時停止の切り替え
	/// </summary>
	/// <param name="enable"> true : 停止  false : 解除 </param>
	/// <param name="type"></param>
	public void pause( bool enable ) {
		if ( enable ) {
			_sorce.Pause( );	// 一時停止
		} else {
			_sorce.UnPause( );	//　一時停止の解除
		}
	}

	/// <summary>
	/// 再生の確認
	/// </summary>
	/// <returns></returns>
	public bool isPlay( ) {
		return _sorce.isPlaying;
	}

	/// <summary>
	/// 音量の設定
	/// </summary>
	/// <param name="value"></param>
	/// <param name="type"></param>
	public void setVolume( float value ) {
		_sorce.volume = value;
	}

	/// <summary>
	/// 現在の再生中のクリップの取得
	/// </summary>
	/// <returns></returns>
	public AudioClip getCurrentPlayClip( ) {
		return _sorce.clip;
	}

	/// <summary>
	/// リセッター
	/// </summary>
	public void resetter( ) {
		_sorce.mute = false;
	}
}
