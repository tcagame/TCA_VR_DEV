using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RhythmData : ScriptableObject {
	
	[System.Serializable]
	public class RHYTHM_DATA {
		public string obj_type;
		public Vector3 create_pos;
		public Vector3 start_dir;
		public float speed;
		public string target_type;
        public int rhythm_num;
	}
	public List<RHYTHM_DATA> _rhythm = new List<RHYTHM_DATA>( );
	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {

	}

	public void saveFile( ) {
		StreamWriter sw = new StreamWriter( Application.dataPath + "/EnemyData.csv", false );
		for ( int i = 0; i < _rhythm.Count; i++ ) {
            // 生成リズム番号
            sw.Write( _rhythm[ i ].rhythm_num );
			sw.Write( "," );
			// タイプ
			sw.Write( _rhythm[ i ].obj_type );
			sw.Write( "," );
			// 生成位置
			sw.Write( _rhythm[ i ].create_pos.x );
			sw.Write( "," );
			sw.Write( _rhythm[ i ].create_pos.y );
			sw.Write( "," );
			sw.Write( _rhythm[ i ].create_pos.z );
			sw.Write( "," );
			// 開始方向
			sw.Write( _rhythm[ i ].start_dir.x );
			sw.Write( "," );
			sw.Write( _rhythm[ i ].start_dir.y );
			sw.Write( "," );
			sw.Write( _rhythm[ i ].start_dir.z );
			sw.Write( "," );
			// 移動スピード
			sw.Write( _rhythm[ i ].speed );
			sw.Write( "," );
			// ターゲットタイプ
			sw.Write( _rhythm[ i ].target_type );
			sw.Write( "," );
			sw.WriteLine( );
		}
		sw.Close( );
	}

	public void add( RHYTHM_DATA add_data ) {
		_rhythm.Add( add_data );
	}

	public void allDelete( ) {
		_rhythm.Clear( );
	}

	public void selectDelete( int num ) {
		_rhythm.RemoveAt( num );
	}

	public RHYTHM_DATA getRhythmForNum( int num ) {
		return _rhythm[ num ];
	}

	public int getRhythmCount( ) {
		return _rhythm.Count;
	}
}
