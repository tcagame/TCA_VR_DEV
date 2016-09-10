using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {
	
	public const int MEMBER_NUM = 5;

	[ SerializeField ]
	private Transform[ ] _member_pos = new Transform[ MEMBER_NUM ];
	// Use this for initialization
	void Start () {
	
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
}
