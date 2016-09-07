using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class FileManager : Manager< FileManager > {

	#region ファイルクラス
	[ System.Serializable ]
 	private class File {
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
            frag = (_data.enemyGenerator.list != null) ? true : false;	// エネミージェネレーターの配列確認
			frag = ( _data.rhythm.md != null )? true : false;		    // リズム配列の確認
			frag = ( _data.rhythm.be != null )? true : false;		    // ベース配列の確認
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
	private File _file = new File( );

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
			data.enemyGenerator = getLoadFileEnemyGeneratorData( ref sr );
				
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
	private FILE_DATA.RHYTHM getLoadFileRhythmData( ref StreamReader sr ) {
		FILE_DATA.RHYTHM data = new FILE_DATA.RHYTHM( );

		// 個数の取得
		string str = sr.ReadLine( );
		string[ ] values = str.Split( ',' );
		int md_size = int.Parse( values[ 0 ] );
		int be_size = int.Parse( values[ 2 ] );

		// 配列確保
		data.md = new TIMING_DATA[ md_size ];
		data.be = new TIMING_DATA[ be_size ];


		int length = 0;
        // 大きいほうを設定
        if ( md_size > be_size ) {
            length = md_size;
        } else if ( be_size > md_size ) {
            length = be_size;
        } else {
			length = md_size;
		}

		for ( int i = 0; i < length; i++ ) {
			// ファイルから一行読み込む
			str = sr.ReadLine( );

			// 読み込んだ一行をカンマ毎に分けて配列に格納する
			values = str.Split( ',' );

            // タイミングデータを書き込み
            if ( md_size < length && i >= md_size ) {
				// インデックスの取得
				data.be[ i ].index = int.Parse( values[ 2 ] );

				// 時間の取得
				data.be[ i ].frame = uint.Parse( values[ 3 ] );
            } else if ( be_size < length && i >= be_size ) {
				// インデックスの取得
				data.md[ i ].index = int.Parse( values[ 0 ] );

				// 時間の取得
				data.md[ i ].frame = uint.Parse( values[ 1 ] );
            } else {
				// インデックスの取得
				data.md[ i ].index = int.Parse( values[ 0 ] );

				// 時間の取得
				data.md[ i ].frame = uint.Parse( values[ 1 ] );
            
				// インデックスの取得
				data.be[ i ].index = int.Parse( values[ 2 ] );

				// 時間の取得
				data.be[ i ].frame = uint.Parse( values[ 3 ] );
            }
		}

		return data;
	}
		
	/// <summary>
	/// ロードしたファイルからエネミージェネレーターのデータを取得
	/// </summary>
	/// <returns></returns>
	private FILE_DATA.ENEMY_GENERATOR getLoadFileEnemyGeneratorData( ref StreamReader sr ) {
		FILE_DATA.ENEMY_GENERATOR data = new FILE_DATA.ENEMY_GENERATOR( );

		// リストの確保
		data.list = new List< FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA >( );

		while ( !sr.EndOfStream ) {
            FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA enemyData_1;
            FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA enemyData_2;

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
			data.list.Add( enemyData_1 );

            /////////////// リズム２個目 ////////////////

            // リズム番号の取得
			enemyData_2.rhythm_num = int.Parse( values[ 0 ] );

			// 生成タイプの取得
			enemyData_2.obj_type = values[ 1 ];

			// 生成位置の取得
			enemyData_2.create_pos.x = float.Parse( values[ 2 ] );
			enemyData_2.create_pos.y = float.Parse( values[ 3 ] );
			enemyData_2.create_pos.z = float.Parse( values[ 4 ] );

			// 方向の取得
			enemyData_2.start_dir.x = float.Parse( values[ 5 ] );
			enemyData_2.start_dir.y = float.Parse( values[ 6 ] );
			enemyData_2.start_dir.z = float.Parse( values[ 7 ] );

			// スピードの取得
			enemyData_2.speed = float.Parse( values[ 8 ] );

			// ターゲットの取得
			enemyData_2.target_type = values[ 9 ];

			// 追加
			data.list.Add( enemyData_2 );
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

	public FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA getRhythmForNum( int num ) {
		return getFileData( ).enemyGenerator.list[ num ];
	}

	public int getRhythmCount( ) {
		return getFileData( ).enemyGenerator.list.Count;
	}
}
