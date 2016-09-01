using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Common {
	// タイミングデータ
	public struct TIMING_DATA {
		public int index;	// インデックス
		public uint frame;	// 再生してからのフレーム数
	}

	// ファイルデータ
	public struct FILE_DATA {
		#region リズム 構造体
		public struct RHYTHM {
			public TIMING_DATA[ ] md;	// メロディー配列
			public TIMING_DATA[ ] ba;	// ベース配列
		}
		#endregion
		
		#region エネミージェネレーター 構造体
		public struct ENEMY_GENERATOR {
			#region エネミーデータ
			public struct ENEMY_DATA {
				public int rhythm_num;
				public string obj_type;
				public Vector3 create_pos;
				public Vector3 start_dir;
				public float speed;
				public string target_type;
			}
			#endregion

			public List< ENEMY_DATA > list;
		}
		#endregion

		public RHYTHM rhythm;
		public ENEMY_GENERATOR enemyGenerator;
	}
	
}