//ControllerManeger  (base SteamVR_TestThrow.cs)
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControllerMng2 : MonoBehaviour
{

	//Particle発動
	[SerializeField] ParticleSystem[] ParSys;

    //    public GameObject b_prefab;
    public JointAnchor_cube b_prefab;
    public Rigidbody attachPoint;

    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;
	//public SteamVR_Camera VR_camera;
    //  GameObject _base_GO;
    JointAnchor_cube _base_GO;
	public LayerMask mask;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
		//VR_camera = GetComponent<SteamVR_Camera>();
    }

    void FixedUpdate()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        //Cube掴む？
        //  if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        if (joint == null && device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bhit = Physics.Raycast(raycast, out hit, 0.5f, mask.value);
            if(bhit == true){

                _base_GO = GameObject.Instantiate(b_prefab);
                
                //  _base_GO.transform.position = attachPoint.transform.position;

                var go = hit.transform.gameObject;
                _base_GO.transform.position = go.transform.position;
                _base_GO.Set_CTRL(this.transform); ;
                //            var go = GameObject.Instantiate(prefab);
                //           go.transform.position = attachPoint.transform.position;

                joint = go.AddComponent<FixedJoint>();
                //  joint.connectedBody = attachPoint;
                joint.connectedBody = _base_GO.GetComponent<Rigidbody>();

                //Hitパーティクルを生成
                {
					ParSys[0].transform.position = raycast.origin + hit.distance * raycast.direction.normalized;
					ParSys[0].Play();
					//ParSys[1].transform.rotation = Quaternion.Euler();
					ParSys[1].transform.position = raycast.origin + hit.distance * raycast.direction.normalized;
					ParSys[1].Play();
                }
            }

        }
        //Cube外す？
        else if (joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            var go = joint.gameObject;
            var rigidbody = go.GetComponent<Rigidbody>();

            var _bgo = joint.connectedBody.transform.gameObject;

            Object.DestroyImmediate(joint);
            joint = null;
            //Object.Destroy(go, 15.0f);      //１５秒後に消滅

            Object.Destroy(_bgo, 0f);      //0秒後に消滅
            //Object.Destroy(_base_GO, 1f);      //1秒後に消滅
            _base_GO = null;

            //離した瞬間の移動速度と角速度を、Cubeのrigidbodyに伝える。    

            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
            if (origin != null)
            {
        //        rigidbody.velocity = origin.TransformVector(device.velocity);
                rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
            }
            else
            {
        //        rigidbody.velocity = device.velocity;
                rigidbody.angularVelocity = device.angularVelocity;
            }

            rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
        }
        //それ以外は、Fixedjointが有効なら座標と角度をコントローラと同期する。
        //joint中なら２拍子のリズムチェックを行う。
        else if (joint != null)
        {
         
        }
    }

}
