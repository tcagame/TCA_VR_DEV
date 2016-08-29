using UnityEngine;
using System.Collections;

/// <summary>
/// ネオン専用シェーダーコントロールクラス
/// </summary>
public class NeonShaderController : MonoBehaviour {
	
	// クラス
	#region Shader Animation Module Class
	[ System.Serializable ]
	abstract public class ShaderAnimationModule {

		protected NeonShaderController _controller;
		protected Material _mat;
		protected bool _play;

		protected ShaderAnimationModule( NeonShaderController controller, Material mat ) {
			_controller = controller;
			_mat = mat;
		}

		/// <summary>
		/// 再生確認
		/// </summary>
		/// <returns></returns>
		public bool isPlay( ) {
			return _play;
		}

		/// <summary>
		/// 実行
		/// </summary>
		public void play( ) {
			_play = true;
			playSetting( );
		}

		/// <summary>
		/// 実行時の設定( playメソッドから呼び出しする )
		/// </summary>
		protected abstract void playSetting( );

		/// <summary>
		/// 更新処理
		/// </summary>
		public abstract void update( );
	}
	#endregion

	#region Line Coloring Class
	[ System.Serializable ]
	public class LineColoring : ShaderAnimationModule {
		public Color _color; 
		public Vector3 _start; 
		public Vector3 _end;
		public float _lineColoringTime = 1f;		// アニメーションの時間
		private float _curentTime = 0f;

		public LineColoring( NeonShaderController controller, Material mat ) 
			: base ( controller, mat )  {
		}

		protected override void playSetting( ) {
			_curentTime = 0f;	// 現在時刻を初期化
			if ( _controller.isSynchronize( ) ) {
				// ハイライトを強制スタートへ
				_controller.setHighlight( _start, _end );
			}
		}

		public override void update( ) {
			if ( !isPlay( ) ) {
				return;
			}

			// 時間経過
			if ( _lineColoringTime - _curentTime > 0f ) {
				_curentTime += Time.deltaTime;	
			} else {
				_controller.setVertexColor( _color );
				_mat.SetFloat( "_LineColoringRatio", 0 );
				_play = false; // 終了
				return;
			}

			// パラメーター
			if ( _controller.isSynchronize( ) ) {
				_controller.setHighlightTime( _lineColoringTime );//	HIGHLIGHT_TIME = ANIMATIO_TIME;	// 到達時間を合わせる
			}
			float ratio = _curentTime / _lineColoringTime;
			_mat.SetVector( "_LineColoringStartPoint", _start );
			_mat.SetVector( "_LineColoringPoint", ( _end - _start ) * ratio + _start );
			_mat.SetFloat( "_LineColoringRatio", ratio );
			_mat.SetColor( "_LineColoringColor", _color );
		}

		/// <summary>
		/// 時間のセット
		/// </summary>
		/// <param name="time"></param>
		public void setTime( float time ) {
			if ( time < 0f ) {
				return;
			}
			_lineColoringTime = time;
		}
	}
	#endregion
	
	#region High Light Class
	[ System.Serializable ]
	public class Highlight : ShaderAnimationModule {
		public Vector3 _start;
		public Vector3 _end;
		public float _range = 1f;
		public float _highlightTime = 3f;	// ハイライトの時間
		public float _curentTime = 0f;
		

		public Highlight( NeonShaderController controller, Material mat ) 
			: base( controller, mat ) {
		}

		protected override void playSetting( ) {
			_curentTime = 0f;	// 現在時刻を初期化
		}

		/// <summary>
		///  ハイライトの比を取得（進行具合）
		/// </summary>
		/// <returns></returns>
		private float getHighlightRatio( ) {
			return _curentTime / _highlightTime;
		}

		// ハイライトの更新
		public override void update( ) {
			if ( !isPlay( ) ) {
				// マテリアル側のの設定初期化
				_mat.SetVector( "_LightPoint", Vector3.zero );
				_mat.SetVector( "_LightStartPoint", Vector3.zero );
				_mat.SetFloat( "_LightMoveRatio", 0f );
				return;
			}

			// 時間経過
			_curentTime += Time.deltaTime;
			if ( _curentTime > _highlightTime ) {
				_curentTime = 0f;
			}
		
			// シェーダーへの設定
			float ratio = _curentTime / _highlightTime;
			_mat.SetFloat( "_LightMoveRatio", ratio );
			_mat.SetVector( "_LightPoint", ( _end - _start ) * ratio + _start );
			_mat.SetVector( "_LightStartPoint", _start );
			_mat.SetFloat( "_LightRange", _range );
		}

