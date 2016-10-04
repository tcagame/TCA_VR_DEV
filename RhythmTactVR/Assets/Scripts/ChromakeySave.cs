using UnityEngine;
using System.Collections;

public class ChromakeySave : MonoBehaviour {
	[ SerializeField ]SetInformChromaKey _setChromakey;
	[ SerializeField ]ScreenPosition _ScreenPosition;
	[ SerializeField ]WebCameraManager _webCamera;

	public void SaveConfig( ) {
		_setChromakey.Save( );
		_ScreenPosition.Save( );
		_webCamera.Save( );
	}
}
