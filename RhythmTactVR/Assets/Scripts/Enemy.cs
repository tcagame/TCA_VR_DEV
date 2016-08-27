using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public enum OBJECT_TYPE {
		FAST_MIDDLE,
		SLOW_MIDDLE,
		FAST_SMALL,
		SLOW_SMALL,
	};

	public enum TARGET_TYPE {
		BODY,
		RIGHT_ARM,
		LEFT_ARM,
	};
	
	int _rhythm_num;
	OBJECT_TYPE _obj_type;
	TARGET_TYPE _target_type;
	Vector3 _start_dir;
	float _speed;
	bool _start = false;

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}
	
	public void setRhyrhmNum( int num ) {
		_rhythm_num = num;
	}

	public void setObjType( string type ) {
		if ( type == "FAST_MIDDLE" ) {
			_obj_type = OBJECT_TYPE.FAST_MIDDLE;
		} else if ( type == "SLOW_MIDDLE" ) {
			_obj_type = OBJECT_TYPE.SLOW_MIDDLE;
		} else if ( type == "FAST_SMALL" ) {
			_obj_type = OBJECT_TYPE.FAST_SMALL;
		} else if ( type == "SLOW_SMALL" ) {
			_obj_type = OBJECT_TYPE.SLOW_SMALL;
		}
	}

	public void setTargetType( string type ) {
		if ( type == "BODY" ) {
			_target_type = TARGET_TYPE.BODY;
		} else if ( type == "RIGHT_ARM" ) {
			_target_type = TARGET_TYPE.RIGHT_ARM;
		} else if ( type == "LEFT_ARM" ) {
			_target_type = TARGET_TYPE.LEFT_ARM;
		}
	}
	
	public void setDir( Vector3 dir ) {
		_start_dir = dir;
	}
	
	public void setSpeed( float speed ) {
		_speed = speed;
	}

	public void started( ) {
		_start = true;
	}

	public int getRhyrhmNum( ) {
		return _rhythm_num;
	}
	
	public OBJECT_TYPE getObjType( ) {
		return _obj_type;
	}

	public TARGET_TYPE getTargetType( ) {
		return _target_type;
	}
	
	public Vector3 getDir( ) {
		return _start_dir;
	}
	
	public float getSpeed( ) {
		return _speed;
	}

	public bool isStart( ) {
		return _start;
	}
}
