using UnityEngine;
using System.Collections;

public class NeonTaktShaderController : NeonShaderController {

	#region ラインカラーリングタクト
	public class LineColoringTakt : LineColoring {

		private float _ratio = 0f;

		public LineColoringTakt( NeonTaktShaderController controller, Material mat ) : 
			base ( controller, mat ) {
		}

		public override void update( ) {
			_mat.SetVector( "_LineColoringStartPoint", _start );
			_mat.SetVector( "_LineColoringPoint", ( _end - _start ) * getRatio( ) + _start );
			_mat.SetFloat( "_LineColoringRatio", getRatio( ) );
			_mat.SetColor( "_LineColoringColor", _color );
		}

		/// <summary>
		/// 割合の取得
		/// </summary>
		/// <returns></returns>
		public float getRatio( ) {
			return _ratio;
		}

		/// <summary>
		/// 割合のセット
		/// </summary>
		/// <param name="value"></param>
		public void setRatio( float value ) {
			_ratio = ( value > 1f )? 1f : ( value < 0f )? 0f : value;	//　抑制( 1 ~ 0 )
		}

		/// <summary>
		/// セッティング
		/// </summary>
		/// <param name="color"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void play( Color color , Vector3 start, Vector3 end ) {
			base.play( );

			_color = color;
			_start = start;
			_end = end;
		}
	}
	#endregion

	#region ハイライトタクト
	[ System.Serializable]
	public class HighlightTakt : Highlight {

		private float _ratio = 0f;
		private float _time = 0f;

		public HighlightTakt( NeonTaktShaderController controller, Material mat ) 
			: base( controller, mat ) {
		}

		public void setRatio( float ratio ) {
			ratio = ( ratio > 1f )? 1f : ( ratio < 0f )? 0f : ratio;	// 抑制（ 1 ~ 0 ） 
			_ratio = ratio;
		}

		public override void update( ) {
			if ( !isPlay( ) ) {
				// マテリアル側のの設定初期化
				_mat.SetVector( "_LightPoint", Vector3.zero );
				_mat.SetVector( "_LightStartPoint", Vector3.zero );
				_mat.SetFloat( "_LightMoveRatio", 0f );
				return;
			}

			//// 時間経過
			//_curentTime += Time.deltaTime;
			//if ( _highlightTime - _curentTime <= 0f ) {
			//	_curentTime = 0f;
			//}
			//
			//// 割合更新
			//float ratio = _curentTime / ( _highlightTime / 2f );
			//if ( ratio > 1f ) {
			//	ratio = 2f - ratio;
			//}
 			//setRatio( ratio );  

			// シェーダーへの設定
			_mat.SetFloat( "_LightMoveRatio", _ratio );
			_mat.SetVector( "_LightPoint", ( _end - _start ) * _ratio + _start );
			_mat.SetVector( "_LightStartPoint", _start );
			_mat.SetFloat( "_LightRange", _range );
		}

		public void setting( Vector3 start, Vector3 end ) {
			_start = start;
			_end = end;
			_curentTime = 0f;
			setTime( 10f );
			_range = 0.05f;	// レンジの設定
		}

		/// <summary>
		/// 実行切り替え
		/// </summary>
		/// <param name="enable"></param>
		public void setActive( bool enable ) {
			_play = enable;
		}
	} 
	#endregion

	#region
	[ System.Serializable ]
	public class HitEffectTaky : HitEffect {

		public HitEffectTaky( NeonShaderController controller, Material mat ) 
			: base( controller, mat ) { 
		}

		/// <summary>
		/// ヒットアニメーションのセット
		/// </summary>
		/// <param name="firstTime"></param>
		/// <param name="secondTime"></param>
		/// <param name="thirdTime"></param>
		public void setting( Color color, int firstFrame = 0, int secondFrame = 0, int thirdFrame = 0 ) {
			// 再生の確認
			if ( !isPlay( ) ) {
				// 現在の頂点カラーの取得
				setCurentColor( _controller.getVertexColor( ) );	
			}
			// 各フレームにセット
			setFrame( HitEffect.STEP.FIRST, firstFrame );	// first
			setFrame( HitEffect.STEP.SECOND, secondFrame );	// second
			setFrame( HitEffect.STEP.THIRD, thirdFrame );	// third
			// 変化する色
			_color = color;
			// フラグON
			play( );
		}
	}
	#endregion

	[ SerializeField ]
	private LineColoringTakt _lineColorTakt;
	[ SerializeField ]
	private HighlightTakt _highlightTakt;
	[ SerializeField ]
	private HitEffectTaky _hitEffectTakt;

	public override void createShaderModules( Material mat ) {
		_mat = mat;
		_lineColorTakt = new LineColoringTakt( this, mat );
		_highlightTakt = new HighlightTakt( this, mat );
		_hitEffectTakt = new HitEffectTaky( this, mat );
	}

	public override void updateShaderModules( ) {
		_highlightTakt.update( );
		_lineColorTakt.update( );
		_hitEffectTakt.update( );
	}

	/// <summary>
	/// ハイライトのタクトの取得.
	/// </summary>
	/// <returns></returns>
	public HighlightTakt getHighlightTakt( ) {
		return _highlightTakt;
	}

	/// <summary>
	/// ラインカラータクトの取得.
	/// </summary>
	/// <returns></returns>
	public LineColoringTakt getLineColoringTakt( ) {
		return _lineColorTakt;
	}

	/// <summary>
	/// ヒットエフェクトタクトの取得
	/// </summary>
	/// <returns></returns>
	public HitEffectTaky getHitEffectTakt( ) {
		return _hitEffectTakt;
	}
}
