//タクトの振りリズム判定スクリプト
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class RhythmCTRL_MNG : MonoBehaviour {

//	public float VeloTriggerClearLine;	//振り速度の合格ライン
//	public float PreVelo;				//前回のvelocity;
//	public float VeloTimer;				//振りトリガーONからの経過時間
//	public float VeloTimerRhythmRange;	//振りトリガーからリズムとしての有効時間

    public Vector3 Pre_velocity;
    public Vector3 Pre_angularVelocity;
    public float Pre_maxAngularVelocity;
    public float Pre_magnitude;

    public SteamVR_TrackedObject trackedObj;

    public ControllerMng3 mng3Obj;

    void Awake()
    {
//        trackedObj = GetComponent<SteamVR_TrackedObject>();
//        mng3Obj = GetComponent<ControllerMng3>();
    }

    void FixedUpdate()
    {
        VeloUpdate();       //リズム振り速度更新
    }

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

    void Update()
    {
        Debug.DrawLine(transform.position, (transform.position - Pre_velocity), Color.yellow);
        Debug.DrawLine(transform.position, (transform.position - Pre_velocity), Color.yellow);

        //transform.localScale = new Vector3(transform.localScale.x, Vector3.Distance(Vector3.zero,Pre_velocity) + 0.1f, transform.localScale.z);
        //transform.localScale = Pre_velocity;
        transform.localScale = Pre_angularVelocity;

    }

}
