using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TempTexture : MonoBehaviour {

	[ SerializeField ]
	private DisplayWebCamera _target;

	private Texture2D _texture;
	private Image _image;
	private Sprite _texture_sprite;

	// Use this for initialization
	void Start () {
		_texture = ( Texture2D )_target.GetComponent< MeshRenderer >( ).material.mainTexture;
		_image = GetComponent< Image >( );
	}
	
	// Update is called once per frame
	void Update () {
		//if ( !_texture ) {
		//	Renderer rend =  _target.GetComponent< Renderer >( );
		//	_texture = _target.GetComponent< Renderer >( ).material.mainTexture as Texture2D; //( Texture2D )_target.GetComponent< MeshRenderer >( ).material.mainTexture; 
		//	// Texture -> Spriteに変換する
		//	_texture_sprite = Sprite.Create( _texture, new Rect(0,0,256,256), Vector2.zero);
		//	return;
		//}
		//_image.sprite = _texture_sprite;
		//WebCamTexture tex = _target.getWebCamTexture( );
		//_image.sprite = tex as Sprite;
	}
}
