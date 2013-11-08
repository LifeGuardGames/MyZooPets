using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CutsceneFrames
// Very simple cutscene that shows individual image
// frames.  It either flips through the frames auto-
// matically, or the user presses a button to go
// forwards and back.
//---------------------------------------------------

public class CutsceneFrames : MonoBehaviour {
	
	public string strSceneID;
	
	public int nSize;
	
	public UISprite frame;
	public UIAtlas atlas;
	
	// back/previous buttons
	public GameObject goPrevious;
	public GameObject goNext;
	
	public float time;
	private float timeLeft;
	private int nFrame = 1;
	
	//=======================Events========================
	public static EventHandler<EventArgs> OnCutsceneDone;
	//=====================================================	
	
	// Use this for initialization
	void Start () {
		ClickManager.Instance.ClickLock();
		
		frame.atlas = atlas;
		frame.spriteName = strSceneID + "-1";
		
		timeLeft = time;
		
		// some cutscenes don't have next/previous buttons
		if ( goPrevious )
			goPrevious.SetActive( false );
	}
	
	void Update() {
		// if the time was not set, it means this cutscene must be flipped through manually
		if ( time <= 0 )
			return;
		
		timeLeft -= Time.deltaTime;
		
		if ( timeLeft <= 0 ) {
			timeLeft = time;
			Next();
		}
	}
	
	private void Done() {
		ClickManager.Instance.ReleaseClickLock();
		
		if( OnCutsceneDone != null )
    		OnCutsceneDone(this, EventArgs.Empty);				
		
		Destroy( gameObject );			
	}
	
	private void Skip() {
		Done();	
	}
	
	private void UpdateFrame() {
		frame.spriteName = strSceneID + "-" + nFrame;	
	}
	
	private void Next() {
		nFrame++;
		
		if ( nFrame >= 0 && nFrame <= nSize )
			UpdateFrame();
		else 
			Done();		
		
		if ( nFrame > 0 && goPrevious )
			goPrevious.SetActive( true );
	}
	
	private void Previous() {
		nFrame--;
		
		if ( nFrame >= 0 && nFrame <= nSize )
			UpdateFrame();	
		
		if ( nFrame == 0 )
			goPrevious.SetActive( false );
	}
}
