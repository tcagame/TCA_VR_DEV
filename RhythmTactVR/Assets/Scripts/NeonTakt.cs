using UnityEngine;
using System.Collections;

public class NeonTakt : Neon {

	public float _maxAngleVelocity = 5f;

	[ SerializeField ]
	private RhythmCTRL_MNG _rhythmCTRL_MNG;
	[ SerializeField ]
	private ControllerMng3 _controller;

	public float _thresholdSwing = 10f;	// 振り判定時の閾値.

	[ SerializeField ]
	private Color _awakeVertexColor = Color.blue;
	
	private NeonTaktShaderController _shader;

	private const int MAX_BUF = 3;
	private float[ ] _angleVelocityBuf = new float[ MAX_BUF ];
	private int _bufIndex = 0;
	private int _energy = 0;	// エネルギー
	private const int MAX_ENERGY = 10000;
	private bool _requestSwingTiming = false;	// タイミング前にスウィングできているかのフラグ

	#region 平均算出用
	private float _max = 0f;
	private const int MAX_SAMPLING = 5;	// サンプリング数
	private const int ACCURACY = MAX_SAMPLING - 2; // 精度
	private int _averageIndex = 0;	// インデックス
	private float[ ] _samplingArray = new float[ MAX_SAMPLING ];	// 
	private bool _sampling = false;
	#endregion

	/// <summary>
	///  シェーダーの初期設定
	/// </summary>
	/// <param name="mat"></param>
	void initShaderSetting( Material mat ) {
		_shader = GetComponent< NeonTaktShaderController >( );
		_shader.createShaderModules( mat );	// 各シェーダークラスを作成
		_shader.setVertexColor( _awakeVertexColor );	// 初期の頂点カラーをセット

		// ハイライトのセット
		//playHighlight( );
		// ラインカラーの実行
		//playLineColoring( );
	}

