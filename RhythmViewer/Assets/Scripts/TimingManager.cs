using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common;

public class TimingManager : MonoBehaviour {

	[ SerializeField ]
	private EditRhythmManager _editRhythmManager;

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
		public RectTransform create( ) {
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
	private int _originListCount = 0;
	private int _firstIndex = 0;

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

	/// <summary>
	/// リストのインデックスの番号並び替え
	/// </summary>
	void setOrderlineListOfIndex( ) {
		// 配列の確認
		if ( _list.Count < 1 ) {
			return;
		}
		// インデックスの更新
		TimingModule[ ] array = new TimingModule[ _list.Count ];
		int[ ] rank = new int[ _list.Count ];
		for ( int i = 0; i < array.Length; i++  ) {
			array[ i ] = _list[ i ].GetComponent< TimingModule >( );
			rank[ i ] = i;
		}

		// 順番付
		for ( int i = 0; i < array.Length - 1; i++ ) {
			for ( int j = i + 1; j < array.Length; j++ ) {
				if ( array[ i ].getCurrentFrame( ) > array[ j ].getCurrentFrame( ) ) {
					int tmp = rank[ i ];
					rank[ i ] = rank[ j ];
					rank[ j ] = tmp;
				}
			}
		}
		// インデックスの登録
		int firstIndex = _firstIndex;
		for ( int i = 0; i < rank.Length; i++ ) {
			array[ rank[ i ] ].setIndex( i + firstIndex );
		}
	}

	void create( ) {
		// セーブ
		setDataFrame( );

		// 現在のリストにあるものを削除に指定
		for ( int i = 0; i < _list.Count; i++ ) {
			_list[ i ].name = "Destory";
		}

		// オリジナルの個数を初期化
		_originListCount = 0;


		// 現在のフレーム
		int frame = _rhythmView.getFrameScale( ) * _rhythmView.getStageIndex( );

		// インデックス
		int index = _editRhythmManager.getIndex( );

		// ステージを作成
		while ( frame < _rhythmView.getFrameScale( ) * ( _rhythmView.getStageIndex( ) + 1 ) ) {
			// 生成
			if ( frame == ( int )_rhythmView.getData( index ).frame ) {
				RectTransform rectTransform = _copyModule.create( );

				// ゲームオブジェクトの配下に設定
				rectTransform.SetParent( transform );

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

				// オリジナルの個数の繰り上げ
				_originListCount++;
			}
			frame++;
		}

		// ステージ内の最初のインデック番号の取得
		_firstIndex = _rhythmView.getStageFirstTimingIndex( );
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
		int diff = getAddToDiff( );
		// 削除
		if ( diff < 0 ) {
			_rhythmView.deleteArrayDataRange( diff * -1, _firstIndex + _originListCount - 1 );
		}

		// 追加
		if ( diff > 0 ) {
			//TimingModule timing = _list[ 0 ].GetComponent< TimingModule >( );
			//_rhythmView.addArrayData( diff, timing.getIndex( ) + _originListCount );
			_rhythmView.addArrayData( diff, _firstIndex + _originListCount );
		}

		// インデックスの並び替え
		setOrderlineListOfIndex( );


		// 登録
		for ( int i = 0; i < _list.Count; i++ ) {
			TimingModule timing = _list[ i ].GetComponent< TimingModule >( );
			_rhythmView.setArrayDataFrame( timing.getIndex( ), timing.getCurrentFrame( ) );
		}

		
		// リズムマネージャーのデータ更新
		_rhythmView.setDataToRhythmManager( );
	}

	/// <summary>
	/// 最小のインデックスの取得
	/// </summary>
	/// <returns></returns>
	int getMinIndex( ) {
		int min = _list[ 0 ].GetComponent< TimingModule >( ).getIndex( );
		for ( int i = 1; i < _list.Count; i++ ) {
			TimingModule target = _list[ i ].GetComponent< TimingModule >( );
			if ( min > target.getIndex( ) ) {
				min = target.getIndex( );
			}
		}
		return min;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="index"></param>
	public void deleteTiming( int index ) {
		for ( int i = 0; i < _list.Count; i++ ) {
			TimingModule module = _list[ i ].GetComponent< TimingModule >( );
			if ( module.getIndex( ) == index ) {
				Destroy( _list[ i ].gameObject );
				_list.Remove( _list[ i ] );
				setOrderlineListOfIndex( );	// インデックスの並び替え
				break;
			}
		}
	}

	/// <summary>
	/// リストに登録
	/// </summary>
	public void addArray( ) {
		RectTransform rectTransform = _copyModule.create(  );

		// ゲームオブジェクトの配下に設定
		rectTransform.SetParent( transform );

		//トランスフォームの設定
		rectTransform.sizeDelta = new Vector2( _copyModule.getWidth( ), 0f );
		rectTransform.localPosition = Vector3.zero;
		rectTransform.localScale = Vector3.one;

		// コンポーネント追加
		TimingModule modult = rectTransform.gameObject.AddComponent< TimingModule >( );
		modult.initialize(  _rhythmView.getStageIndex( ) + _list.Count - 1,
							_rhythmView.getStageFirstFrame( ) + _rhythmView.getFrameScale( ) - 1,
							_rhythmView.getFrameScale( ),
							0f,
							_rhythmView );

		// リストに登録
		_list.Add( rectTransform.gameObject );
	}

	/// <summary>
	/// 追加した差分を取得
	/// </summary>
	/// <returns></returns>
	public int getAddToDiff( ) {
		return _list.Count - _originListCount;
	}
}
