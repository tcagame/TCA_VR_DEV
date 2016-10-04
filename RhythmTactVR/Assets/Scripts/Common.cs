using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

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
    
    public enum DANCE_PART {
        NO_DANCE_PART = 0,
        PART_ONE = 1,
        PART_TWO = 2,
        PART_THREE = 3,
        PART_FOUR = 4,
        PART_FIVE = 5,
        PART_SIX = 6,
        DANCE_PART_NUM
    };

    // ダンスの種類
    public enum DANCE_TYPE { 
        DANCE_NONE        = 0,
        DANCE_ONE_ONE     = 1,
        DANCE_ONE_TWO     = 2,
        DANCE_ONE_THREE   = 3,
        DANCE_TWO_ONE     = 4,
        DANCE_THREE_ONE   = 5,
        DANCE_THREE_TWO   = 6,
        DANCE_FOUR_A_ONE  = 7,
        DANCE_FOUR_A_TWO  = 8,
        DANCE_FOUR_B_ONE  = 9,
        DANCE_FOUR_B_TWO  = 10,
        DANCE_FIVE_ONE    = 11,
        DANCE_FIVE_TWO    = 12,
        DANCE_FIVE_THREE  = 13,
        DANCE_FIVE_FOUR   = 14,
        DANCE_SIX_ONE     = 15,
        DANCE_SEVEN_A_ONE = 16,
        DANCE_SEVEN_A_TWO = 17,
        DANCE_SEVEN_B_ONE = 18,
        DANCE_SEVEN_B_TWO = 19,
        DANCE_EUGHT_ONE   = 20,
        DANCE_EIGHT_TWO   = 21,
        DANCE_NINE_ONE    = 22,
        DANCE_NINE_TWO    = 23,
        DANCE_NINE_THREE  = 24,
        DANCE_NINE_FOUR   = 25,
        DANCE_TEN_ONE     = 26,
        DANCE_FIN_ONE     = 27,
        DANCE_FIN_TWO     = 28,
        DANCE_TYPE_NUM    = 29,
    };
    
    public enum GROUP_TYPE {
        GROUP_A,
        GROUP_B,
        GROUP_C,
        GROUP_TYPE_NUM,
        GROUP_NONE,
    };

    public struct GROUP_DANCE_DATA {
        public GROUP_TYPE group_type;
        public List< DANCE_TYPE > dance_type;
    };
    
    public struct DANCE_PART_DATA {
        public DANCE_PART dance_part;
        public GROUP_DANCE_DATA[ ] group_data;
    };

    public struct DANCE_FILE_DATA {
        public struct DANCE_DATA { 
            public DANCE_PART_DATA[ ] dance_part;
        };

        public DANCE_DATA dance;
    };

    public enum MODE_CHANGE_NUM { 
        A_PART_START,
        A_PART_FINISH,
        SABI_START,
        SABI_FINISH,
    };

    public enum MUSIC_MODE {
        MODE_NONE = 0,
        MODE_A_PART = 1,
        MODE_B_PART = 2,
        MODE_SABI = 3,
        MODE_C_PART = 4
    };
	
}