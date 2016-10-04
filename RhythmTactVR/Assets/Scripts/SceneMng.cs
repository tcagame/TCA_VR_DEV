using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class SceneMng : MonoBehaviour {

    public SteamVR_TrackedObject R_TrackObj;
    public SteamVR_TrackedObject L_TrackObj;
    public bool flg_cubeDestroy;        //Cube全滅フラグ。

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var deviceR = SteamVR_Controller.Input((int)R_TrackObj.index);
        var deviceL = SteamVR_Controller.Input((int)L_TrackObj.index);

        if(deviceR.GetPress(SteamVR_Controller.ButtonMask.Touchpad) || deviceL.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            flg_cubeDestroy = true; //Cube全滅
        }else
        {
            flg_cubeDestroy = false;
        }


    }

    public bool IsCubeDestroy()
    {
        return flg_cubeDestroy;
    }

}
