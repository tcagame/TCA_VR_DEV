using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class FileManager : Manager< FileManager > {

	#region ファイルクラス
	[ System.Serializable ]
 	protected class File {
		[ SerializeField ]
		private string _name;		// 名前

		private FILE_DATA _data;	// データ

		/// <summary>
		/// データのセット
		/// </summary>
		/// <param name="data"></param>
		public void setData( FILE_DATA data ) {
			_data = data;
		}

		/// <summary>
		/// ファイル名の取得
		/// </summary>
		/// <returns></returns>
		public string getName( ) {
			return _name;
		}

		/// <summary>
		/// データの存在確認
		/// </summary>
		/// <returns></returns>
		public bool isData( ) {
			bool frag = false;
            frag = ( _data.enemy.ma.list != null) ? true : false;	// エネミージェネレーターの配列確認
            frag = ( _data.enemy.sb.list != null) ? true : false;	// エネミージェネレーターの配列確認
            frag = ( _data.enemy.vo.list != null) ? true : false;	// エネミージェネレーターの配列確認
			frag = ( _data.rhythm.ma != null )? true : false;		    // リズム配列の確認
            frag = ( _data.rhythm.sb != null) ? true : false;		    // ドラム配列の確認
            frag = ( _data.rhythm.vo != null) ? true : false;		    // ボーカル配列の確認
            frag = ( _data.rhythm.md != null) ? true : false;		    // モード配列の確認
            frag = ( _data.rhythm.ga != null) ? true : false;		    // モード配列の確認
			return frag;
		}

		/// <summary>
		/// データの取得
		/// </summary>
		/// <returns></returns>
		public FILE_DATA getData( ) {
			return _data;
		}
	}
	#endregion

	[ SerializeField ]
	protected File _file = new File( );

	// Awake関数の代わり
	protected override void initialize( ) {

	}

	void FixedUpdate( ) {
		cheackFilesData( );
	}

	/// <summary>
	/// ファイルデータのチェック
	/// </summary>
	void cheackFilesData( ) {
		// データ確認
		if ( !_file.isData( ) ) {
			loadFile( _file );// ロード
		}
	}

	/// <summary>
	/// ファイルのロード
	/// </summary>
	/// <param name="fileName"> ファイルの名前 </param>
	/// <param name="list"> ファイルデータ型のリスト </param>
	private bool loadFile( File file ) {
		try {
			StreamReader sr = new StreamReader( "../" + file.getName( ) + ".csv" );

			FILE_DATA data = new FILE_DATA( );

			// リズムデータの取得
			data.rhythm = getLoadFileRhythmData( ref sr );
				
			// エネミーデータの取得
			data.enemy = getLoadFileEnemyGeneratorData( ref sr );
				
			sr.Close( );

			// データ上書き
			file.setData( data );

			return true;
		} catch {
			Debug.LogError( "Missing Load File..." );
			return false;
		}
	}
		
	/// <summary>
	/// ロードしたファイルからリズムデータを取得
	/// </summary>
	/// <returns></returns>
	protected FILE_DATA.RHYTHM getLoadFileRhythmData( ref StreamReader sr ) {
		FILE_DATA.RHYTHM data = new FILE_DATA.RHYTHM( );

		// 個数の取得
		string str = sr.ReadLine( );
		string[ ] values = str.Split( ',' );

		int ma_size = int.Parse( values[ 0 ] );
		int sb_size = int.Parse( values[ 2 ] );
		int vo_size = int.Parse( values[ 4 ] );
		int md_size = int.Parse( values[ 6 ] );
		int ga_size = int.Parse( values[ 8 ] );

		// 配列確保
		data.ma = new TIMING_DATA[ ma_size ];
        data.sb = new TIMING_DATA[ sb_size ];
        data.vo = new TIMING_DATA[ vo_size ];
        data.md = new TIMING_DATA[ md_size ];
        data.ga = new TIMING_DATA[ ga_size ];


        int[ ] array = new int[ ] { ma_size, sb_size, vo_size, md_size, ga_size };
        int length = array[ 0 ];
        // 大きいほうを設定
       for ( int i = 1; i < array.Length; i++ ) {
           if ( array[ i ] > length ) {
               length = array[ i ];
           }
       }

		for ( int i = 0; i < length; i++ ) {
			// ファイルから一行読み込む
			str = sr.ReadLine( );

			// 読み込んだ一行をカンマ毎に分けて配列に格納する
			values = str.Split( ',' );

            // タイミングデータを書き込み
            if ( i < ma_size ) {
				// インデックスの取得
				data.ma[ i ].index = int.Parse( values[ 0 ] );
				// 時間の取得
				data.ma[ i ].frame = uint.Parse( values[ 1 ] );
            } 
            if ( i < sb_size ) {
				// インデックスの取得
				data.sb[ i ].index = int.Parse( values[ 2 ] );
				// 時間の取得
				data.sb[ i ].frame = uint.Parse( values[ 3 ] );
            } 
            if ( i < vo_size ) {
				// インデックスの取得
				data.vo[ i ].index = int.Parse( values[ 4 ] );
				// 時間の取得
				data.vo[ i ].frame = uint.Parse( values[ 5 ] );
            } 
            if ( i < md_size ) {
				// インデックスの取得
				data.md[ i ].index = int.Parse( values[ 6 ] );
				// 時間の取得
				data.md[ i ].frame = uint.Parse( values[ 7 ] );
            } 
            if ( i < ga_size ) {
				// インデックスの取得
				data.ga[ i ].index = int.Parse( values[ 8 ] );
				// 時間の取得
				data.ga[ i ].frame = uint.Parse( values[ 9 ] );
            } 
		}

		return data;
	}
		
	/// <summary>
	/// ロードしたファイルからエネミージェネレーターのデータを取得
	/// </summary>
	/// <returns></returns>
	protected FILE_DATA.ENEMY_FOR_RHYTHM getLoadFileEnemyGeneratorData( ref StreamReader sr ) {
		FILE_DATA.ENEMY_FOR_RHYTHM data = new FILE_DATA.ENEMY_FOR_RHYTHM( );

		// リストの確保
		data.ma.list = new List< ENEMY_GENERATOR.ENEMY_DATA >( );
		data.sb.list = new List< ENEMY_GENERATOR.ENEMY_DATA >( );
		data.vo.list = new List< ENEMY_GENERATOR.ENEMY_DATA >( );

		while ( !sr.EndOfStream ) {
            ENEMY_GENERATOR.ENEMY_DATA enemyData_1;
            ENEMY_GENERATOR.ENEMY_DATA enemyData_2;
            ENEMY_GENERATOR.ENEMY_DATA enemyData_3;

			// ファイルから一行読み込む
			string line = sr.ReadLine( );

			// 読み込んだ一行をカンマ毎に分けて配列に格納する
			string[ ] values = line.Split( ',' );

            /////////////// リズム1個目 ////////////////

			// リズム番号の取得
			enemyData_1.rhythm_num = int.Parse( values[ 0 ] );

			// 生成タイプの取得
			enemyData_1.obj_type = values[ 1 ];

			// 生成位置の取得
			enemyData_1.create_pos.x = float.Parse( values[ 2 ] );
			enemyData_1.create_pos.y = float.Parse( values[ 3 ] );
			enemyData_1.create_pos.z = float.Parse( values[ 4 ] );

			// 方向の取得
			enemyData_1.start_dir.x = float.Parse( values[ 5 ] );
			enemyData_1.start_dir.y = float.Parse( values[ 6 ] );
			enemyData_1.start_dir.z = float.Parse( values[ 7 ] );

			// スピードの取得
			enemyData_1.speed = float.Parse( values[ 8 ] );

			// ターゲットの取得
			enemyData_1.target_type = values[ 9 ];

			// 追加
			data.ma.list.Add( enemyData_1 );

            /////////////// リズム２個目 ////////////////

            // リズム番号の取得
			enemyData_2.rhythm_num = int.Parse( values[ 10 ] );

			// 生成タイプの取得
			enemyData_2.obj_type = values[ 11 ];

			// 生成位置の取得
			enemyData_2.create_pos.x = float.Parse( values[ 12 ] );
			enemyData_2.create_pos.y = float.Parse( values[ 13 ] );
			enemyData_2.create_pos.z = float.Parse( values[ 14 ] );

			// 方向の取得
			enemyData_2.start_dir.x = float.Parse( values[ 15 ] );
			enemyData_2.start_dir.y = float.Parse( values[ 16 ] );
			enemyData_2.start_dir.z = float.Parse( values[ 17 ] );

			// スピードの取得
			enemyData_2.speed = float.Parse( values[ 18 ] );

			// ターゲットの取得
			enemyData_2.target_type = values[ 19 ];

			// 追加
			data.sb.list.Add( enemyData_2 );

              /////////////// リズム3個目 ////////////////

            // リズム番号の取得
			enemyData_3.rhythm_num = int.Parse( values[ 20 ] );

			// 生成タイプの取得
			enemyData_3.obj_type = values[ 21 ];

			// 生成位置の取得
			enemyData_3.create_pos.x = float.Parse( values[ 22 ] );
			enemyData_3.create_pos.y = float.Parse( values[ 23 ] );
			enemyData_3.create_pos.z = float.Parse( values[ 24 ] );

			// 方向の取得
			enemyData_3.start_dir.x = float.Parse( values[ 25 ] );
			enemyData_3.start_dir.y = float.Parse( values[ 26 ] );
			enemyData_3.start_dir.z = float.Parse( values[ 27 ] );

			// スピードの取得
			enemyData_3.speed = float.Parse( values[ 28 ] );

			// ターゲットの取得
			enemyData_3.target_type = values[ 29 ];

			// 追加
			data.vo.list.Add( enemyData_3 );
		}

		return data;
	}

	/// <summary>
	/// ファイルデータの取得
	/// </summary>
	/// <returns></returns>
	private FILE_DATA getFileData( ) {
		FILE_DATA data = new FILE_DATA( );
		if ( _file.isData( ) ) {
			data = _file.getData( );
		}

		return data;
	}

	/// <summary>
	/// リズムデータの取得
	/// </summary>
	/// <returns></returns>
	public FILE_DATA.RHYTHM getRhythmData( ) {
		return getFileData( ).rhythm;
	}

	public ENEMY_GENERATOR.ENEMY_DATA getRhythmForNum( int num, RhythmManager.RHYTHM_TAG tag ) {
		ENEMY_GENERATOR.ENEMY_DATA enemy_data = new ENEMY_GENERATOR.ENEMY_DATA( );

		if ( tag == RhythmManager.RHYTHM_TAG.MAIN ) {
			enemy_data = getFileData( ).enemy.ma.list[ num ];
		} else if ( tag == RhythmManager.RHYTHM_TAG.SUB ) {
			enemy_data = getFileData( ).enemy.sb.list[ num ];
		} else if ( tag == RhythmManager.RHYTHM_TAG.VOCAL ) {
			enemy_data = getFileData( ).enemy.vo.list[ num ];
		}

		return enemy_data;
	}

	public int getRhythmCount( RhythmManager.RHYTHM_TAG tag ) {
		int count = 0;

		if ( tag == RhythmManager.RHYTHM_TAG.MAIN ) {
			count = getFileData( ).enemy.ma.list.Count;
		} else if ( tag == RhythmManager.RHYTHM_TAG.SUB ) {
			count = getFileData( ).enemy.sb.list.Count;
		} else if ( tag == RhythmManager.RHYTHM_TAG.VOCAL ) {
			count = getFileData( ).enemy.vo.list.Count;
		}

		return count;
	}
}
