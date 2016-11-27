﻿using UnityEngine;
using System.Collections;
using Common;

public class Enemy : MonoBehaviour {

	public enum OBJECT_TYPE {
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
	
	private OBJECT_TYPE _obj_type;
	private TARGET_TYPE _target_type;
	private Vector3 _start_dir;
	private int _rhythm_num;
	private float _speed;
	private bool _start = false;

	private ModeManager _modemanager;
	private ControllerMng3 _ControllerMng3;
    private NeonShaderController _neon_shader_controller;

	public Vector3 ret_velo;
    public float Ref_speed;
	public Transform _target;
	public int _distance;
	public Vector3 _min_scale;
	public Vector3 _max_scale;

	// Use this for initialization
	void Awake( ) {

        GetComponentInChildren< Neon >( ).setChangeRhythmColoringMode( false ); // アニメーションカラーをOFF

		_modemanager = GameObject.Find( "ModeManager" ).GetComponent< ModeManager >( );
        _neon_shader_controller = GetComponentInChildren< NeonShaderController >( );
	    _ControllerMng3 = GameObject.Find( "Controller (left)" ).GetComponent< ControllerMng3 >( );
		_target = GameObject.Find( "Camera (eye)" ).GetComponent< Transform >( );
	
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		float distance = Vector3.Distance( this.transform.position, _target.position );
		if( distance > _distance ) {
			this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			if( this.transform.localScale.x < _min_scale.x && this.transform.localScale.y < _min_scale.y && this.transform.localScale.z < _min_scale.z ) {
				this.transform.localScale += this.transform.localScale * Time.deltaTime;
			}
		}
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
		switch ( type ) {
            case "CENTER":
                _target_type = TARGET_TYPE.CENTER;
                break;
            case "NORTH":
                _target_type = TARGET_TYPE.NORTH;
                break;
            case "NORTH_EAST":
                _target_type = TARGET_TYPE.NORTH_EAST;
                break;
            case "EAST":
                _target_type = TARGET_TYPE.EAST;
                break;
            case "SOUTH_EAST":
                _target_type = TARGET_TYPE.SOUTH_EAST;
                break;
            case "SOUTH":
                _target_type = TARGET_TYPE.SOUTH;
                break;
            case "SOUTH_WEST":
                _target_type = TARGET_TYPE.SOUTH_WEST;
                break;
            case "WEST":
                _target_type = TARGET_TYPE.WEST;
                break;
            case "NORTH_WEST":
                _target_type = TARGET_TYPE.NORTH_WEST;
                break;
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

    public void setNeonColor( Color color ) {
        _neon_shader_controller.setVertexColor( color );
    }

	public bool isStart( ) {
		return _start;
	}

	public void setNeonHitColor( Color color ) {
		_neon_shader_controller.setHitEffectColor( color );
	}  
}
