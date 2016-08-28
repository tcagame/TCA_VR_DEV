using UnityEngine;
using System.Collections;
using Common;

public class RhythmViewer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// 周波数の表示
	/// </summary>
	/// <param name="data"> 周波数のデータ配列 </param>
	public void drawFrequency( ref TIMING_DATA[ ] data, int index, int frame ) {
		int scale = 100;
		int barHight = 1;
		float width = 0.2f;
		for ( int i = 0; i < scale; i++ ) {
			for ( int j = index; j < data.Length / 2; j++ ) {
				// タイミングのライン
				if ( i + frame  == data[ index ].frame ) {
					Debug.DrawLine(
							new Vector3( i * width, barHight, 0 ), 
							new Vector3( i * width, -barHight, 0 ), 
							Color.red );
				}
			}
			// 下地のライン
			Debug.DrawLine(
					new Vector3( i * width , 0, 0 ), 
					new Vector3( ( i + 1 ) * width, 0, 0 ), 
					Color.cyan );
		}
	}

}
