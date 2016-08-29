using UnityEngine;
using System.Collections;

[ RequireComponent( typeof( MeshFilter ) ) ]
public class Neon : MonoBehaviour {

	// エディター設定
	public MODE _mode = MODE.ALL_COLORING;	// モード
	public bool _synchronizeColoringAndHightlight = false;		// カラーリングとハイライトを同期させるフラグ
	
	[ SerializeField ]
	private Material _mat;		//  マテリアル

	[ SerializeField ]
	private bool _useModel = true;		// モデルの使用フラグ

	// 列挙型
	public enum MODE {
		ALL_COLORING,		// 一括のカラーリング
		SINGLE_COLORING,	// 単体でのカラーリング
		NONE,
	}

	// インスタンス
	private MeshFilter _filter;
	private NeonShaderController _shader;
	private RhythmManager _rhythmManager;

	// 変数
	private Color[ ] _colors;

	private void Awake( ) {
		// マテリアルのインスタンスを作成
		Material mat = Instantiate( _mat ) as Material;

		// コンポーネント取得
		_shader = GetComponent< NeonShaderController >( );

		// シェーダモジュール達をの作成
		_shader.createShaderModules( mat );	

		// メッシュの取得
		Mesh mesh = getMesh( );	

		// モデルを使用しないとき用
		if ( !_useModel ) {
			mesh.vertices = new Vector3[ ] {
				//new Vector3 (   0,  1f ),
				//new Vector3 (  1f, -1f ),
				//new Vector3 ( -1f, -1f ),
				new Vector3 ( 0f, 0f ),
				new Vector3 ( 1f, 0f ),
				new Vector3 ( 1f, 1f ),
				new Vector3 ( 0f, 1f ),
			};
			mesh.triangles = new int[ ] {
				0, 2, 1,
				0, 3, 2
			};
		}

		// 頂点カラー配列を確保
		_colors = new Color[ mesh.vertices.Length ];

		// 適用
		_filter = GetComponent< MeshFilter >( );
		_filter.sharedMesh = mesh;

		MeshRenderer renderer = GetComponent< MeshRenderer >( );
		renderer.material = mat;


		
		// リズムマネージャーの取得
		_rhythmManager = GameObject.Find( "RhythmManager" ).GetComponent< RhythmManager >( );
	}

	// 更新
	void FixedUpdate( ) {
		// モードの更新
		switch ( _mode ) {
			case MODE.ALL_COLORING:
				updateAllColoring( );
				break;
			case MODE.SINGLE_COLORING:
				updateSingleColoring( );
				break;
		}
		
		// シェーダーの更新
		_shader.setSynchronizeAnimation( _synchronizeColoringAndHightlight );
		_shader.updateMode( _mode );
		_shader.updateShaderModules( );
	}

	// オールカラーリングの更新(シェーダーでの操作)
	void updateAllColoring( ) {
		Color[ ] table = {
			Color.red,
			Color.green,
			Color.blue,
			Color.cyan,
			Color.yellow,
		};
		// すべての頂点カラーをセット
		if ( Input.GetKeyDown( KeyCode.F1 ) ) {
			lineColoringPlay( table[ Random.Range( 0, table.Length ) ], new Vector3( -1, -1, -1 ), new Vector3( 1, 1, 1 ) );
		}

		// ハイライトの実行
		if ( Input.GetKeyDown( KeyCode.F2 ) ) {
			_shader.setHighlight( new Vector3( -1, -1, -1 ), new Vector3( 1, 1, 1 ) );
			_shader.setHightightActive( ( _shader.isHighLight( ) )? false : true );
		}

		// ヒットアニメーション
		if ( Input.GetKeyDown( KeyCode.F3 ) ) {
			hitAnimationPlay( );
		}

		// サークルカラーアニメーション
		if ( Input.GetKeyDown( KeyCode.F4 ) ) {
			circleColoringPlay( Vector3.zero, 2f, 1f, table[ Random.Range( 0, table.Length ) ] );
		}

		// リズムマネージャー
		if ( _rhythmManager ) {
			// タイミング時
			if ( _rhythmManager.isTiming( ) ) {
				hitAnimationPlay( );
				// カラーアニメーションの実行
				if ( _rhythmManager.getIndex( ) % 3 == 0 ) {
					circleColoringPlay( Vector3.zero, 2f, 1f, table[ _rhythmManager.getIndex( ) % table.Length ] );
				}
			}
		}
	}
	
	// シングルカラーリングの更新
	void updateSingleColoring( ) {
		setVertexColoring( ref _colors );

		// 頂点カラーの更新
		_filter.mesh.colors = _colors;
	}

