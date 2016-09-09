using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaseLine : MonoBehaviour {

	public float _length = 20f;	// ライン長さ

	private LineRenderer _line;

	// Use this for initialization
	void Awake( ) {
		_line = GetComponent< LineRenderer >( );
		setting( );
	}

	void setting( ) {
		// 頂点設定
		_line .SetVertexCount( 2 );
		// 位置の指定
		_line.SetPosition( 1, Vector3.right * _length );
	}
}
