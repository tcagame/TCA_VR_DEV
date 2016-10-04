using UnityEngine;
using System.Collections;
using Common;

public class Group : MonoBehaviour {
	
	public const int MEMBER_NUM = 5;


	[ SerializeField ]
	private Transform[ ] _member_pos = new Transform[ MEMBER_NUM ];
    private GameObject[ ] _member = new GameObject[ MEMBER_NUM ];
	[ SerializeField ]
    private GROUP_TYPE _group_type;
	[ SerializeField ]
    private DANCE_TYPE _dance_type;
	[ SerializeField ]
    private int _dance_count;			// ダンスを進めるカウント
	[ SerializeField ]
	private int _part_count;			// パートを進めるカウント
    private bool _dance_finish;
	private bool _finish_dance_part;
	// Use this for initialization
	void Start( ) {
        _dance_finish = false;
		_finish_dance_part = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Transform getMemberPos( int num ) {
		return _member_pos[ num ];
	}

    public void setMemberPos( int member_num, Vector3 pos ) {
        _member_pos[ member_num ].transform.localPosition = pos;
	}

    public GROUP_TYPE getGroupType( ) {
        return _group_type;
    }

    public void setGroupType( GROUP_TYPE type ) {
        _group_type = type;
    }

    public GameObject getMember( int member_num ) {
        return _member[ member_num ];
    }

    public void setMember( int member_num, GameObject obj ) {
        _member[ member_num ] = obj;
    }

    public DANCE_TYPE getDanceType( ) {
        return _dance_type;
    }

    public void setDanceType( DANCE_TYPE dance_type ) {
        _dance_type = dance_type;
    }

    public int getDanceCount( ) {
        return _dance_count;
    }

    public void resetDanceCount( ) {
        _dance_count = 0;
    }

    public void updateDanceCount( ) {
        _dance_count++;
    }

	public int getPartCount( ) {
		return _part_count;
	}

	public void resetPartCount( ) {
		_part_count = 0;
	}

	public void updatePartCount( ) {
		_part_count++;
	}


    public bool isFinishDance( ) {
        if ( _dance_finish == true ) {
			_dance_finish = false;
			return true;
		}
		return false;
    }

    public void setDanceFinish( bool finish ) {
        _dance_finish = finish;
    }

	public bool isFinishDancePart( ) {
		return _finish_dance_part;
    }

	public void finishDancePart( ) {
		_finish_dance_part = true;
	}
}
