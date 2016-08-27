Shader "Unlit/NeonTest"
{
	// マテリアルからの設定
	Properties {
		_Alpha("Alpha Color", Range(0, 1)) = 0.5	// 透過
		_Size("Size", Range(1, 100)) = 1	// サイズ
		_Billbord("Billbord", Range(0, 1)) = 0	// ビルボード率

		// ライト
		_LightStartPoint("Start Light Point", Vector) = (0, 0, 0)
		_LightaPoint("Light Point", Vector) = (0, 0, 0)
		_LightMoveRatio("Light Ratio", Range(0, 1)) = 0
		_LightRange("Light Range", Range(0, 1)) = 1

		// 頂点カラー
		_AllColoring("All Coloring", Range(0, 1)) = 0 // 全ての頂点カラーを塗りつぶすフラグ
		_VertexColor("Vertex Color", Color) = (1, 1, 1, 1)	// 頂点カラー

		// ラインカラーリング
		_LineColoringStartPoint("LineColoring Start Point", Vector) = (0, 0, 0)
		_LineColoringPoint("LineColoring Point", Vector) = (0, 0, 0)
		_LineColoringRatio("LineColoring Ratio", Range(0, 1)) = 0
		_LineColoringColor("LineColoring Color", Color) = (1, 1, 1, 1)

		// サークルカラーリング
		_CircleColoringCenter("CircleColoring Center", Vector) = ( 0, 0, 0 )
		_CircleColoringLength("CircleColoring Length", Float) = 1
		_CircleColoringRatio("CircleColoring Ratio", Range( 0, 1 ) ) = 0
		_CircleColoringColor("CircleColoring Color", Color) = ( 1, 1, 1, 1)
    }
	SubShader {
		
		// タグ 
		Tags { 
			"RenderType" = "Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha		// 透過のブレンドモード

		// パス
		Pass {

			CGPROGRAM	// 開始

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			// 構造体
			struct VERTEX {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
				fixed3 color : COLOR0;
			};

			struct FRAGMENT {
				float4 vertex : SV_POSITION;
				fixed3 color : COLOR0;
			};

			// グローバル
			uniform float _Alpha;		// 透過度
			uniform float _Size;		// サイズ
			uniform bool _Billbord;		// ビルボードの使用

			uniform float4 _LightPoint;	// ライトポイント
			uniform float4 _LightStartPoint;// スタート位置
			uniform float _LightMoveRatio;
			uniform float _LightRange;		//範囲

			uniform int _AllColoring;	// 全ての頂点カラーを塗りつぶすフラグ
			uniform fixed3 _VertexColor;// 頂点カラー

			uniform float4 _LineColoringStartPoint;
			//uniform float4 _LineColoringEndPoint;
			uniform float4 _LineColoringPoint;
			uniform float  _LineColoringRatio;
			uniform fixed3 _LineColoringColor;

			uniform float4 _CircleColoringCenter;
			uniform float _CircleColoringLength;
			uniform float _CircleColoringRatio;
			uniform fixed3 _CircleColoringColor;

			// ビルボード変換
			float3 transformBillboard(float4 pos, float4 normal, float2 texcoord1, float2 texcoord2) {
				// Bilbord
				//float size = ( texcoord1.x - 0.5f ) * ( texcoord1.y - 0.5f ) * _Size;
				//float size = (texcoord1.y - 0.5f) * _Size;
				float size = texcoord2.x * _Billbord * _Size;
				float3 eye = ObjSpaceViewDir(pos);
					float3 side = normalize(cross(eye, normal));
					//input.vertex.xyz += ( texcoord1.x - 0.5f ) * ( texcoord1.y - 0.5f ) * side * abs( size );
					return (texcoord1.x - 0.5f) * side * size;
			}

			// ライト色の加算
			fixed3 addLightColoring(float4 vertex, fixed3 color) {
				float4 light_move_dir = normalize(_LightPoint - _LightStartPoint);		// ライトの進む方向
				float4 vertex_dir_from_the_highlight = normalize(vertex - _LightPoint);	// ハイライトから見た頂点の方向
				float abs_dot = abs( dot( light_move_dir, vertex_dir_from_the_highlight ) );
				// 範囲内か確認
				if (abs_dot > _LightRange) {
					return color;
				}
				float ratio = 1.0 - abs_dot;		// 比率 0 ~ 1（内積0で最大にする）
				color.r += (1.0 - color.r) * ratio;
				color.g += (1.0 - color.g) * ratio;
				color.b += (1.0 - color.b) * ratio;
				return color;
			}

			// ラインカラーリング
			fixed3 lineColoring( float4 vertex, fixed3 color ) {
				float4 vec = _LineColoringPoint - _LineColoringStartPoint;//(_LineColoringEndPoint - _LineColoringStartPoint) * _LineColoringRatio;
				float4 vertex_dir = normalize( vertex - ( vec + _LineColoringStartPoint ) );
				
				if ( dot( vec, vertex_dir ) < 0 ) {
					color = _LineColoringColor;
				}

				return color;
			}

			// サークルカラーリング
			fixed3 circleColoring( float4 vertex, fixed3 color) {
				// ベクトル作成
				float4 vec = vertex - _CircleColoringCenter;

				// ベクトルの長さとカラーリングの長さの差分で変化
				if ( length(vec) - _CircleColoringLength * _CircleColoringRatio > 0) {
					float ratio = _CircleColoringLength * _CircleColoringRatio / length(vec);
					color.r += (_CircleColoringColor.r - color.r) * ratio;
					color.g += (_CircleColoringColor.g - color.g) * ratio;
					color.b += (_CircleColoringColor.b - color.b) * ratio;
				}
				else {
					color = _CircleColoringColor;
				}
				return color;
			}

			// Vertex
			FRAGMENT vert(VERTEX input) {

				FRAGMENT frag;

				// 頂点の計算
				if (_Billbord) {
					input.vertex.xyz += transformBillboard(input.vertex, input.normal, input.texcoord, input.texcoord2);
				}
				frag.vertex = mul(UNITY_MATRIX_MVP, input.vertex);

				// 色の取得
				fixed3 color = input.color;

				// 全ての頂点カラーを塗りつぶす
				if ( _AllColoring ) {
					color = _VertexColor;
				}

				// ラインカラーリング
				if ( _LineColoringRatio > 0)  {
					color = lineColoring(input.vertex, color);
				}

				// サークルカラーリング
				if (_CircleColoringRatio > 0 ) {
					color = circleColoring( input.vertex, color );
				}

				// ライト色の加算
				if ( _LightMoveRatio > 0) {
					color = addLightColoring(input.vertex, color);
				}

				// 色を設定
				frag.color = color;

				return frag;
			}

			// Fragment
			fixed4 frag(FRAGMENT input) : SV_Target{
				return fixed4(input.color, _Alpha);
			}

			ENDCG	// 終了
		}
	}
}
