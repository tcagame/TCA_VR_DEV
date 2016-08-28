using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Common {
	// 構造体
	public struct TIMING_DATA {
		public int index;	// インデックス
		public uint frame;	// 再生してからのフレーム数
	}

	// ファイルデータ
	public struct FILE_DATA {
		// リズムデータ
		public struct RHYTHM {
			public TIMING_DATA[ ] array;
		}
		public struct ENEMY_GENERATOR {
			public struct ENEMY_DATA {
				public int rhythm_num;
				public string obj_type;
				public Vector3 create_pos;
				public Vector3 start_dir;
				public float speed;
				public string target_type;
			}
			public List< ENEMY_DATA > list;
		}
		public RHYTHM rhythnData;
		public ENEMY_GENERATOR enemyGenerator;
	}
	
}