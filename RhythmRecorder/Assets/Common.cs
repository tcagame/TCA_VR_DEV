using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Common {
	// 構造体
	public struct FILE_DATA {
		public int index;	// インデックス
		public uint frame;	// 再生してからのフレーム数
	}
	public class FileManager {

		/// <summary>
		/// ファイルのセーブ
		/// </summary>
		/// <param name="fileName"> ファイルの名前 </param>
		/// <param name="list"> ファイルデータ型のリスト </param>
		public static bool saveFile( string fileName, ref List< FILE_DATA > list ) {
			try {
				StreamWriter sw = new StreamWriter( Application.dataPath + "/" + fileName + ".csv", false );
				for ( int i = 0; i < list.Count; i++ ) {
					sw.Write( list[ i ].index );
					sw.Write( "," );
					sw.WriteLine( list[ i ].frame );
				}
				sw.Close( );

				return true;
			} catch {
				Debug.LogError( "Missing Save File..." );
				return false;
			}
		}

		/// <summary>
		/// ファイルのロード
		/// </summary>
		/// <param name="fileName"> ファイルの名前 </param>
		/// <param name="list"> ファイルデータ型のリスト </param>
		public static bool loadFile( string fileName, ref List< FILE_DATA > list ) {
			try {
				StreamReader sr = new StreamReader( Application.dataPath + "/" + fileName + ".csv" );

				// リストを空にする
				list.Clear( );	

				while ( !sr.EndOfStream ) {
					FILE_DATA data;

					// ファイルから一行読み込む
					string line = sr.ReadLine( );

					// 読み込んだ一行をカンマ毎に分けて配列に格納する
					string[ ] values = line.Split( ',' );

					// インデックスの取得
					data.index = int.Parse( values[ 0 ] );

					// 時間の取得
					data.frame = uint.Parse( values[ 1 ] );

					// リストの追加
					list.Add( data );
				}
				sr.Close( );
				return true;
			} catch {
				Debug.LogError( "Missing Load File..." );
				return false;
			}
		}
	}
}