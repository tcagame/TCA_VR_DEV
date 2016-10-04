using UnityEngine;
using System.Collections;

public class GetInformChromakey : MonoBehaviour {
	private SetInformChromaKey _setInformCk;
	private float[ ] _informChromakey;
 	private Material _material;

	// Use this for initialization
	void Start () {
		_informChromakey = new float[ 9 ];
		_material = gameObject.GetComponent<Renderer>().material;
		_setInformCk = GameObject.Find( "ChromakeyManager" ).GetComponent<SetInformChromaKey> ( );
	}

	void Update ( ) {
		if ( _setInformCk.GetSetFlag( ) ) {
			_informChromakey = _setInformCk.GetChromakeyInform( );
			SetChromakey( );
		}
	}

	//  Chromakeyに変化した設定をセット.
	void SetChromakey( ) {
		_material.SetFloat( "_MaxHue", _informChromakey[ 6 ] + _informChromakey[ 0 ] );
		_material.SetFloat( "_MinHue", _informChromakey[ 6 ] - _informChromakey[ 1 ] );
		_material.SetFloat( "_MaxSaturation", _informChromakey[ 7 ] + _informChromakey[ 2 ] );
		_material.SetFloat( "_MinSaturation", _informChromakey[ 7 ] - _informChromakey[ 3 ] );
		_material.SetFloat( "_MaxValue", _informChromakey[ 8 ] + _informChromakey[ 4 ] );
		_material.SetFloat( "_MinValue", _informChromakey[ 8 ] - _informChromakey[ 5 ] );
	}
}