	/// <summary>
	/// メッシュの取得
	/// </summary>
	/// <returns></returns>
	Mesh getMesh( ) {
		Mesh mesh = null;
		if ( _useModel ) {
			MeshFilter mf = GetComponent< MeshFilter >( );
			mesh = mf.mesh;
		} else {
			mesh = new Mesh( );
		}
		return mesh;
	}

	/// <summary>
	/// 頂点カラーに色を塗る
	/// </summary>
	/// <param name="index"> 頂点の数 </param>
	/// <returns> 成功: 配列の取得　失敗：null </returns>
	void setVertexColoring( ref Color[ ] colors ) {
		// カラーのテーブル
		Color[ ] table = {
			Color.red,
			Color.green,
			Color.blue,
		};

		int first_skip = 0;	// 最初のスキップ数
		int later_skip = 0;	// 最後のスキップ数
		int step = 20;			// ステップ数（頂点が指定回数をまたぐと色が変わる）
		int arrayLength = colors.Length;	// 配列の長さ
		for ( int i = 0; i < arrayLength; i++ ) {
			// 色の選択
			int index = i / step % table.Length;
			Color color = table[ index ];

			// スキップ
			if ( first_skip > 0 ) {
				first_skip--;
				color = Color.grey;	// 最初のスキップ中の色
			}
			if ( arrayLength - later_skip <= i ) {
				later_skip--;
				color = Color.grey;	// 後のスキップ中の色
			}

			// セット
			colors[ i ] = color;
		}
	}
	
	/// <summary>
	/// 指定した板ポリゴンの頂点カラーを設定する(Plane専用)
	/// </summary>
	/// <param name="colors"> 頂点カラー配列 </param>
	/// <param name="number"> 板ポリゴンのナンバー </param>
	/// <param name="color"> 色 </param>
	void setQuadVertecColor( ref Color[ ] colors, int number, Color color ) {
		const int ROW = 10;	// 行
		const int COL = 10;	// 列
		
		// 配列オーバーの対応
		if ( ROW * COL <= number || number < 0 ) {
			return;
		}

		// 色の変更
		int index = number / ROW + number;
		colors[ index ] = color;
		colors[ index + 1 ] = color;
		colors[ index + 1 + ROW ] = color;
		colors[ index + 1 + ROW + 1 ] = color;
	}

	/// <summary>
	/// 全ての頂点カラーを設定する
	/// </summary>
	/// <param name="color"></param>
	void setAllVertexColoring( Color color ) {
		_shader.setVertexColor( color );
	}

	/// <summary>
	/// アニメーション式の頂点カラーリング
	/// </summary>
	/// <param name="color"> 変化する色 </param>
	/// <param name="start"> スタート位置（ローカル空間） </param>
	/// <param name="end"> エンド位置（ローカル空間） </param>
	void lineColoringPlay( Color color, Vector3 start, Vector3 end ) {
		_shader.setAnimationVertexColoring( color, start, end );
	}

	/// <summary>
	/// ヒットしたアニメーション
	/// </summary>
	void hitAnimationPlay( ) {
		// テーブル
		int[ ] ratioTable = {
			1, 2, 1,
		};
		int[ ] frameTable = {
			10, 20, 10,
		};
		int frameSum = frameTable[ 0 ] + frameTable[ 1 ] + frameTable[ 2 ];	// 総フレーム数
		int ratioSum = ratioTable[ 0 ] + ratioTable[ 1 ] + ratioTable[ 2 ];	// 総比率
		int nextFrameTiming = _rhythmManager.getNextBetweenFrame( );	// 次までのフレーム
		// 次のフレームまで終わるか確認
		if ( frameSum > nextFrameTiming ) {
			frameTable[ 0 ] = nextFrameTiming * ratioTable[ 0 ] / ratioSum;
			frameTable[ 1 ] = nextFrameTiming * ratioTable[ 1 ] / ratioSum;
			frameTable[ 2 ] = nextFrameTiming * ratioTable[ 2 ] / ratioSum;
		}

		_shader.setHitAnimation( Color.white, frameTable[ 0 ], frameTable[ 1 ], frameTable[ 2 ] );
	}

	/// <summary>
	/// サークルカラーリングの実行
	/// </summary>
	/// <param name="center"></param>
	/// <param name="length"></param>
	/// <param name="time"></param>
	/// <param name="color"></param>
	void circleColoringPlay( Vector3 center, float length, float time, Color color ) {
		_shader.setCircleColoring( center, length, time, color );
	}
}