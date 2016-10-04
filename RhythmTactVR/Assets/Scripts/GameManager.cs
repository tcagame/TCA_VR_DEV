using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private STATE _oldState = STATE.NONE;

	private bool _gameStart = false;
	private bool _gameFinish = false;

	public enum STATE {
		GAME_START,
		GAME_PLAY,
		GAME_FINIFH,
		MAX_STAE,
		NONE,
	}

	void initialize( ) {
		_gameFinish = false;
		_gameStart = false; 
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {

		checkeGameState( );	// 開始＆終了確認

		switch ( getState( ) ) {
			case STATE.GAME_START:
				break;
			case STATE.GAME_FINIFH:
				FadeSceneManeger.LoadScene( FadeSceneManeger.TAG.TITLE );
				initialize( );	// フラグ初期化
				break;
		}


	}

	public STATE getState( ) {
		STATE state = STATE.NONE;

		// プレイ中
		if ( _gameStart ) {
			state = STATE.GAME_START;
		}

		// おわり
		if ( _gameFinish ) {
			state = STATE.GAME_FINIFH;
		}

		return state;
	}

	void checkeGameState( ) {
		// 開始したか確認
		//if ( _rhythmManager.isPlay( ) && !_gameStart ) {
		if ( Music.IsPlaying && !_gameStart ) {
			_gameStart = true;
		}

		if ( !Music.IsPlaying && _gameStart && !_gameFinish ) {
			_gameStart = false;
			_gameFinish = true;
		}
	}
}
