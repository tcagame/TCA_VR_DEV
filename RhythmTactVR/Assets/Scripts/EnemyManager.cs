using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class EnemyManager : MonoBehaviour {
	
	[ SerializeField ]
	private FileManager _file_manager;
	[ SerializeField ]
	private RhythmManager _rhythm_manager;

	public List<GameObject> _enemy = new List<GameObject>( );
	public Transform _target;

	GameObject _enemy_prefab;
	int _create_count = 0;   // 打ち出すエネミーの番号
	int _comming_rhyrhm_num = 0; // 仮のリズム番号

	void Awake( ) {
		_enemy_prefab = ( GameObject )Resources.Load( "Prefabs/Enemy" );
	}

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		// リズムマネジャーに聞く処理
		/*
		 */
		if ( Input.GetMouseButtonDown( 0 ) || _rhythm_manager.isTiming( ) == true ) {
			// Smallキューブの移動
			Move( );
			// 遅延キューブを発射させる
			for ( int i = 0; i < getEnemyNum( ); i++ ) {
				if ( _enemy[ i ].GetComponent<Enemy>( ).getObjType( ) == Enemy.OBJECT_TYPE.SLOW_MIDDLE &&
					 _enemy[ i ].GetComponent<Enemy>( ).isStart( ) == false ) {
					_enemy[ i ].GetComponent<Rigidbody>( ).AddForce( _enemy[ i ].GetComponent<Enemy>( ).getDir( ) * _enemy[ i ].GetComponent<Enemy>( ).getSpeed( ) );
					_enemy[ i ].GetComponent<Enemy>( ).started( );
				}
			}
			// キューブの生成
			if ( _create_count < _file_manager.getRhythmCount( ) ) {
				if ( ( _comming_rhyrhm_num == _file_manager.getRhythmForNum( _create_count ).rhythm_num ) ||
					 ( _rhythm_manager.getIndex( ) == _file_manager.getRhythmForNum( _create_count ).rhythm_num ) ) {
					Create( _create_count );
					_create_count++;
				}
			}
			
			_comming_rhyrhm_num++;
		}
	}

	void Move( ) {
		int num = getEnemyNum( );
		
		if ( num > 0 ) {
			for ( int i = 0; i < num; i++ ) {
				if ( _enemy[ i ].GetComponent<Enemy>( ).getObjType( ) == Enemy.OBJECT_TYPE.FAST_SMALL ||
					 _enemy[ i ].GetComponent<Enemy>( ).getObjType( ) == Enemy.OBJECT_TYPE.SLOW_SMALL ) {
					SetEnemyPos( i );
				}
			}
		}
	}
	
	public void Create( int count ) {
		// 値の設定
		GameObject obj = ( GameObject )Instantiate( _enemy_prefab, _file_manager.getRhythmForNum( count ).create_pos, Quaternion.identity );
		obj.GetComponent<Enemy>( ).setObjType( _file_manager.getRhythmForNum( count ).obj_type );
		obj.GetComponent<Enemy>( ).setSpeed( _file_manager.getRhythmForNum( count ).speed );
		obj.GetComponent<Enemy>( ).setTargetType( _file_manager.getRhythmForNum( count ).target_type );
			
		// ターゲットの設定
		Vector3 pos = _target.position;
		if ( obj.GetComponent<Enemy>( ).getTargetType( ) == Enemy.TARGET_TYPE.RIGHT_ARM ) {
			pos.x += 2.0f;
		} else if ( obj.GetComponent<Enemy>( ).getTargetType( ) == Enemy.TARGET_TYPE.LEFT_ARM ) {
			pos.x -= 2.0f;
		}
		// 打ち出す方向の設定
		Vector3 vec = pos - obj.transform.position;
		obj.GetComponent<Enemy>( ).setDir( vec );
		if ( obj.GetComponent<Enemy>( ).getObjType( ) == Enemy.OBJECT_TYPE.FAST_MIDDLE ) {
			obj.GetComponent<Rigidbody>( ).AddForce( obj.GetComponent<Enemy>( ).getDir( ) * obj.GetComponent<Enemy>( ).getSpeed( ) );
			obj.GetComponent<Enemy>( ).started( );
		}
		_enemy.Add( obj );
	}

	public int getEnemyNum( ) {
		return _enemy.Count;
	}

	public void SetEnemyPos( int num ) {
		Vector3 pos = _enemy[ num ].transform.position;
		FILE_DATA.ENEMY_GENERATOR.ENEMY_DATA rhythm = _file_manager.getRhythmForNum( num );
		
		_enemy[ num ].transform.position = new Vector3( pos.x + _enemy[ num ].GetComponent<Enemy>( ).getDir( ).x * rhythm.speed,
														pos.y + _enemy[ num ].GetComponent<Enemy>( ).getDir( ).y * rhythm.speed,
														pos.z + _enemy[ num ].GetComponent<Enemy>( ).getDir( ).z * rhythm.speed );
	}
}
