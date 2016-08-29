using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationManager : MonoBehaviour {

	#region モジュール クラス
	private class Module< T > {
		private int _id = -1;
		private T _instance;

		public Module( int id, T instance ) {
			_id = id;
			_instance = instance;
		}

		/// <summary>
		/// IDの取得
		/// </summary>
		/// <returns></returns>
		public int getID( ) {
			return _id;
		}

		/// <summary>
		/// IDのセット
		/// </summary>
		/// <param name="id"></param>
		public void setId( int id ) {
			_id = id;
		}
		
		/// <summary>
		/// インスタンス取得
		/// </summary>
		/// <returns></returns>
		public T getInstance( ) {
			return _instance;
		}

	}
	#endregion

	private int _idCount = -1;	// IDのカウント
	private List< Module< Neon > > _neonList = new List< Module< Neon > >( );

	// Update is called once per frame
	void FixedUpdate( ) {
		
	}

	/// <summary>
	/// 登録用IDの取得
	/// </summary>
	/// <returns></returns>
	private int getId( ) {
		return _idCount++;
	}

	public int addNeon( Neon instance ) {
		Module< Neon > data = new Module< Neon >( getId( ), instance );
		_neonList.Add( data );
		return data.getID( );
	}
}
