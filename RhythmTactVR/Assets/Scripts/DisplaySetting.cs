using UnityEngine;
using System.Collections;

public class DisplaySetting : Manager< DisplaySetting > {

	protected override void initialize( ) {
	}

	void Start( ) {

        Debug.Log( "displays connected: " + Display.displays.Length );
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.

		// ディスプレイを起動
		for ( int i = 0; i < Display.displays.Length; i++ ) {
			Display.displays[ i ].Activate( );
		}
    }
}
