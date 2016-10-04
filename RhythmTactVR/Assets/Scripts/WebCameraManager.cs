using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WebCameraManager : MonoBehaviour {
	[ SerializeField ] GameObject _prefab;
	[ SerializeField ] GameObject _chromakey;
	[ SerializeField ] GameObject _content;
	[ SerializeField ] GameObject _parentChromakey;
	[ SerializeField ] ToggleGroup _toggleGroup;
	[ SerializeField ] SetInformChromaKey _setChromakey;
	private GameObject[ ] _chromakeyScreen;
	private WebCamTexture[ ] _webCamTexs;
	private Toggle[ ] _toggles;
	private string[ ] _cameraNames;
	private string _cameraId = "CameraNumber";
	private int _nowCamera;

	// Use this for initialization
	void Start () {
		WebCamDevice[ ] devices = WebCamTexture.devices;
		_chromakeyScreen = new GameObject[ devices.Length ];
		GameObject chromakey;
		_cameraNames = new string[ devices.Length ];
		for ( int i = 0; i < devices.Length; i++ ) {
			_cameraNames[ i ] = devices[ i ].name;
			chromakey =  Instantiate ( _chromakey, new Vector3( 0, 0, 0 ), Quaternion.identity) as GameObject;
			chromakey.name = _cameraNames[ i ];
			chromakey.transform.SetParent( _parentChromakey.transform, false );
			_chromakeyScreen[ i ] = chromakey;
			_chromakeyScreen[ i ].SetActive( false );  
        }
		_toggles = new Toggle[ _cameraNames.Length ];
		ScrollBarSetUp( );
		_nowCamera = PlayerPrefs.GetInt( _cameraId );
		_toggles[ _nowCamera ].isOn = true;
		_chromakeyScreen[ _nowCamera ].SetActive( true );
		_setChromakey.UpdateChromakey( );
	}

	// Update is called once per frame
	void Update () {
		if ( !_toggles[ _nowCamera ].isOn ) {
			ChengeWebCamera( );
		}
	}

	void ScrollBarSetUp( ) {
		GameObject go; 
		for ( int i = 0; i < _cameraNames.Length; i++ ) {
			go = Instantiate ( _prefab, new Vector3( 0, 0, 0 ), Quaternion.identity) as GameObject;
			go.name = _cameraNames[ i ];
			go.GetComponentInChildren<Text>( ).text = _cameraNames[ i ];
			_toggles[ i ] = go.GetComponentInChildren<Toggle>( );
			_toggles[ i ].isOn = false;
			_toggles[ i ].group = _toggleGroup;
			go.transform.SetParent( _content.transform, false );
		}
	}

	void ChengeWebCamera( ) {
		_chromakeyScreen[ _nowCamera ].SetActive( false );
		for ( int i = 0; i < _cameraNames.Length; i++ ) {
			if ( _toggles[ i ].isOn ) {
				_nowCamera = i;
				_chromakeyScreen[ _nowCamera ].SetActive( true );
			}
		}
	}

	public int GetNowCamera( ) {
		return _nowCamera;
	}

	public void Save( ) {
		PlayerPrefs.SetInt( _cameraId, _nowCamera );
	}
}
