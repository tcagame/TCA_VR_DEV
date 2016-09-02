//タクトの振りリズム判定スクリプト hamada
using UnityEngine;
using System.Collections;

/*
//----------------------------------------------------------------------------
//タクトクラス start 作りかけ 8/31
//----------------------------------------------------------------------------
public class TaktSwing
{
    //public Vector3[] ary_taktswing;
    //public int key_head;
    //public int key_tail;

    public TaktSwing()
    {
        //ary_taktswing = new Vector3[256];
    }

}
//----------------------------------------------------------------------------
//タクトクラス end
//----------------------------------------------------------------------------
*/

//[RequireComponent(typeof(SteamVR_TrackedObject))]
public class RhythmCTRL_MNG : MonoBehaviour {


    //	public float VeloTriggerClearLine;	//振り速度の合格ライン
    //	public float PreVelo;				//前回のvelocity;
    //	public float VeloTimer;				//振りトリガーONからの経過時間
    //	public float VeloTimerRhythmRange;	//振りトリガーからリズムとしての有効時間

    public bool flg_RhythmTaktChk;
    public Vector3 Pre_velocity;
    public Vector3 Pre_angularVelocity;
    public float Pre_maxAngularVelocity;
    public float Pre_magnitude;

    //  public TaktSwing taktswing;
    public Vector3[] ary_taktswing;
    public int key_pointer;
    public int key_head;
    public int key_tail;

    public ControllerMng3 mng3Obj;
    public SteamVR_TrackedObject trackedObj;


    //----------------------------------------------------------------------------
    //公開関数
    //----------------------------------------------------------------------------
    void Awake()
    {
        //        trackedObj = GetComponent<SteamVR_TrackedObject>();
        //        mng3Obj = GetComponent<ControllerMng3>();

        //  taktswing = new TaktSwing();
        ary_taktswing = new Vector3[256];
        key_pointer = key_head = key_tail = 0;
    }

    //----------------------------
    void FixedUpdate()
    {
        VeloUpdate();       //リズム振り速度更新

        RecVelo();          //振り角速度を配列に保存

//        Update_KeyHead();

        RhythmTaktChk();
    }


    //----------------------------
    //振りタクトのリズム判定
    void RhythmTaktChk()
    {
        //リズムマネージャから一番近いキーフーレム情報を取得

        //今のフレームがリズムキーフレームじゃなかったらreturn

        //リズムマネージャから１つ後ろキーフーレムの後ろ１つの情報を取得

        //２つのキーフレームの間の時間の、タクトの振り角度の配列を取得
        //1つ前のキーをkey_head,一番近いキーをkeytailをする。

        //間の配列をすべて確認し、角速度の差分が多ければリズムOKとする。
        //違うならリズムNGとする。
    }

    //----------------------------
    //判定用頭キーの更新
    void Update_KeyHead()
    {
        //if (false)
        {
            key_head = key_pointer;
        }
    }

    //----------------------------
    //振り角速度を配列に保存
    void RecVelo()
    {
        ary_taktswing[key_pointer++] = Pre_angularVelocity;
        if(key_pointer > ary_taktswing.Length)
        {
            key_pointer = 0;
        }
    }

    //----------------------------
    //リズム振り速度更新
    void VeloUpdate()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        var rb = GetComponent<Rigidbody>(); //伝えたい物体のrigidbody 今は仮で自分自身のrigidbody
        var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
        if (origin != null)
        {
            //    rb.velocity = origin.TransformVector(device.velocity);
            //    rb.angularVelocity = origin.TransformVector(device.angularVelocity);
            Pre_velocity = origin.TransformVector(device.velocity);
            Pre_angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            //    rb.velocity = device.velocity;
            //    rb.angularVelocity = device.angularVelocity;
            Pre_velocity = device.velocity;
            Pre_angularVelocity = device.angularVelocity;
        }
        //rb.maxAngularVelocity = rb.angularVelocity.magnitude;
        Pre_maxAngularVelocity = rb.angularVelocity.magnitude;
        //Pre_magnitude = Pre_velocity.magnitude;
    }

    //----------------------------
    void Update()
    {
        Debug.DrawLine(transform.position, (transform.position - Pre_velocity), Color.yellow);

        //transform.localScale = new Vector3(transform.localScale.x, Vector3.Distance(Vector3.zero,Pre_velocity) + 0.1f, transform.localScale.z);
        //transform.localScale = Pre_velocity;
        transform.localScale = Pre_angularVelocity;

    }

}
