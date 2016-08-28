using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// oculusのUI表示の参考 http://kagring.blog.fc2.com/blog-entry-55.html

public class FadeSceneManeger : Manager< FadeSceneManeger > {

	#region　シーンセッティング用クラス
	[ System.Serializable ]
	private class SceneSetting {
		[ SerializeField ]
		private FadeSceneManeger.TAG _tag = TAG.NONE;
		[ SerializeField ]
		private string _name;

		/// <summary>
		/// タグの取得
		/// </summary>
		/// <returns></returns>
		public TAG getTag( ) {
			return _tag;
		}

		/// <summary>
		/// シーンの名前取得
		/// </summary>
		/// <returns></returns>
		public string getName( ) { 
			return _name;
		}  
 	}
	#endregion

	[ SerializeField ]
    private SceneSetting[ ] _scenes;   // シーン

	public uint _time = 120; //シーンの切り替わるまでの待ち時間

    [ SerializeField ]
	private Color _fadeColor = Color.black;	// フェードカラー

	[ SerializeField ]
    private Canvas _canvas;	// フェードで使うキャンバス

	[ SerializeField ]
    private Image _image;	// フェードアウト用イメージ
	
	// 列挙方
	public enum TAG {
		NONE,
		TITLE,
		TUTORIAL,
		GAME,
		MAX_TAG,
	}
    
	// 定数
    private const int SORTING_ORDER = 100;  // 描画の優先順位
    private const float IMAGE_SCALE = 10;   // イメージの拡大値

	// 変数
	private bool _play = false;
	private float _alphaColor = 0.0f;
	private int _curentTime = 0;
	private TAG _curentTag = TAG.NONE;

	//　インスタンス
	private CanvasGroup _group;
	private Camera _targetCamera;

	#region static関数

	// オブジェクトの作成
	private static void create( ) {
		if ( FadeSceneManeger.getInstance( ) ) {
			return;
		}
		GameObject prefab = ( GameObject )Resources.Load ( "Prefabs/FadeSceneManager" );
		GameObject manager = Instantiate( prefab );
		manager.gameObject.name = "FadeSceneManager";
	}

	/// <summary>
	/// シーンのロード
	/// </summary>
	/// <param name="tag"> タグ </param>
	public static void LoadScene( TAG tag ) {
		FadeSceneManeger instance =  FadeSceneManeger.getInstance( );
		// インスタンス確認
		if ( instance == null ) {
			create( );
		}
		// タグ設定の確認
		if ( tag == TAG.NONE ) {
			Debug.LogError( "タグの設定が\"NONE\"だと実行できません。" );
			return;
		}
		// 設定
		instance._curentTime = ( int )instance._time;
		instance._play = true;
		instance._curentTag = tag;
	}

	/// <summary>
	/// シーンのロード
	/// </summary>
	/// <param name="tag"> タグ</param>
	/// <param name="fadeColor"> フェードの色 </param>
	public static void LoadScene( TAG tag, Color fadeColor ) {
		LoadScene( tag );
		FadeSceneManeger.getInstance( ).setFadeColor( fadeColor );
	}

	public static bool isLoadScene( ) {
		FadeSceneManeger instance = FadeSceneManeger.getInstance( );
		if ( instance == null ) {
			create( );
		}
		return instance._play;
	}
	#endregion

	/// <summary>
	/// Awake関数の代わり
	/// </summary>
	protected override void initialize( ) {
        // 子オブジェ取得
		_group = _canvas.GetComponent< CanvasGroup >( );
        _image = _canvas.transform.FindChild( "Image" ).GetComponent< Image >( );

        // オーダーレイヤー
        _canvas.sortingOrder = SORTING_ORDER;
	}

	// Update is called once per frame
	void FixedUpdate( ) {
		// エラーの確認
		if ( isError( ) ) {
			return;
		}
		// シーンのリロード（Debug）
		debugReloadScene( );// debug

        // フェード用のスクリーンの切り替え
        setFadeScreen( _play );

		// メインカメラをキャンバスにセット
		setMainCameraToCanvas( );
		
        // 実行確認
		if ( !_play ) {
			return;
		}

		// 時間経過
		if ( _curentTime > 0 ) {
			_curentTime--;
		} else {
			_play = false;
		}
		
		// シーン切替
		if ( ( int )_time / 2 == _curentTime ) {
           if ( _curentTag != TAG.NONE ) {
			   SceneManager.LoadScene( getSceneName( _curentTag ), LoadSceneMode.Single );	// シーンのロード
           } else {
               Debug.LogError( "Tag is \"None\"..." );
           }
		}
	}

    // フェードスクリーンの切り替え
    void setFadeScreen( bool enable ) {
        _canvas.gameObject.SetActive( enable );
    }
	
	void OnGUI( ) {
		if ( _play ) {
			// 透明度の更新
			float index = ( ( int )_time - _curentTime ) / ( ( int )_time / 2.0f );
			if ( index > 1.0f ) {
				index = _curentTime / ( ( int )_time / 2.0f );
			}
			_alphaColor = index;
            
            // イメージの色を反映
            _image.color = _fadeColor;

			// グループに反映
			_group.alpha = _alphaColor;
		}
	}

	// シーンをリロードする debug
	void debugReloadScene( ) {
		// 現在のシーンを読み込む
		if ( Input.GetKeyDown( KeyCode.Backspace ) ) {
			foreach ( SceneSetting scene in _scenes ) {
				if ( SceneManager.GetActiveScene( ).name == scene.getName( ) ) {
					LoadScene( scene.getTag( ) );
				}
			}
		}
		// シーンを強制切り替え
		if ( Input.GetKeyDown( KeyCode.F5 ) ) {
			int index = ( _curentTag + 1 < TAG.MAX_TAG )? ( int )_curentTag + 1: ( int )TAG.TITLE;
			LoadScene( ( TAG )index );
		}
	}

    /// <summary>
    /// キャンバスにメインカメラを設定
    /// </summary>
    void setMainCameraToCanvas( ) {
        // カメラ取得 
        Camera camera = Camera.main;
        // カメラのセット
        _canvas.worldCamera = camera;
        // ニアのセット
        const float addNearLength = 0.1f;   // ちらつき防止用に長さを足す
        _canvas.planeDistance = camera.nearClipPlane + addNearLength;
    }

	/// <summary>
	/// エラーの確認
	/// </summary>
	/// <returns></returns>
	bool isError( ) {
		bool error = false;

		// ターゲットのカメラの取得
		if ( !_targetCamera ) {
			try {
				_targetCamera = Camera.main;
			} catch {
				Debug.LogError( "Missing Get Target Camera..." );
			}
		}

		return error;
	}

	// シーンの名前取得
	private string getSceneName( TAG tag ) {
		string name = null;
		foreach ( SceneSetting setting in _scenes ) {
			// タグ確認
			if ( setting.getTag( ) == tag ) {
				name = setting.getName( );
				break;
			}
		}

		return name;
	}

	/// <summary>
	/// フェードカラーを設定
	/// </summary>
	/// <param name="fadeColor"></param>
	public void setFadeColor( Color fadeColor ) {
		_fadeColor = fadeColor;
	}
}
