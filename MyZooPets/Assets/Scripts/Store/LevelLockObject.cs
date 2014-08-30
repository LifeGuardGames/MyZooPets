using UnityEngine;
using System.Collections;
using System;

//---------------------------------------------------
// LevelLockObject
// UI that appears over anything that is locked by
// level.
//---------------------------------------------------

public class LevelLockObject : MonoBehaviour {
	// elements of this UI
	public UISprite spriteIcon;
	public UILabel labelLevel;
	
	// level of this lock
	private int nLevel;
	
	//---------------------------------------------------
	// CreateLock()
	// Instantiates and inits the UI with the incoming
	// lock data.
	//---------------------------------------------------	
	public static void CreateLock(GameObject goParent, int nLevel, bool bBreaks = false, string strPrefab = "LevelLockUI"){
		GameObject goPrefab = Resources.Load(strPrefab) as GameObject;
		GameObject lockObject = NGUITools.AddChild(goParent, goPrefab);

		// UI change if lite version

		lockObject.GetComponent<LevelLockObject>().Init(nLevel, bBreaks);
	}
	
	//---------------------------------------------------
	// Init()
	// This function does the work and actually sets the
	// UI labels, sprites, etc for this UI based on the
	// incoming data.
	//---------------------------------------------------	
	public void Init(int nLevel, bool bBreaks){
		// if this lock breaks, it needs to listen for level up messages
		labelLevel.text = "" + nLevel;		

		// set the proper values on the entry
		this.nLevel = nLevel;
		HUDAnimator.OnLevelUp += LevelUp;		
	}

	//---------------------------------------------------
	// InitLiteVersion()
	// Alternative to the real version init
	// Dont set any listeners or anything
	//---------------------------------------------------
	public void InitLiteVersion(){
		labelLevel.text = "Pro";
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------	
	void OnDestroy(){
		// stop listening for callbacks
		HUDAnimator.OnLevelUp -= LevelUp;	
	}
	
	//---------------------------------------------------
	// LevelUp()
	// If this lock breaks, it listens to level up
	// messages and destroys itself if appropriate.
	//---------------------------------------------------		
	private void LevelUp(object senders, EventArgs args){
        int nNewLevel = (int) LevelLogic.Instance.CurrentLevel; 
		if (nNewLevel >= nLevel){
			Destroy(gameObject);
		}
	}
}
