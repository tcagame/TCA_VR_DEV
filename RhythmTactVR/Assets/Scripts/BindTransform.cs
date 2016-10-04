using UnityEngine;
using System.Collections;

public class BindTransform : MonoBehaviour {
	[ SerializeField ]
	private Transform _myTransform;
	[ SerializeField ]
	private Transform _target;

	public float _scaleAccuracy = 0.1f;

	public Vector2 _aspect = new Vector2( 4f, 3f );

	private Vector3 _originalScale;

	void Start( ) {
		_originalScale = Vector3.one;//_myTransform.localScale;
	}
	
	// Update is called once per frame
	void Update( ) {
		const float THRESHOLD = 0.1f;

		// 位置
		float dictance = Vector3.Distance( _myTransform.position, _target.position );
		Vector3 lpos = transform.localPosition;
		lpos.z = dictance;
		transform.localPosition = lpos;
		// スケール
		Vector3 lscale = transform.localScale;
		lscale.x = _originalScale.x * _aspect.x * dictance * _scaleAccuracy * THRESHOLD;
		lscale.z = _originalScale.z * _aspect.y * dictance * _scaleAccuracy * THRESHOLD;
		transform.localScale = lscale;
		//Debug.Log( "dictance : " + dictance + "Vctor3 : " + lpos );
	}
}
