using UnityEngine;
using System.Collections;

public class AudioProduction : Audio {

	public enum TAG {
		NONE,
		NO_1,
		NO_2,
		NO_3,
		MAX_TAG,
		RANDOM,
	}

	#region オーディオ 情報
	[ System.Serializable ]
	private class AudioInfo {

		[ SerializeField ]
		private TAG _tag;

		[ SerializeField ]
		AudioClip _clip;

		public TAG getTag( ) {
			return _tag;
		}
		
		public AudioClip getAudioClip( ) {
			return _clip;
		}
	}
	#endregion

	[ SerializeField ]
	private TAG _awakeStartTag = TAG.RANDOM;

	[ SerializeField ]
	private uint _wakeUpTime = 400;	// 立ち上がりの時間

	[ SerializeField ]
	private uint _sleepTime = 400;	// 立ちさがりの時間

	[ SerializeField ]
	AudioInfo[ ] _audioInfo;		// オーディオ情報配列

	private int _curentTime = 0;
	private bool _wakeUp = false;
	private bool _sleep = false;
	private TAG _curentTag;

	protected override void init( ) {
		// 再生
		play( _awakeStartTag );

		// 起き上がり演出
		playWakeUp( );	
	}  

	// Use this for initialization
	void Start( ) {
	
	}

	void Update( ) {
		if ( Input.GetKeyDown( KeyCode.F9 ) ) {
			playWakeUp( );
		}
		if ( Input.GetKeyDown( KeyCode.F10 ) ) {
			playSleep( );
		}
		if ( Input.GetKeyDown( KeyCode.F11 ) ) {
			play( TAG.RANDOM );
		}
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		updateWakeUp( );
		updateSleep( );
	}

	void updateWakeUp( ) {
		if ( !_wakeUp ) {
			return;
		}

		// 時間経過
		if ( _curentTime > 0 ) {
			_curentTime--;
		} else {
			_wakeUp = false;
		}

		// 音量上げ
		setVolume( ( _wakeUpTime - _curentTime ) / ( float )_wakeUpTime );
	}

	void updateSleep( ) {
		if ( !_sleep ) {
			return;
		}

		// 時間経過.s
		if ( _curentTime > 0 ) {
			_curentTime--;
		} else {
			stop( );	// 再生停止
			_sleep = false;
		}

		// 音量下げ.
		setVolume( _curentTime / ( float )_sleepTime );

	}
	
	private AudioClip getClip( TAG tag ) {
		for ( int i = 0;  i < _audioInfo.Length; i++ )  {
			if ( _audioInfo[ i ].getTag( ) == tag )   {
				return _audioInfo[ i ].getAudioClip( );
			}
		}
		return null;
	}
 
	public void playWakeUp( ) {
		// 演出実行の確認.
		if ( !_wakeUp && !_sleep ) {
			if ( !isPlay( ) ) {
				play( getClip( _curentTag ) );
			}
			_wakeUp = true;
			_curentTime = ( int )_wakeUpTime;
		}
	}

	public void playSleep( ) {
		// 演出実行の確認.
		if ( !_sleep && !_wakeUp ) {
			_sleep = true;
			_curentTime = ( int )_sleepTime;
		}
	}

	private void setTag( TAG tag ) {
		_curentTag = tag;
	}

	/// <summary>
	/// 再生
	/// </summary>
	/// <param name="tag"></param>
	public new void play( TAG tag ) {

		// タグがランダムに設定されているか確認.
		if ( tag == TAG.RANDOM ) {
			tag = ( TAG )Random.Range( ( int )TAG.NO_1, ( int )TAG.MAX_TAG );	// ランダム選択
		}

		// タグのセット
		setTag( tag );

		// 再生
		base.play( getClip( tag ) );
	}
}
