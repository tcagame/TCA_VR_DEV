using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TimingManager : MonoBehaviour {

	[ SerializeField ]
	private RhythmManager _rhythmManager;

	[ SerializeField ]
	private RhythmViewer _rhythmView;

	[ SerializeField ]
	private CopyModule _copyModule;


	#region コピー用モジュール　クラス
	[ System.Serializable ]
	private class CopyModule {
		[ SerializeField ]
 		private GameObject _prefab;
		[ SerializeField ]
		private Slider _slider;
		[ SerializeField ]
		private float _width = 5f;
		
		/// <summary>
		/// スライダーの取得
		/// </summary>
		/// <returns></returns>
		public Slider getSlider( ) {
			return _slider;
		}

		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="frame"> フレーム位置 </param>
		/// <returns></returns>
		public RectTransform create( int frame ) {
			GameObject obj = ( GameObject )Instantiate( _prefab );
			return obj.GetComponent< RectTransform >( );
		}

		/// <summary>
		/// 幅の取得
		/// </summary>
		/// <returns></returns>
		public float getWidth( ) {
			return _width;
		}
	}
	#endregion

	private List< GameObject > _list = new List< GameObject >( ); 
	private int _oldStageIndex = -1;

	// Use this for initialization
	void Start( ) {

	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		checkDelete( );
		if ( checkCreate( ) ) {
			create( );
		}
		
		_oldStageIndex = _rhythmView.getStageIndex( );
	}

	void create( ) {
		// セーブ
		setDataFrame( );

		// 現在のリストにあるものを削除に指定
		for ( int i = 0; i < _list.Count; i++ ) {
			_list[ i ].name = "Destory";
		}

		// 現在のフレーム
		int frame = _rhythmView.getFrameScale( ) * _rhythmView.getStageIndex( );

		// インデックス
		int index = _rhythmManager.getIndex( );

		// ステージを作成
		while ( frame < _rhythmView.getFrameScale( ) * ( _rhythmView.getStageIndex( ) + 1 ) ) {
			// 生成
			if ( frame == _rhythmView.getData( index ).frame ) {
				RectTransform rectTransform = _copyModule.create( frame );

				// ゲームオブジェクトの配下に設定
				rectTransform.parent = transform;

				//トランスフォームの設定
				rectTransform.sizeDelta = new Vector2( _copyModule.getWidth( ), 0f );
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one;

				// コンポーネント追加
				TimingModule modult = rectTransform.gameObject.AddComponent< TimingModule >( );
				modult.initialize( index, frame, _rhythmView.getFrameScale( ), 0f, _rhythmView );

				// リストに登録
				_list.Add( rectTransform.gameObject );

				// インデックスの繰り上げ
				index++;
			}
			frame++;
		}
	}

	void checkDelete( ) {
		// リストから削除
		for ( int i = 0; i < _list.Count; i++ ) {
			if ( _list[ i ].name == "Destory" ) {
				Destroy( _list[ i ] );
				_list.RemoveAt( i );
			}
		}
	}

	bool checkCreate( ) {
		return _rhythmView.getStageIndex( ) != _oldStageIndex;
	}

	/// <summary>
	/// データを上書き
	/// </summary>
	public void setDataFrame( ) {
		for ( int i = 0; i < _list.Count; i++ ) {
			TimingModule timing = _list[ i ].GetComponent< TimingModule >( );
			_rhythmView.setArrayDataFrame( timing.getIndex( ), timing.getCurrentFrame( ) );
		}
	}
}
