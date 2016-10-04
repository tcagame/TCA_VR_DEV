using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using uCPf;

public class SetInformChromaKey : MonoBehaviour {
	[ SerializeField ] private Slider[ ] Sliders; 
	[ SerializeField ]private ColorPicker _colorPicker;
	private Color _NowColor;
	private float[ ] _MargingHSV;
	[ HideInInspector ]private bool _setChromakeyFlag = false;
	private bool firstFlag = false;

	// Use this for initialization
	void Start () {
		_MargingHSV = new float[ 6 ];
		_colorPicker.hsv = new Color ( PlayerPrefs.GetFloat( "ColorPickerHue" ), PlayerPrefs.GetFloat( "ColorPickerSaturation" ),  PlayerPrefs.GetFloat( "ColorPickerValue" ), 1 );
		for ( int i = 0; i < _MargingHSV.Length; i++ ) {
			Sliders[ i ].value = PlayerPrefs.GetFloat( Sliders[ i ].name );
			_MargingHSV[ i ] = Sliders[ i ].value;
		}
	}
	
	// ColorPickerの設定に変化があったかを確認.
	bool CheckColorPicker( Color colorPicker ) {
		if ( _NowColor.r != colorPicker.r ) {
			return true;
		} else if ( _NowColor.g != colorPicker.g ) {
			return true;
		} else if ( _NowColor.b != colorPicker.b ) {
			return true;
		}
		return false;
	}

	//  Margingに変化があったかを確認.
	bool CheckMarginChecker( ) {
		for ( int i = 0; i < _MargingHSV.Length; i++ ) {
			if ( _MargingHSV[ i ] != Sliders[ i ].value ) {
				return true;
			}
		}
		return false;
	}

	public bool GetSetFlag( ) {
		return _setChromakeyFlag;
	}

	//  クロマキー合成に必要な情報の取得.
	public float[ ] GetChromakeyInform( ) {
		float[ ] ChromakeyInform = new float[ 9 ];
		for ( int i = 0; i < _MargingHSV.Length; i++ ) {
			ChromakeyInform[ i ] = _MargingHSV[ i ];
		}
		ChromakeyInform[ 6 ] =  _colorPicker.hsv.h;
		ChromakeyInform[ 7 ] =  _colorPicker.hsv.s;
		ChromakeyInform[ 8 ] =  _colorPicker.hsv.v;
		_setChromakeyFlag = false;
		return  ChromakeyInform;
	}

	// Update is called once per frame
	void Update ( ) {
		if ( CheckColorPicker( _colorPicker.color ) || CheckMarginChecker(  ) ) {
			UpdateChromakey( );
		}
		if ( !firstFlag ) {
			UpdateChromakey( );
			firstFlag  = true;
		}
	}
	
	public void UpdateChromakey( ) {
					Debug.Log( "Active" );
		_NowColor = _colorPicker.color;
		for ( int i= 0; i< _MargingHSV.Length; i++ ) {
				_MargingHSV[ i ] = Sliders[ i ].value;
		}
		_setChromakeyFlag = true;
	}

	//  セーブ処理.
	public void Save( ) {
		for ( int i = 0; i < Sliders.Length; i++ ) {
			PlayerPrefs.SetFloat( Sliders[ i ].name, _MargingHSV[ i ] );
		}
		PlayerPrefs.SetFloat( "ColorPickerHue", _colorPicker.hsv.h );
		PlayerPrefs.SetFloat( "ColorPickeSaturation", _colorPicker.hsv.s );
		PlayerPrefs.SetFloat( "ColorPickerValue", _colorPicker.hsv.v );
	}
}