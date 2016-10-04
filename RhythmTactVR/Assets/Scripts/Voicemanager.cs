using UnityEngine;
using System.Collections;

public class Voicemanager : MonoBehaviour {

	[ SerializeField ]
	private RhythmManager _rhythm_manager;
	public GameObject Tokyo;
	public AudioClip _audio_clip;
	// Use this for initialization
	void Start () {
		Tokyo.SetActive( false );
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if( _rhythm_manager.isTiming( RhythmManager.RHYTHM_TAG.VOCAL ) ) {
			if( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.VOCAL ) == 276 ){
				Tokyo.SetActive( true );
			}
			if( _rhythm_manager.getIndex( RhythmManager.RHYTHM_TAG.VOCAL ) == 278 ) {
				Debug.Log("Tokyo");
				Tokyo.GetComponent< AudioSource >().clip = _audio_clip;
				Tokyo.GetComponent< AudioSource >().PlayOneShot( _audio_clip );
			}
		}
	
	}
}
