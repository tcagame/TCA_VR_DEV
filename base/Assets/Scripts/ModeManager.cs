using UnityEngine;
using System.Collections;
using Common;

public class ModeManager : MonoBehaviour {

    [SerializeField]
    private RhythmManager _rhythm_manager;
    [ SerializeField ]
    private MUSIC_MODE _music_mode = MUSIC_MODE.MODE_NONE;
 
    void Awake( ) {
        _music_mode = MUSIC_MODE.MODE_NONE;
    }
	// Use this for initialization
	void Start( ) {
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    void FixedUpdate( ) {
        if ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) && _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.A_PART_START ) {
            _music_mode = MUSIC_MODE.MODE_A_PART;
        }
        if ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) && _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.A_PART_FINISH ) {
            _music_mode = MUSIC_MODE.MODE_B_PART;
        }
        if ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) && _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.SABI_START ) {
            _music_mode = MUSIC_MODE.MODE_SABI;
        }
        if ( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) && _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.MODE_CHANGE ) == ( int )MODE_CHANGE_NUM.SABI_FINISH ) {
            _music_mode = MUSIC_MODE.MODE_C_PART;
        }
    }

    public MUSIC_MODE getMusicMode( ) {
        return _music_mode;
    }
}
