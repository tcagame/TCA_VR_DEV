using UnityEngine;
using System.Collections;

public class ChkDestroy : MonoBehaviour {

    GameObject scenemng;

    // Use this for initialization
	void Start () {
        scenemng = GameObject.Find("SceneMng");
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (scenemng.GetComponent<SceneMng>().IsCubeDestroy() )
        {
            Object.Destroy(this.transform.gameObject, 0.1f);      //0秒後に消滅
        }
        */
    }
}
