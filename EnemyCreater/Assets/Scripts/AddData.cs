using UnityEngine;
using System.Collections;

public class AddData : MonoBehaviour {

    public enum OBJ_TYPE { 
        FAST_MIDDLE,
        SLOW_MIDDLE,
        FAST_SMALL,
        SLOW_SMALL,
    };

    public enum TARGET_TYPE { 
        CENTER,
        NORTH,
        NORTH_EAST,
        EAST,
        SOUTH_EAST,
        SOUTH,
        SOUTH_WEST,
        WEST,
        NORTH_WEST,
        MAX_TARGET_NUM
    };

	public RhythmData _rhythm_data;
	Vector3[ ] _create_pos = new Vector3[ ( int )TARGET_TYPE.MAX_TARGET_NUM ];
    private OBJ_TYPE _obj_type;
	// Use this for initialization
	void Start( ) {
	    _create_pos[ 0 ] = new Vector3( 0, 0, 10 );
	    _create_pos[ 1 ] = new Vector3( 0, 5, 10 );
	    _create_pos[ 2 ] = new Vector3( 5, 5, 10 );
	    _create_pos[ 3 ] = new Vector3( 5, 0, 10 );
	    _create_pos[ 4 ] = new Vector3( 5, -5, 10 );
	    _create_pos[ 5 ] = new Vector3( 0, -5, 10 );
	    _create_pos[ 6 ] = new Vector3( -5, -5, 10 );
	    _create_pos[ 7 ] = new Vector3( -5, 0, 10 );
	    _create_pos[ 8 ] = new Vector3( -5, 5, 10 );
	}
	
	// Update is called once per frame
	void Update( ) {
		if ( Input.GetMouseButtonDown( 0 ) ) {
			//randomCreate( );
		}
        if ( Input.GetKeyDown( KeyCode.F1 ) ) {
            _obj_type = OBJ_TYPE.FAST_MIDDLE;
        }
        if ( Input.GetKeyDown( KeyCode.F2 ) ) {
            _obj_type = OBJ_TYPE.SLOW_MIDDLE;
        }
        if ( Input.GetKeyDown( KeyCode.F3 ) ) {
            _obj_type = OBJ_TYPE.FAST_SMALL;
        } 
        if ( Input.GetKeyDown( KeyCode.F4 ) ) {
            _obj_type = OBJ_TYPE.SLOW_SMALL;
        }

	}

	void randomCreate( ) {
		// 範囲の宣言
		float range_x = 0.0f;
		float range_y = 0.0f;
		float range_z = 0.0f;
	
		// 範囲の設定
		Vector3 pos = new Vector3( );
		pos.x = Random.Range( -range_x, range_x );
		pos.y = Random.Range( -range_y, range_y );
		pos.z = Random.Range( -range_z, range_z );
	
		// リズムの追加
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = pos;
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "BODY";
		_rhythm_data.add( rhythm );
	}

	
	public void pushNumOne( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 0 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "CENTER";
		_rhythm_data.add( rhythm );
	}
	
	public void pushNumTwo( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 1 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 15.0f;
		rhythm.target_type = "NORTH";
		_rhythm_data.add( rhythm );
	}
	
	public void pushNumThree( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 2 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "NORTH_EAST";
		_rhythm_data.add( rhythm );
	}
    
	public void pushNumFour( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 3 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "EAST";
		_rhythm_data.add( rhythm );
	}
    
	public void pushNumFive( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 4 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "SOUTH_EAST";
		_rhythm_data.add( rhythm );
	}
    
	public void pushNumSix( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 5 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "SOUTH";
		_rhythm_data.add( rhythm );
	}
    
	public void pushNumSeven( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 6 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "SOUTH_WEST";
		_rhythm_data.add( rhythm );
	}
    
	public void pushNumEight( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 7 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "WEST";
		_rhythm_data.add( rhythm );
	}
    
	public void pushNumNine( ) {
		RhythmData.RHYTHM_DATA rhythm = new RhythmData.RHYTHM_DATA( );
		rhythm.obj_type    = getObjType( );
		rhythm.create_pos  = _create_pos[ 8 ];
		rhythm.start_dir   = new Vector3( 10, 0, 0 );
		rhythm.speed       = 10.0f;
		rhythm.target_type = "NORTH_WEST";
		_rhythm_data.add( rhythm );
	}

	public void pushAllDelete( ) {
		_rhythm_data.allDelete( );
	}

	public void pushSave( ) {
		_rhythm_data.saveFile( );
	}

    string getObjType( ) {
        string obj_type = "";

        switch ( _obj_type ) {
            case OBJ_TYPE.FAST_MIDDLE:
                obj_type = "FAST_MIDDLE";
                break;
            case OBJ_TYPE.SLOW_MIDDLE:
                obj_type = "SLOW_MIDDLE";
                break;
            case OBJ_TYPE.FAST_SMALL:
                obj_type = "FAST_SMALL";
                break;
            case OBJ_TYPE.SLOW_SMALL:
                obj_type = "SLOW_SMALL";
                break;
        }

        return obj_type;
    }
}
