using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common;

public class TimingManager : MonoBehaviour {

	[ SerializeField ]
	private FileManager _fileManager;

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
			Text text = obj.GetComponentInChildren< Text >( );
			text.text = frame.ToString( );
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

	private FILE_DATA.RHYTHM _data;
	private int _stage = 0;
	private int _oldStage = 0;
	private List< GameObject > _list = new List< GameObject >( ); 

	// Use this for initialization
	void Start( ) {
		_data = _fileManager.getRhythmData( FileManager.TAG.MELODY );

		create( );
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		updateSlider( );
		checkDelete( );
		if ( checkCreate( ) ) {
			//create( );
		}
	}

	void updateSlider( ) {
		Slider offset = _copyModule.getSlider( );
		int index = _rhythmManager.getIndex( );
		foreach ( GameObject obj in _list ) {
			Slider slider = obj.GetComponent< Slider >( );
			slider.maxValue = offset.maxValue;
			slider.minValue = offset.minValue;
			slider.value = _data.array[ index ].frame % _rhythmView.getFrameScale( );
			index++;
		}
	}

	void create( ) {
		// 現在のリストにあるものを削除に指定
		for ( int i = 0; i < _list.Count; i++ ) {
			_list[ i ].name = "Destory";
		}
		// 現在のフレーム
		int frame = _rhythmView.getFrameScale( ) * _stage;
		// インデックス
		int index = _rhythmManager.getIndex( );
		// ステージを作成
		while ( frame < _rhythmView.getFrameScale( ) * ( _stage + 1 ) ) {
			// 生成
			if ( frame == _data.array[ index ].frame ) {
				RectTransform rectTransform = _copyModule.create( frame );

				// リストに登録
				_list.Add( rectTransform.gameObject );

				// ゲームオブジェクトの配下に設定
				rectTransform.parent = transform;

				//トランスフォームの設定
				rectTransform.sizeDelta = new Vector2( _copyModule.getWidth( ), 0f );
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one;

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
		bool check = false;

		_oldStage = _stage;
		_stage = _rhythmManager.getFrame( ) / _rhythmView.getFrameScale( );
		if ( _oldStage != _stage ) {
			check = true;
		}

		return check;
	}
}
