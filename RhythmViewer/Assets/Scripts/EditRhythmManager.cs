using UnityEngine;
using System.Collections;

public class EditRhythmManager : RhythmManager {
	
	public void setFrame( int frame ) {
		_frame = frame;
	}

	public void setIndex( int index ) {
		_index = index;
	}

	public void stop( ) {
		setFrame( 0 );
		setIndex( 0 );
	 }
 }