		/// <summary>
		/// 実行切り替え
		/// </summary>
		/// <param name="enable"></param>
		public void active( bool enable ) {
			_play = enable;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="time"></param>
		public void setTime( float time ) {
			if ( time < 0f ) {
				return;
			}
			_highlightTime = time;
		}
	}
	#endregion
	
	#region Hit Effect Class
	[ System.Serializable ]
	public class HitEffect : ShaderAnimationModule {
		// 列挙型
		public enum STEP {
			FIRST,
			SECOND,
			THIRD,
			MAX_STEP,
		}
		// 変数
		private float[ ] _frame = new float[ ( int )STEP.MAX_STEP ];		
		private Color _curentColor;	// 現在の頂点カラー
		public Color _color;
		private int _hitAnimationFrameCount = 0;	// ヒットアニメーションの経過時間

		public HitEffect( NeonShaderController controller, Material mat ) 
			: base( controller, mat ) { 

		}

		protected override void playSetting( ) {
			_hitAnimationFrameCount = 0;	// フレームの初期化
		}

		/// <summary>
		/// フレームのセット
		/// </summary>
		/// <param name="tag"> フレームタグ </param>
		/// <param name="frame"> フレーム数 </param>
		public void setFrame( STEP tag, int frame ) {
			_frame[ ( int )tag ] = frame;
		}

		/// <summary>
		/// 現在の頂点からをセット
		/// </summary>
		/// <param name="color"></param>
		public void setCurentColor( Color color ) {
			_curentColor = color;
		}

		/// <summary>
		/// ヒットアニメーションを更新
		/// </summary>
		public override void update( ) {
			if ( !isPlay( ) ) {
				return;
			}

			// フレームのカウント
			_hitAnimationFrameCount++;

			// first
			Color changColor = _color;
			Color curentColor = _curentColor;
			float frame = _frame[ ( int )STEP.FIRST ];
			if ( frame - _hitAnimationFrameCount >= 0 ) {
				float ratio = _hitAnimationFrameCount / ( float )frame;
				curentColor += ( changColor - curentColor ) * ratio;
				_controller.setVertexColor( curentColor );
				return;
			}
		
			// second
			frame += _frame[ ( int )STEP.SECOND ];
			if ( frame - _hitAnimationFrameCount > 0 ) {
				_controller.setVertexColor( changColor );
				return;
			}
		
			// third
			frame += _frame[ ( int )STEP.THIRD ];
			if ( frame - _hitAnimationFrameCount >= 0 ) {
				float ratio = 1f - ( _hitAnimationFrameCount - ( _frame[ ( int )STEP.FIRST ] + _frame[ ( int )STEP.SECOND ] ) ) / ( float )_frame[ ( int )STEP.THIRD ];
				curentColor +=  ( changColor - curentColor ) * ratio;
				_controller.setVertexColor( curentColor );
				return;
			}

			// end
			_play = false;
		}
	}
	#endregion

	#region Circle Coloring Class
	[ System.Serializable ]
	public class CircleColoring : ShaderAnimationModule {
		public Vector3 _center;
		public float _length = 1f;
		public float _circleColoringTime = 1f;
		public Color _color;

		private float _curentTime = 0f;	// サークルカラーリングの経過時間

		public CircleColoring( NeonShaderController controller, Material mat ) 
			: base( controller, mat ) { 

		}
		
		protected override void playSetting( ) {
			_curentTime = 0f;
		}

		/// <summary>
		/// サークルカラーリングの更新
		/// </summary>
 		public override void update( ) {
			if ( !isPlay( ) ) {
				return;
			}
			// 時間経過
			if ( _curentTime <= _circleColoringTime ) {
				_curentTime += Time.deltaTime;
			} else  {
				_controller.setVertexColor( _color ); // 頂点カラーのセット
				_mat.SetFloat( "_CircleColoringRatio", 0 );
				_play = false;	// フラグOFF
				return;
			}
			_mat.SetVector( "_CircleColoringCenter", _center );	// センター位置
			_mat.SetFloat( "_CircleColoringLength", _length );	// 長さ
			_mat.SetFloat( "_CircleColoringRatio",  _curentTime / _circleColoringTime );	// 比率
			_mat.SetColor( "_CircleColoringColor", _color );	// 色のセット
		}

	}
	#endregion

	// インスタンス
	private Material _mat;

	// 変数
	[ SerializeField ]
	private LineColoring _lineColoring;
	[ SerializeField ]
	private Highlight _highlight;
	[ SerializeField ]
	private HitEffect _hitEffect;
	[ SerializeField ]
	private CircleColoring _circleColoring;

	private bool _synchronizeAnimation = false;	// 同期してアニメーションするフラグ

