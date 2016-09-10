using UnityEngine;
using System.Collections;

public class VRController : MonoBehaviour {

	// Use this for initialization
	void Start () {
       var trackedController = gameObject.GetComponent<SteamVR_TrackedController>();
       if (trackedController == null) {
           trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
       }

       trackedController.TriggerClicked += new ClickedEventHandler(ClickOn);
       trackedController.TriggerUnclicked += new ClickedEventHandler(ClickOff);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void ClickOn(object sender, ClickedEventArgs e)
    {
        Debug.Log("Click On!!");

    }

    void ClickOff(object sender, ClickedEventArgs e)
    {
        Debug.Log("Click Off!!");

    }
}
