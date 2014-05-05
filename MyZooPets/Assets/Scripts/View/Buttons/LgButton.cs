using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// LgButton (Lifeguard Button)
// Generic button class that other buttons derive from.
// This class handles high level input restrictions
// and makes sure that the button can process a click.
//---------------------------------------------------

public abstract class LgButton : MonoBehaviour {
	
	//=======================Events========================
	public EventHandler<EventArgs> OnProcessed; 	// when this button is processed
	//=====================================================		
	
	// is this button a sprite (2D)?  if it is, it is clicked a little differently than a 3d object
	public bool bSprite;
	public UIModeTypes eMode = UIModeTypes.NotInited; // the mode that this button will check for to make sure it can be clicked
	public string strSoundProcess; // the sound resource this button plays when it is clicked
	public string buttonName; //the name that will be used for analytics event if not empty

	// ah...this boolean is for buttons that are on a UI that do not care about checking the click manager.
	// however, as soon as we have UIs that open other UIs, we will need to implement a more real system by
	// which buttons have a mode, opening a UI pushes a mode (and closing a UI pops a mode) and this class should
	// actually check the button's mode against the latest mode in the queue.
	public bool bCheckClickManager = true;


	public UIModeTypes GetMode() {
		return eMode;	
	}

	public string GetProcessSound() {
		return strSoundProcess;	
	}
	
	private bool ShouldCheckClickManager() {
		return bCheckClickManager;	
	}

	// if button is clicked when mode is locked, play a negative feedback sound
	// public bool playNotProcessSound = false;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {
		// do a check for a valid mode
		if ( eMode == UIModeTypes.NotInited ) {
			//Debug.LogError("LgButton(" + gameObject.name + ") does not have a proper mode!", gameObject);
			eMode = UIModeTypes.None;	
		}
		
		_Start();
	}
	protected virtual void _Start() {}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------		
	void OnDestroy() {
		_OnDestroy();
	}	
	protected virtual void _OnDestroy() {}
	
	void Awake() {
		_Awake();	
	}

	protected virtual void _Awake() {
	}
	
	//---------------------------------------------------
	// OnClick()
	// 2D sprite buttons will receive this event, which
	// will click the button.  At the moment 3D objects
	// also happen to receive this event, but it's possible
	// they won't in the future, so this is for 2D only.
	//---------------------------------------------------	
	void OnClick() {
		if ( enabled && bSprite )
			ButtonClicked();
	}

	//---------------------------------------------------
	// OnPress()
	// 2D sprite buttons - Play the sound when the button is pressed
	//---------------------------------------------------	
	void OnPress( bool bPressed ) {
		if(bPressed){
			CheckSoundToPlay();
		}
	}
	
	//---------------------------------------------------
	// OnTap()
	// 3D gameObjects will receive this event.
	//---------------------------------------------------
	void OnTap(TapGesture gesture) { 
		ButtonClicked();
	}

	//---------------------------------------------------
	// OnFingerStationary()
	// 3D objects - Play the sound when the object is pressed down
	//---------------------------------------------------	
	void OnFingerStationary( FingerMotionEvent e ) {
		if ( e.Phase == FingerMotionPhase.Started ) {
			CheckSoundToPlay();
		}
	}
	
	//---------------------------------------------------
	// ButtonClicked()
	// When the button is actually clicked.
	//---------------------------------------------------	
	public void ButtonClicked ()
	{
		// if the button needs to check the click manager before proceding, do so and return if necessary
		if (ShouldCheckClickManager() && !ClickManager.Instance.CanRespondToTap(gameObject)){
			return;
		}
		
		// special case hack here...if we are in a tutorial, regardless of if we are supposed to check the click manager, check it
		if(ShouldCheckClickManager() == false && TutorialManager.Instance && 
			!TutorialManager.Instance.CanProcess(gameObject)){
			return;
		}
		
		// let anything listening know that this button has been processed
		if ( OnProcessed != null )
			OnProcessed( this, EventArgs.Empty );

		// process the click
		ProcessClick();
	}

	private void CheckSoundToPlay(){
		if (ShouldCheckClickManager() && !ClickManager.Instance.CanRespondToTap(gameObject)){
			if(bSprite){
				// Play the bad sound
				PlayNotProcessSound();
			}
			return;
		}
		
		if(ShouldCheckClickManager() == false && TutorialManager.Instance && 
			!TutorialManager.Instance.CanProcess(gameObject)){
			if(bSprite){
				// Play the bad sound
				PlayNotProcessSound();
			}
			return;
		}
		
		// Play the good sound
		PlayProcessSound();
	}

	private void PlayProcessSound() {
		string strSound = GetProcessSound();
		
		if ( !string.IsNullOrEmpty(strSound) )
			AudioManager.Instance.PlayClip( strSound );	
	}

	private void PlayNotProcessSound(){
		string sound = "buttonDontClick";

		if(!string.IsNullOrEmpty(sound))
			AudioManager.Instance.PlayClip(sound);
	}
	
	//---------------------------------------------------
	// ProcessClick()
	// Children should implement this.  This function will
	// only be called if the button is allowed to process
	// the click (i.e., UI is not locked, etc).
	//---------------------------------------------------	
	protected virtual void ProcessClick() {
		Analytics.Instance.LgButtonClicked(buttonName);
		
		Debug.LogError("Children should implement ProcessClick!");	
	}
}
