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

	// ファイルデータ
	public struct FILE_DATA {
		#region リズム 構造体
		public struct RHYTHM {
			public TIMING_DATA[ ] ma;	// メイン４拍子配列
			public TIMING_DATA[ ] sb;   // サブリズム配列
            public TIMING_DATA[ ] vo;   // ボーカル配列
            public TIMING_DATA[ ] md;   // モード配列
            public TIMING_DATA[ ] ga;   // 群れアニメ配列
		}
		#endregion
		
		
		#region エネミー 構造体
		public struct ENEMY_FOR_RHYTHM {
			public ENEMY_GENERATOR ma;	// メイン４拍子配列
			public ENEMY_GENERATOR sb;   // サブリズム配列
            public ENEMY_GENERATOR vo;   // ボーカル配列
		}
		#endregion

		public RHYTHM rhythm;
		public ENEMY_FOR_RHYTHM enemy;
	}

    public enum MODE_CHANGE_NUM { 
        A_PART_START,
        A_PART_FINISH,
        SABI_START,
        SABI_FINISH,
    };

    public enum MUSIC_MODE {
        MODE_NONE,
        MODE_A_PART,
        MODE_B_PART,
        MODE_SABI,
        MODE_C_PART
    };
	
}