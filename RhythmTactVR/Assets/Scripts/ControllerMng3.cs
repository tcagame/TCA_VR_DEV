﻿//ControllerManeger3  (base SteamVR_TestThrow.cs)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControllerMng3 : MonoBehaviour {

//    //Particle発動
//    [SerializeField]
//    ParticleSystem[] ParSys;

    public JointAnchor_cube b_prefab;
    public Rigidbody attachPoint;

    SteamVR_TrackedObject trackedObj;

    FixedJoint joint;
    public JointAnchor_cube _base_GO;
    public List<JointAnchor_cube> _base_GO_List;

    public ushort shakerange;
    public int shaketime;
    private int shaketimecount;
    public LayerMask mask;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        _base_GO_List = new List<JointAnchor_cube>();

        shaketimecount = 0;
    }

    void FixedUpdate()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        //Cube掴む？
        if (joint == null)
        {
            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bhit = Physics.Raycast(raycast, out hit, 0.75f, mask.value);
            if (bhit == true)
            {
                _base_GO = GameObject.Instantiate(b_prefab);    //味方のアンカーcube生成

                var go = hit.transform.gameObject;               //hitした敵cube

                _base_GO.transform.position = go.transform.position;
                _base_GO.Set_CTRL(this.transform);
                _base_GO.Set_Enemy(go);

                //hitした瞬間の角速度を、Cubeのrigidbodyに伝える。
                var rb = go.GetComponent<Rigidbody>();
                var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
                if (origin != null)
                {
                    rb.angularVelocity = origin.TransformVector(device.angularVelocity);
                    rb.velocity = Vector3.zero;
                }  else {
                    rb.angularVelocity = device.angularVelocity;
                    rb.velocity = Vector3.zero;
                }
                rb.maxAngularVelocity = rb.angularVelocity.magnitude;

                // タッチパッドに触れたから振動開始
                SetShakeCTRLTime();

                _base_GO_List.Add(_base_GO);                    //味方cubeを配列登録
                go.layer = LayerMask.NameToLayer("nohit");   //敵cubeの衝突判定を無効にするため、レイヤーマスク値を変更

                //暫定的に敵キューブを2秒後に殺す。
                Object.Destroy(go, 2f);

                joint = null;

                //Hitパーティクルを生成
                //                {
                //                    ParSys[0].transform.position = raycast.origin + hit.distance * raycast.direction.normalized;
                //                    ParSys[0].Play();
                //                    //ParSys[1].transform.rotation = Quaternion.Euler();
                //                    ParSys[1].transform.position = raycast.origin + hit.distance * raycast.direction.normalized;
                //                    ParSys[1].Play();
                //                }
            }

            UpdateShakeCTRL();

        }
        //それ以外は、Fixedjointが有効なら座標と角度をコントローラと同期する。
        //joint中なら２拍子のリズムチェックを行う。
        else if (joint != null)
        {

        }
    }

    //コントローラの振動
    void UpdateShakeCTRL()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (shaketimecount > 0)
        {
            shaketimecount--;
            device.TriggerHapticPulse(shakerange);
        }

    }

    //コントローラの振動時間セット
    void SetShakeCTRLTime()
    {
        shaketimecount = shaketime;
    }

}
