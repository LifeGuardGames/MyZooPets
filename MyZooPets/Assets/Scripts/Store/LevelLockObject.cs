using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LevelLockObject
// UI that appears over anything that is locked by
// level.
//---------------------------------------------------

public class LevelLockObject : MonoBehaviour {
	// elements of this UI
	public UISprite spriteIcon;
	public UILabel labelLevel;
	
	//---------------------------------------------------
	// CreateLock()
	// Instantiates and inits the UI with the incoming
	// lock data.
	//---------------------------------------------------	
	public static void CreateLock( GameObject goParent, int nLevel ) {
		GameObject goPrefab = Resources.Load( "LevelLockUI" ) as GameObject;
		GameObject lockObject = NGUITools.AddChild( goParent, goPrefab );
		lockObject.GetComponent<LevelLockObject>().Init( nLevel );
	}	
	
	//---------------------------------------------------
	// Init()
	// This function does the work and actually sets the
	// UI labels, sprites, etc for this UI based on the
	// incoming data.
	//---------------------------------------------------	
	public void Init( int nLevel ) {
		// set the proper values on the entry
		labelLevel.text = "" + nLevel;		
	}	
}
