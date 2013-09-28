using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CutsceneFrames : MonoBehaviour {
	
	public string strSceneID;
	
	public int nSize;
	
	public UISprite frame;
	public UIAtlas atlas;
	
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
	}
	
	void Update() {
		timeLeft -= Time.deltaTime;
		
		if ( timeLeft <= 0 ) {
			timeLeft = time;
			nFrame++;
			
			if ( nFrame <= nSize )
				frame.spriteName = strSceneID + "-" + nFrame;
			else 
				Done();
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
}
