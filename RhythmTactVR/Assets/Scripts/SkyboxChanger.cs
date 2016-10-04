using UnityEngine;
using System.Collections;
using Common;

public class SkyboxChanger : MonoBehaviour {

	#region Order Info Class
	[ System.Serializable ]
	private class OrderInfo {
		[ SerializeField ]
		private Color _color;

		private bool _active = false;

		public Color getColor( ) {
			return _color;
		}

		public bool isActive( ) {
			return _active;
		}

		public void setActive( bool enable ) {
			_active = enable;
		}
	}
	#endregion

	[ SerializeField ]
	private ModeManager _modeManager;	// モードマネージャー.

	[ SerializeField ]
    private Material _skybox;	// マテリアル（スカイボックス）.

	[ SerializeField ]
	private OrderInfo[ ] _orders = new OrderInfo[ 2 ];		// タグ実行の順番		

	public Color _currentColor = Color.white;	// 色.

	public uint _time = 200;

	public uint _wakeUpTime = 300;

	private bool _animation = false;	// アニメーションフラグ.
	private int _currentTime = 0;		// 現在の時間.
	private Color _curentColor;			// 現在の色.
	private int _rotationRatio = 0;	// 回転の割合.
	private float _expouse = 0f;	// 感光度.
	private int _tableIndex = 0;

	private bool _wakeUp = false;
	private int _currentWakeUp = 0;
	private float _originExpouse = 0f;

	// 定数
	private const int MAX_ROTATION = 360;	// 最大回転値.

	private const float MIN_EXPOSURE = 0.005f;	// 最小の感光.
	private const float MAX_EXPOSURE = 0.4f;	// 最大の感光.
	private const float FINISH_EXPOSURE = 1f;	// 最終時の最大感光.

	// 列挙型.
	private enum TAG {
		COLOR,
		EXPOSURE,
		ROTATION,
		TEXTURE,
		MAX_TAG,
	}

	// プロパティーネーム.
	private readonly string[ ] PROPERTY_NAMES = new string[ 4 ] {
		"_Tint",		// Color
		"_Exposure",	// Range
		"_Rotation",	// Range
		"_Tex"			// Cube map texture(HDR)
	};

    void Start( ) {
		// 生成.
		_skybox = Instantiate( _skybox ) as Material;

		//初期値の設定
		_rotationRatio = _skybox.GetInt( PROPERTY_NAMES[ ( int )TAG.ROTATION ] );
		_expouse = _skybox.GetFloat( PROPERTY_NAMES[ ( int )TAG.EXPOSURE ] );

		// 初期のいろをセット.
		_skybox.SetColor( PROPERTY_NAMES[ ( int )TAG.COLOR ], _currentColor );

		// スカイボックスを適用.
        RenderSettings.skybox = _skybox;

		// 感光のスタート
		playWakeUp( );
    }

    void FixedUpdate( ) {
		Color[ ] table = {
			Color.red,
			Color.green,
			Color.blue,
			Color.cyan,
			Color.yellow,
		};
		// デバッグ
		if ( Input.GetKeyDown( KeyCode.F6 ) ) {
			play( table[ Random.Range( 0, table.Length ) ] );
		}

		// 感光
		if ( Input.GetKeyDown( KeyCode.F7 ) ) {
			playWakeUp( );
		}

		// タイミングでスカイボックスの切り替え（モードチェンジ）.
		if ( _modeManager.getMusicMode( ) < MUSIC_MODE.MODE_B_PART ) {
			// first 
			if ( !_orders[ 0 ].isActive( ) ) {
				_orders[ 0 ].setActive( true );
				play( _orders[ 0 ].getColor( ) );
			}
		} else {
			// second
			if ( !_orders[ 1 ].isActive( ) ) {
				_orders[ 1 ].setActive( true );
				play( _orders[ 1 ].getColor( ) );
			}
		}

		// アニメーションの更新.
		updateAniamtion( );

		// 回転の更新
		updateRotation( );

		// 感光の更新
		updateExposure( );
	}

	/// <summary>
	/// アニメーションの更新.
	/// </summary>
	void updateAniamtion( ) {
		// 実行確認.
		if ( !_animation ) {
			return;
		}

		// 時間経過.
		if ( _currentTime > 0 ) {
			_currentTime--;
		} else {
			_skybox.SetColor( PROPERTY_NAMES[ ( int )TAG.COLOR ], _currentColor );	// 最終の色をセット.
			_animation = false;	// 終了.
		}

		// 色の変化.
		_skybox.SetColor( PROPERTY_NAMES[ ( int )TAG.COLOR ], _curentColor + ( _currentColor - _curentColor ) * ( _time - _currentTime ) / ( float )_time );
	} 

	void updateRotation( ) {
		_skybox.SetInt( PROPERTY_NAMES[ ( int )TAG.ROTATION ], _rotationRatio );
	}

	void updateExposure( ) {
		updateWakeUp( );
		_skybox.SetFloat( PROPERTY_NAMES[ ( int )TAG.EXPOSURE ], _expouse );
	}

	void updateWakeUp( ) {
		if ( !_wakeUp ) {
			return;
		}

		// 時間経過
		if ( _currentWakeUp > 0 ) {
			_currentWakeUp--;
		} else {
			_wakeUp = false;
			return;
		}

		// 感光
		setExposure( _originExpouse + ( MAX_EXPOSURE - MIN_EXPOSURE ) * ( _wakeUpTime - _currentWakeUp ) / ( float )_wakeUpTime );
	}

	public void play( Color color ) {
		// 再生確認.
		if ( _animation ) {
			return;
		}
		// 設定.
		_animation = true;
		_currentTime = ( int )_time;
		_currentColor = color;
		_curentColor = _skybox.GetColor( PROPERTY_NAMES[ ( int )TAG.COLOR ] );	// 現在の色を取得.
	}

	public void playWakeUp( ) {
		if ( _wakeUp ) {
			return;
		}
		setExposure( MIN_EXPOSURE );	// 感光の初期化
		_currentWakeUp = ( int )_wakeUpTime;
		_originExpouse = _expouse;
		_wakeUp = true;
	}

	/// <summary>
	/// ローテンションを設定.
	/// </summary>
	/// <param name="ratio"></param>
	public void setRotaion( int ratio ) {
		_rotationRatio = ( ratio > 360 )? 360 : ( ratio < 0 )? 0: ratio;
	}

	/// <summary>
	/// 感光のセット.
	/// </summary>
	/// <param name="ratio"></param>
	public void setExposure( float ratio ) {
		_expouse = ( ratio > MAX_EXPOSURE )? MAX_EXPOSURE : ( ratio < MIN_EXPOSURE )? MIN_EXPOSURE : ratio;
	}
}