	/// <summary>
	/// モデルの作成（モデルを使用しないとき用）
	/// </summary>
	/// <param name="mesh"></param>
	void createModel( ref Mesh mesh ) {
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

	protected override void Awake( ) {
		// マテリアルのインスタンスを作成
		Material mat = Instantiate( _mat ) as Material;

		// シェーダーの初期設定
		initShaderSetting( mat );

		// メッシュの取得
		Mesh mesh = getMesh( );	

		// モデルを作成
		if ( !_useModel ) {
			createModel( ref mesh );
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

	protected new void FixedUpdate( ) {
		// モードの更新
		switch ( _mode ) {
			case MODE.ALL_COLORING:
				updateAllColoring( );
				break;
			case MODE.SINGLE_COLORING:
				//updateSingleColoring( );
				break;
		}

		// シェーダーの更新
		//_shader.setSynchronizeAnimation( _synchronizeColoringAndHightlight );
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
			playLineColoring( );
		}

		// ハイライトの実行
		if ( Input.GetKeyDown( KeyCode.F2 ) ) {
			playHighlight( );
		}

		// ヒットアニメーション
		if ( Input.GetKeyDown( KeyCode.F3 ) ) {
			hitAnimationPlay( );
		}

		// サークルカラーアニメーション
		if ( Input.GetKeyDown( KeyCode.F4 ) ) {
			circleColoringPlay( Vector3.zero, 2f, 1f, table[ Random.Range( 0, table.Length ) ] );
		}

		// サンプリングの切り替え
		if ( Input.GetKeyDown( KeyCode.Space ) ) {
			_sampling = ( _sampling )? false: true;
		}

		if ( _sampling ) {
			Debug.Log( _rhythmCTRL_MNG.getAngularVelocityLength( ) );
		}

		// タクトエネルギーを更新
		updateChageEnergy( );

		updateHitEffect( );

		// タイミングの更新
		//updateMachTiming( );
		
		// 平均表示
		//averageAngleVelocity( );
	}

	void updateChageEnergy( ) {
		//Debug.Log( "AngularVec : " + _rhythmCTRL_MNG.getAngularVelocityLength( ) );
		_angleVelocityBuf[ _bufIndex ] = _rhythmCTRL_MNG.getAngularVelocityLength( );
		_bufIndex++;
		_bufIndex %= MAX_BUF;

		// 角速度の比較
		int point = 0;
		for ( int i = 0; i < MAX_BUF; i++ ) {
			if ( _angleVelocityBuf[ i ] > _thresholdSwing ) {
				point++;
			}
		}
		if ( point == MAX_BUF ) {
			_energy += 100;
		} else {
			_energy -= 5;
		}
		_energy = ( _energy > MAX_ENERGY )? MAX_ENERGY : ( _energy < 0 )? 0 : _energy;	// 抑制( MAX_ENERGY ~ 0 ) 
 
		float ratio = _energy / ( float )MAX_ENERGY; 
		//Debug.Log( "Energy : " + _energy + "  Ratio : " + ratio );

		// ハイライトの割合セット.
		setHighlightRatio( ( ratio > 1f )? 1f : ratio  );

		// ラインカラーの割合セット.
		_shader.getLineColoringTakt( ).setRatio( ratio ); 

		//setCi
	}

	void updateMachTiming( ) {
		// タイミングの確認.
		if ( _rhythmManager.isTiming( RhythmManager.RHYTHM_TAG.MAIN ) ) {
			// タイミングよく振ったかの確認.
			if ( _requestSwingTiming ) {
				hitAnimationPlay( );	// 光る.
				_requestSwingTiming = false;
			}
		} else {
			// 振っているかの確認.
			if ( _rhythmCTRL_MNG.isSwing( ) ) {
				_requestSwingTiming = true;
			}
		}
	}

	void updateHitEffect( ) {
		if ( _controller.isHit( ) ) {
			hitAnimationPlay( );	// 光る.
		}
	}

	void averageAngleVelocity( ) {
		// サンプル取得.
		_samplingArray[ _averageIndex ] = _rhythmCTRL_MNG.getAngularVelocityLength( );
		_averageIndex++;

		// 移動平均算出.
		if ( _averageIndex >= MAX_SAMPLING ) {
			float[ ] average = new float[ 3 ]{ 0f, 0f, 0f };
			for ( int i = 0; i < ACCURACY; i++ ) {
				average[ 0 ] += _samplingArray[ i ];
				average[ 1 ] += _samplingArray[ i + 1 ];
				average[ 2 ] += _samplingArray[ i + 2 ];
			}
			average[ 0 ] /= ( float )ACCURACY;
			average[ 1 ] /= ( float )ACCURACY;
			average[ 2 ] /= ( float )ACCURACY;
			float sam =  average[ 0 ] + average[ 1 ] + average[ 2 ];
			Debug.Log( "Average : " + (  sam / 3f ) );

			_averageIndex = 0;
		}
	}

	// ラインカラーの再生
	protected void playLineColoring( ) {
		Color[ ] table = {
			Color.red,
			Color.green,
			Color.blue,
			Color.cyan,
			Color.yellow,
		};
		_shader.getLineColoringTakt( ).play( table[ Random.Range( 0, table.Length ) ],
											 new Vector3( 0, 0, -1f ),
											 new Vector3( 0, 0, 1.2f ) );
	}

	/// <summary>
	/// ハイライトの再生
	/// </summary>
	protected void playHighlight( ) {
		_shader.getHighlightTakt( ).setActive( ( _shader.isHighLight( ) ) ? false : true );
		_shader.getHighlightTakt( ).setting( new Vector3( 0, 0, -0.15f ), new Vector3( 0, 0, 0.15f ) );
	}

	/// <summary>
	/// ハイライトの割合セット
	/// </summary>
	/// <param name="ratio"></param>
	private void setHighlightRatio( float ratio ) {
		_shader.getHighlightTakt( ).setRatio( ratio );
	}

	
	protected override void hitAnimationPlay( ) {
		// テーブル
		int[ ] ratioTable = {
			1, 2, 1,
		};
		int[ ] frameTable = {
			10, 20, 10,
		};
		int frameSum = frameTable[ 0 ] + frameTable[ 1 ] + frameTable[ 2 ];	// 総フレーム数
		int ratioSum = ratioTable[ 0 ] + ratioTable[ 1 ] + ratioTable[ 2 ];	// 総比率
		int nextFrameTiming = _rhythmManager.getNextBetweenFrame( RhythmManager.RHYTHM_TAG.MAIN  );	// 次までのフレーム
		// 次のフレームまで終わるか確認
		if ( frameSum > nextFrameTiming ) {
			frameTable[ 0 ] = nextFrameTiming * ratioTable[ 0 ] / ratioSum;
			frameTable[ 1 ] = nextFrameTiming * ratioTable[ 1 ] / ratioSum;
			frameTable[ 2 ] = nextFrameTiming * ratioTable[ 2 ] / ratioSum;
		}

		_shader.getHitEffectTakt( ).setting( Color.white, frameTable[ 0 ], frameTable[ 1 ], frameTable[ 2 ] );
	}
}