	/// <summary>
	/// シェーダモージュール達を作成
	/// </summary>
	/// <param name="mat"></param>
	public void createShaderModules( Material mat ) {
		_mat = mat;
		_lineColoring = new LineColoring( this, _mat );
		_highlight = new Highlight( this, _mat );
		_hitEffect = new HitEffect( this, _mat );
		_circleColoring = new CircleColoring( this, _mat );
	}

	/// <summary>
	/// モードの更新
	/// </summary>
	/// <param name="mode"></param>
	public void updateMode( Neon.MODE mode ) {
		int index;
		if ( mode == Neon.MODE.ALL_COLORING ) {
			index = 1;
		} else {
			index = 0;
		}
		_mat.SetInt( "_AllColoring", index );
	}

	/// <summary>
	/// シェーダモジュールズの更新
	/// </summary>
	public void updateShaderModules( ) {
		_lineColoring.update( );
		_circleColoring.update( );
		_highlight.update( );
		_hitEffect.update( );
	}

	/// <summary>
	/// 頂点カラーを設定（Shader）
	/// </summary>
	/// <param name="color"> 頂点カラー </param>
	public void setVertexColor( Color color ) {
		//カラーをセット
		_mat.SetColor( "_VertexColor", color );
	}

	/// <summary>
	/// 頂点カラーの取得
	/// </summary>
	/// <returns></returns>
	public Color getVertexColor( ) {
		return _mat.GetColor( "_VertexColor" );
	}

	/// <summary>
	/// ハイライトの設定
	/// </summary>
	/// <param name="start"> 開始位置（ローカル空間内） </param>
	/// <param name="end"> 終了位置（ローカル空間内） </param>
	public void setHighlight( Vector3 start, Vector3 end ) {
		_highlight._start = start;
		_highlight._end = end;
		_highlight._curentTime = 0f;
	}

	/// <summary>
	/// アニメーションの設定
	/// </summary>
	/// <param name="color"> 変化する色</param>
	/// <param name="start"> 開始位置（ローカル空間内） </param>
	/// <param name="end"> 終了位置（ローカル空間内） </param>
	public void setAnimationVertexColoring( Color color , Vector3 start, Vector3 end ) {
		_lineColoring._color = color;
		_lineColoring._start = start;
		_lineColoring._end = end;

		_lineColoring.play( );
	}

	/// <summary>
	/// ハイライトの実行切り替え
	/// </summary>
	/// <param name="enable"></param>
	public void setHightightActive( bool enable ) {
		_highlight.active( enable );
	}

	/// <summary>
	/// ハイライトの実行確認
	/// </summary>
	/// <returns></returns>
	public bool isHighLight( ) {
		return _highlight.isPlay( );
	}

	/// <summary>
	/// ハイライトする時間を設定
	/// </summary>
	/// <param name="time"> ゼロ以外の値を設定してください </param>
	public void setHighlightTime( float time ) {
		if ( time <= 0f ) {
			return;
		}
		//HIGHLIGHT_TIME = time;
		_highlight.setTime( time );
	}

	/// <summary>
	/// カラーリングとハイライトの同期の切り替え
	/// </summary>
	/// <param name="enable"></param>
	public void setSynchronizeAnimation( bool enable ) {
		_synchronizeAnimation = enable;
	}

	/// <summary>
	/// ヒットアニメーションのセット
	/// </summary>
	/// <param name="firstTime"></param>
	/// <param name="secondTime"></param>
	/// <param name="thirdTime"></param>
	public void setHitAnimation( Color color, int firstFrame = 0, int secondFrame = 0, int thirdFrame = 0 ) {
		// 再生の確認
		if ( !_hitEffect.isPlay( ) ) {
			// 現在の頂点カラーの取得
			_hitEffect.setCurentColor( getVertexColor( ) );	
		}
		// 各フレームにセット
		_hitEffect.setFrame( HitEffect.STEP.FIRST, firstFrame );	// first
		_hitEffect.setFrame( HitEffect.STEP.SECOND, secondFrame );	// second
		_hitEffect.setFrame( HitEffect.STEP.THIRD, thirdFrame );	// third
		// 変化する色
		_hitEffect._color = color;
		// フラグON
		_hitEffect.play( );
	}

	/// <summary>
	/// サークルカラーリングのセット
	/// </summary>
	/// <param name="center"></param>
	/// <param name="length"></param>
	/// <param name="time"></param>
	/// <param name="color"></param>
	public void setCircleColoring( Vector3 center, float length, float time, Color color ) {
		_circleColoring._center = center;
		_circleColoring._length = length;
		_circleColoring._circleColoringTime = ( time > 0f )? time : 1f;
		_circleColoring._color = color;
		
		// フラグON
		_circleColoring.play( );
	}

	/// <summary>
	/// 同期設定の確認
	/// </summary>
	/// <returns></returns>
	bool isSynchronize( ) {
		return _synchronizeAnimation;
	}
}
