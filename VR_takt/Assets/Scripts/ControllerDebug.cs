using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerDebug : MonoBehaviour {

    [SerializeField]
    private Transform[ ] _controllers;

    [SerializeField]
    private Text _textButton;

    [SerializeField]
    private Text _textValue;

    [SerializeField]
    private Text _textVibration;

    private SteamVR_TrackedObject[ ] _trackedObjs;

	// Use this for initialization
	void Start () {
		int index = _controllers.Length;
		if ( index > 0 ) {
			// 配列確保
			_trackedObjs = new SteamVR_TrackedObject[ index ];
			
			// コンポーネントの取得
			for (int i = 0; i < index; i++ ) {
				_trackedObjs[ i ] = _controllers[ i ].GetComponent< SteamVR_TrackedObject >( );
			}
		}
		
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		// 入力情報をテキストに表示
        for ( int i = 0; i < _trackedObjs.Length; i++ ) {
            updateInputButton( _trackedObjs[ i ] );		// ボタン判定
			updateInputValue( _trackedObjs[ i ] );		// 値
			updateVibration( _trackedObjs[ i ] );		// 振動
        }

	}

	void updateInputButton( SteamVR_TrackedObject target ) {
		SteamVR_Controller.Device device = SteamVR_Controller.Input( (int) target.index );

		_textButton.text = null;// テキスト初期化
		_textButton.text += "< Button >\n";	// タイトル

		
		_textButton.text += "------ One Frame ------\n";

		{	// トリガー
		
			_textButton.text += "Triiger : ";
		
			if ( device.GetTouchDown( SteamVR_Controller.ButtonMask.Trigger ) ) {
				_textButton.text += "Touch ";
			}
			if ( device.GetPressDown( SteamVR_Controller.ButtonMask.Trigger ) ) {
				_textButton.text += "Down ";
			}
			if ( device.GetTouchUp( SteamVR_Controller.ButtonMask.Trigger ) ) {
				_textButton.text += "Up ";
			}
			
			_textButton.text += "\n";
		}
        
		{// タッチパッド（ホールド）
			
			_textButton.text += "Tuchpad( hold ) : ";
			
			if ( device.GetPressDown( SteamVR_Controller.ButtonMask.Touchpad ) ) {
				_textButton.text += "Down ";
			}
			if ( device.GetPress( SteamVR_Controller.ButtonMask.Touchpad ) ) {
				_textButton.text += "Touch ";
			}
			if ( device.GetPressUp( SteamVR_Controller.ButtonMask.Touchpad ) ) {
				_textButton.text += "Up ";
			}
			
			_textButton.text += "\n";
		}
		
		
		{// タッチパッド
			
			_textButton.text += "Tuchpad : ";
		
			if ( device.GetTouchDown( SteamVR_Controller.ButtonMask.Touchpad ) ) {
				_textButton.text += "Down ";
			}
			if ( device.GetTouchUp( SteamVR_Controller.ButtonMask.Touchpad ) ) {
				_textButton.text += "Up ";
			}
			
			_textButton.text += "\n";
		}
		
		{// メニューボタン
		
			_textButton.text += "Menu : ";
		
			if ( device.GetPressDown( SteamVR_Controller.ButtonMask.ApplicationMenu ) ) {
				_textButton.text += "Down ";
			}
			
			_textButton.text += "\n";
		}
        
		{// グリップボタン
			_textButton.text += "Grip : ";
		
			if ( device.GetPressDown( SteamVR_Controller.ButtonMask.Grip ) ) {
				_textButton.text += "Down ";
			}
			
			_textButton.text += "\n";
		}
		
		{// 毎フレーム判定
			_textButton.text += "------ Every Frame ------\n";

			{//トリガー　※押し込み具合が深い時にも、浅くの判定もとってしてしまう
				_textButton.text += "Trigger : ";
				if ( device.GetTouch( SteamVR_Controller.ButtonMask.Trigger ) ) {
					_textButton.text += "Touch ";
				}
				if ( device.GetPress( SteamVR_Controller.ButtonMask.Trigger ) ) {
					_textButton.text += "Down ";
				}
				_textButton.text += "\n";
			}

			{// タッチパッド
				_textButton.text += "Touchpad : ";
				if ( device.GetTouch( SteamVR_Controller.ButtonMask.Touchpad ) ) {
					_textButton.text += "Touch";
				}
				_textButton.text += "\n";
			}
		}
	}

	void updateInputValue( SteamVR_TrackedObject target ) {
		SteamVR_Controller.Device device = SteamVR_Controller.Input( (int) target.index );

		_textValue.text = null;
		_textValue.text += "< Value >\n";	// タイトル

		{// トリガー
			Vector2 vec = device.GetAxis( Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger );
			_textValue.text += "Trigger : ";
			_textValue.text += "x -> " + vec.x + " " + "y -> " + vec.y;
			_textValue.text += "\n";
		}

		{// タッチパッド
			Vector2 vec = device.GetAxis( );
			//Vector2 vec = device.GetAxis( Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad );
			_textValue.text += "Touchpad : ";
			_textValue.text += "x -> " + vec.x + " " + "y -> " + vec.y;
			_textValue.text += "\n";

		}
	}

	/// <summary>
	/// 振動の更新
	/// </summary>
	/// <param name="target"></param>
	void updateVibration( SteamVR_TrackedObject target ) {
		SteamVR_Controller.Device device = SteamVR_Controller.Input( ( int )target.index );

		// 振動
		const ushort MAX = 2000;
		ushort value = ( ushort )( device.GetAxis( Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger ).x * MAX );
		device.TriggerHapticPulse( value );

		// テキスト
		_textVibration.text = null;
		_textVibration.text += "< Vibration >\n";	// タイトル
		_textVibration.text += "value : " + value;
	}
}
