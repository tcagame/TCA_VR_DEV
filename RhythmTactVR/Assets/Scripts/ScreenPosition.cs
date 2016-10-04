using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenPosition : MonoBehaviour {
	[ SerializeField ] GameObject _ScreenBase;
	[ SerializeField ] Slider[ ] _Sliders;
	float[ ] _nowScreenStatus;

	// Use this for initialization
	void Start () {
		_nowScreenStatus = new float[ _Sliders.Length ];
		for ( int i = 0; i < _Sliders.Length; i++ ) {
			_Sliders[ i ].value = PlayerPrefs.GetFloat( _Sliders[ i ].name );
		}
		ScreenStatusUpdate( );
	}

	//  スクリーンのステータスが変更されているか確認.
	bool CheckSliderUpdate( ) {
		for ( int i = 0; i < _Sliders.Length; i++ ) {
			if ( _nowScreenStatus[ i ] != _Sliders[ i ].value ) {
				return true;
			}
		}
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		if ( CheckSliderUpdate( ) ) {
			ScreenStatusUpdate( );
		}
	}

	// スクリーンのステータスを更新.
	void ScreenStatusUpdate( ) {
		_ScreenBase.transform.localPosition = new Vector3( _Sliders[ 0 ].value * 0.000001f, _Sliders[ 1 ].value * 0.00001f, _Sliders[ 2 ].value * 0.00001f + 0.01f );
		_ScreenBase.transform.localScale = new Vector3( _Sliders[ 3 ].value * 0.00001f, 1, _Sliders[ 4 ].value * 0.00001f );
	}


	//  アプリケーション終了時に設定を保存.
	public void Save( ) {
		for ( int i = 0; i < _Sliders.Length; i++ ) {
			PlayerPrefs.SetFloat( _Sliders[ i ].name, _Sliders[ i ].value );
		}
	}
}
