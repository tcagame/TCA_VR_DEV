using UnityEngine;
using System.Collections;

public class AudioChannel : MonoBehaviour {
	
	[ SerializeField ]
	private AudioProduction _production;

	public void setMusic1( ) {
		_production.play( AudioProduction.TAG.NO_1 );
	}
	
	public void setMusic2( ) {
		_production.play( AudioProduction.TAG.NO_2 );
	}
	
	public void setMusic3( ) {
		_production.play( AudioProduction.TAG.NO_3 );
	}

	public void setMusicNone( ) {
		_production.play( AudioProduction.TAG.NONE );
	}
}
