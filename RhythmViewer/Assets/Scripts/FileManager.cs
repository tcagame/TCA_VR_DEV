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
		private TAG _tag = TAG.MELODY;	// タグ

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
			frag = ( _data.enemyGenerator.list != null )? true : false;	// エネミージェネレーターの配列確認
			frag = ( _data.rhythnData.array != null )? true : false;		// リズム配列の確認
			return frag;
		}

		/// <summary>
		/// タグの取得
		/// </summary>
		/// <returns></returns>
		public TAG getTag( ) {
			return _tag;
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
	private File[ ] _files;

	public enum TAG {
		MELODY,
		BASE,
	}

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
		foreach ( File file in _files ) {
			// データ確認
			if ( !file.isData( ) ) {
				loadFile( file );// ロード
			}
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
			data.rhythnData = getLoadFileRhythmData( ref sr );
				
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
		int size = int.Parse( values[ 0 ] );

		// 配列確保
		data.array = new TIMING_DATA[ size ];

		for ( int i = 0; i < size; i++ ) {
			// ファイルから一行読み込む
			str = sr.ReadLine( );

			// 読み込んだ一行をカンマ毎に分けて配列に格納する
			values = str.Split( ',' );

			// インデックスの取得
			data.array[ i ].index = int.Parse( values[ 0 ] );

			// 時間の取得
			data.array[ i ].frame = uint.Parse( values[ 1 ] );
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
			FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA enemyData;

			// ファイルから一行読み込む
			string line = sr.ReadLine( );

			// 読み込んだ一行をカンマ毎に分けて配列に格納する
			string[ ] values = line.Split( ',' );

			// リズム番号の取得
			enemyData.rhythm_num = int.Parse( values[ 0 ] );

			// 生成タイプの取得
			enemyData.obj_type = values[ 1 ];

			// 生成位置の取得
			enemyData.create_pos.x = float.Parse( values[ 2 ] );
			enemyData.create_pos.y = float.Parse( values[ 3 ] );
			enemyData.create_pos.z = float.Parse( values[ 4 ] );

			// 方向の取得
			enemyData.start_dir.x = float.Parse( values[ 5 ] );
			enemyData.start_dir.y = float.Parse( values[ 6 ] );
			enemyData.start_dir.z = float.Parse( values[ 7 ] );

			// スピードの取得
			enemyData.speed = float.Parse( values[ 8 ] );

			// ターゲットの取得
			enemyData.target_type = values[ 9 ];

			// 追加
			data.list.Add( enemyData );
		}

		return data;
	}

	/// <summary>
	/// ファイルデータの取得
	/// </summary>
	/// <returns></returns>
	private FILE_DATA getFileData( TAG tag ) {
		FILE_DATA data = new FILE_DATA( );
		foreach ( File file in _files ) {
			if ( file.getTag( ) == tag ) {
				data = file.getData( );
				break;
			}
		}

		return data;
	}

	/// <summary>
	/// リズムデータの取得
	/// </summary>
	/// <returns></returns>
	public FILE_DATA.RHYTHM getRhythmData( TAG tag ) {
		return getFileData( tag ).rhythnData;
	}

	public FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA getRhythmForNum( TAG tag,int num ) {
		return getFileData( tag ).enemyGenerator.list[ num ];
	}

	public int getRhythmCount( TAG tag ) {
		return getFileData( tag ).enemyGenerator.list.Count;
	}
}
