using UnityEngine;
using System.Collections;
using System.IO;
using Common;

public class EditFileManager : FileManager {

	public bool saveRhythm( FILE_DATA.RHYTHM data ) {
		try {
			StreamWriter sw = new StreamWriter( Application.dataPath + "/" + _file.getName( ) + ".csv", false );

			// 個数の書き込み
			sw.WriteLine( data.md.Length );

			// タイミングデータを書き込み
			for ( int i = 0; i < data.md.Length; i++ ) {
				sw.Write( data.md[ i ].index );
				sw.Write( "," );
				sw.WriteLine( data.md[ i ].frame );
			}

			// エネミーデータの書き込み
			for ( int i = 0; i < getRhythmCount( ); i++ ) {
				// エネミーデータを取得
				FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA enemyData = getRhythmForNum( i );

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
		} catch {
			Debug.LogError( "Missing Save File..." );
			return false;
		}

	}
}
