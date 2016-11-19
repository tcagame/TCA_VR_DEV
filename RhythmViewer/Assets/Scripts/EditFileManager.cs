using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;

public class EditFileManager : FileManager {

	// Awake関数の代わり
	protected override void initialize( ) {

	}

	public bool saveRhythm( List< TIMING_DATA > data ) {
		// ファイルパス
		string FILE_PATH = Application.dataPath + "/" + _file.getName( ) + ".csv";

		try {
			StreamWriter sw = new StreamWriter( FILE_PATH, false );

			// 個数の書き込み
			sw.WriteLine( data.Count );

			// タイミングデータを書き込み
			for ( int i = 0; i < data.Count; i++ ) {
				sw.Write( data[ i ].index );
				sw.Write( "," );
				sw.WriteLine( data[ i ].frame );
			}

			// エネミーデータの書き込み
			for ( int i = 0; i < getRhythmCount( ); i++ ) {
				// エネミーデータを取得
				FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA enemyData = getRhythmForNum( i );
				
				// 生成番号
				sw.Write( enemyData.rhythm_num );
				sw.Write( "," );
				// タイプ
				sw.Write( enemyData.obj_type );
				sw.Write( "," );
				// 生成位置
				sw.Write( enemyData.create_pos.x );
				sw.Write( "," );
				sw.Write( enemyData.create_pos.y );
				sw.Write( "," );
				sw.Write( enemyData.create_pos.z );
				sw.Write( "," );
				// 開始方向
				sw.Write( enemyData.start_dir.x );
				sw.Write( "," );
				sw.Write( enemyData.start_dir.y );
				sw.Write( "," );
				sw.Write( enemyData.start_dir.z );
				sw.Write( "," );
				// 移動スピード
				sw.Write( enemyData.speed );
				sw.Write( "," );
				// ターゲットタイプ
				sw.Write( enemyData.target_type );
				sw.Write( "," );
				sw.WriteLine( );
			}

			sw.Close( );

			return true;
		} catch( System.Exception exp ) {
			Debug.LogError( "ファイルのセーブに失敗しました。" + getErrorFileInfo( ref exp ) );
			return false;
		}

	}

	public new List< TIMING_DATA > getRhythmData( ) {
		FILE_DATA.RHYTHM data = _file.getData( ).rhythm;
		List< TIMING_DATA > tmp = new List< TIMING_DATA >( );
		tmp.AddRange( data.md );
		return tmp;
	}

	/// <summary>
	/// ファイルのロード
	/// </summary>
	/// <returns></returns>
	public bool loadFile( ) {
		return base.loadFile( _file );
	}
}
