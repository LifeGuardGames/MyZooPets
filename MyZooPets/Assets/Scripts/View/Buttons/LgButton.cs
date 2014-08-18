using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Lg button (Lifeguard Button)
/// Generic button class that other buttons derive from.
/// This class handles high level input restrictions
/// and makes sure that the button can process a click.
/// </summary>
public class LgButton : MonoBehaviour{
	
	//=======================Events========================
	public EventHandler<EventArgs> OnProcessed; 	// when this button is processed
	//=====================================================		
	
	// is this button a sprite (2D)?  if it is, it is clicked a little differently than a 3d object
//	public bool bSprite;
//	public UIModeTypes eMode = UIModeTypes.NotInited; // the mode that this button will check for to make sure it can be clicked
//
//
//	public string strSoundProcess; // the sound resource this button plays when it is clicked

//	public bool bCheckClickManager = true;

	public List<UIModeTypes> modeTypes = new List<UIModeTypes>();
	public string buttonName; //the name that will be used for analytics event if not empty
	public string buttonSound;
	public bool isCheckingClickManager;
	public bool isSprite;
	
	public List<UIModeTypes> GetModes(){
		return modeTypes;
	}

	public string GetProcessSound(){
		return buttonSound;	
	}
	
	private bool ShouldCheckClickManager(){
		return isCheckingClickManager;	
	}
	
	void Start(){
		for(int i=0; i<modeTypes.Count; i++){
			if(modeTypes[i] == UIModeTypes.NotInited)
				modeTypes[i] = UIModeTypes.None;
		}

		
		_Start();
	}
	
	void OnDestroy(){
		_OnDestroy();
	}
	
	void Awake(){
		_Awake();	
	}

	/// <summary>
	/// Raises the click event.
	/// 2D sprite buttons will receive this event, which
	/// will click the button.  At the moment 3D objects
	/// also happen to receive this event, but it's possible
	/// they won't in the future, so this is for 2D only.
	/// </summary>
	void OnClick(){
		if(enabled && isSprite)
			ButtonClicked();
	}

	/// <summary>
	/// Raises the press event.
	/// 2D sprite buttons - Play the sound when the button is pressed
	/// </summary>
	/// <param name="bPressed">If set to <c>true</c> is pressed.</param>
	void OnPress(bool isPressed){
		if(isPressed){
			CheckSoundToPlay();
		}
	}

	/// <summary>
	/// Raises the tap event. 
	/// 3D gameObjects will receive this event.
	/// </summary>
	/// <param name="gesture">Gesture.</param>
	void OnTap(TapGesture gesture){ 
		ButtonClicked();
		CheckSoundToPlay();
	}

	/// <summary>
	/// Raises the finger stationary event.
	/// 3D objects - Play the sound when the object is pressed down
	/// </summary>
	/// <param name="e">E.</param>
	void OnFingerStationary(FingerMotionEvent e){
		if(e.Phase == FingerMotionPhase.Started){
			CheckSoundToPlay();
		}
	}
	
	/// <summary>
	/// When button is actually clicked.
	/// </summary>
	public void ButtonClicked(){
		// if the button needs to check the click manager before proceding, do so and return if necessary
		if(ShouldCheckClickManager() && !ClickManager.Instance.CanRespondToTap(gameObject)){
			return;
		}
		
		// special case hack here...if we are in a tutorial, regardless of if we are supposed to check the click manager, check it
		if(ShouldCheckClickManager() == false && TutorialManager.Instance && 
			!TutorialManager.Instance.CanProcess(gameObject)){
			return;
		}
		
		// let anything listening know that this button has been processed
		if(OnProcessed != null)
			OnProcessed(this, EventArgs.Empty);

		// process the click
		ProcessClick();
	}

	protected virtual void _Start(){}
	protected virtual void _OnDestroy(){}
	protected virtual void _Awake(){}

	/// <summary>
	/// Processes the click.
	/// Children should implement this. This function will only be called if the
	/// button is allowed to process the click (i.e., UI is not locked, etc).
	/// </summary>
	protected virtual void ProcessClick(){
	}

	protected void PlayNotProcessSound(){
		string sound = "buttonDontClick";
		
		if(!string.IsNullOrEmpty(sound)){
			Hashtable option = new Hashtable();
			option.Add("IsSoundClipManaged", false);
			AudioManager.Instance.PlayClip(sound, option);
		}
			
	}

	/// <summary>
	/// Checks the sound to play.
	/// Play click sound or negative sound
	/// </summary>
	private void CheckSoundToPlay(){
		if(ShouldCheckClickManager() && !ClickManager.Instance.CanRespondToTap(gameObject)){
			if(isSprite){
				// Play the bad sound
				PlayNotProcessSound();
			}
			return;
		}
		
		if(ShouldCheckClickManager() == false && TutorialManager.Instance && 
			!TutorialManager.Instance.CanProcess(gameObject)){
			if(isSprite){
				// Play the bad sound
				PlayNotProcessSound();
			}
			return;
		}
		
		// Play the good sound
		PlayProcessSound();
	}

	private void PlayProcessSound(){
		string strSound = GetProcessSound();
		
		if(!string.IsNullOrEmpty(strSound)){
			Hashtable option = new Hashtable();
			option.Add("IsSoundClipManaged", false);
			AudioManager.Instance.PlayClip(strSound, option);
		}
	}


}
