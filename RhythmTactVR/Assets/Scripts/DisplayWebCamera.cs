using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayWebCamera : MonoBehaviour {
	 public Vector2 scrollPosition = Vector2.zero;
	[ SerializeField ] private int _CameraWidth = 1920;
	[ SerializeField ] private int _CameraHeight = 1080;
	[ SerializeField ] private int _CameraFPS = 60;
	private WebCameraManager _webCameraManager;
	private WebCamTexture _webCamTex;
	WebCamTexture _webCamTexture;

	void Start ( ) {
		_webCameraManager = GameObject.Find( "WebCameraManager" ).GetComponent<WebCameraManager>( );
	    WebCamDevice[ ] devices = WebCamTexture.devices;
        WebCamTexture webcamTexture = new WebCamTexture(devices[ _webCameraManager.GetNowCamera( ) ].name, _CameraWidth, _CameraHeight, _CameraFPS);
        GetComponent<Renderer> ( ).material.mainTexture = webcamTexture;
        webcamTexture.Play( );
	}
}