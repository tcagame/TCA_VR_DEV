using UnityEngine;
using System.Collections;

[ RequireComponent( typeof( MeshRenderer ) ) ]
public class VertexLiner : MonoBehaviour {

	// エディター設定
	public Color _color = Color.white;

	private Material _material;

	private float _value = 1f;

	[ SerializeField ]
	private SteamVR_TrackedObject[ ] _steamObjects;
	
	enum COLOR {
		RED,
		GREEN,
		BULE,
		ALPHA,
		MAX_COLOR,
	}

	COLOR _tag = COLOR.RED;

    void Awake( ) {
		// マテリアル取得
		_material = GetComponent< Renderer >( ).material;

		// メッシュの設定
		MeshFilter mf = GetComponent< MeshFilter >( );
		mf.mesh.SetIndices( makeIndices( mf.mesh.triangles ), MeshTopology.LineStrip, 0 );
    }
    

	// Use this for initialization
	void Start( ) {
		
	}
	
	// Update is called once per frame
	void Update( ) {
		for ( int i = 0; i < _steamObjects.Length; i++ ) {
			// デバイスの取得
			SteamVR_Controller.Device device;
			try {
				device = SteamVR_Controller.Input( (int) _steamObjects[ i ].index );
			} catch {
				break;
			}

			{ // 色の対象切り替え
				if ( device.GetPressDown( SteamVR_Controller.ButtonMask.Grip ) ) {
					_tag++;
				}
				if ( Input.GetKeyDown( KeyCode.LeftArrow ) ) {
					_tag += ( int )COLOR.MAX_COLOR - 1;
				}
				if ( Input.GetKeyDown( KeyCode.RightArrow ) ) {
					_tag++;
				}
				_tag = ( COLOR )( ( int )_tag % ( int )COLOR.MAX_COLOR );
			}

			{// 色の更新
				Vector2 vec = Vector2.one;	// 単位ベクトル
				vec -= device.GetAxis( Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger ); // コントローラーの入力
			
				switch ( _tag ) {
					case COLOR.RED:
						_color.r = vec.x;
						break;
					case COLOR.GREEN:
						_color.b = vec.x;
						break;
					case COLOR.BULE:
						_color.g = vec.x;
						break;
					case COLOR.ALPHA:
						_color.a = vec.x;
						break;
				}
			}
		}
		_material.SetColor( "_Color", _color );
		//_material.color = _color;
	}

	int[ ] makeIndices( int[ ] triangles )	{
		int[ ] indices = new int [2 * triangles.Length ];
		int i = 0;
		for( int t = 0; t < triangles.Length; t+=3 )
		{
			indices[i++] = triangles[t];		//start
			indices[i++] = triangles[t + 1];	//end
			indices[i++] = triangles[t + 1];	//start
			indices[i++] = triangles[t + 2];	//end
			indices[i++] = triangles[t + 2];	//start
			indices[i++] = triangles[t];		//end
		}
		return indices;
	}

	bool isError( ) {
		bool error = false;
		try {
			if ( !_steamObjects[ 0 ] ) {
					_steamObjects = GameObject.Find( "ControllerDebug" ).GetComponent< ControllerDebug >( ).getTrackedObject( );
			}
		} catch {
			error = true;
		}

		try {
			if ( !_steamObjects[ 1 ] ) {
					_steamObjects = GameObject.Find( "ControllerDebug" ).GetComponent< ControllerDebug >( ).getTrackedObject( );
			}
		} catch {
			error = true;
		}

		return error;
	}
}
