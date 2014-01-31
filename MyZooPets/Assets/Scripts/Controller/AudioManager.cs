using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// event arguments for game pausing
public class PauseArgs : EventArgs{
	private bool bIsPausing;
	public bool IsPausing() {
		return bIsPausing;	
	}

	public PauseArgs( bool bIsPausing){
		this.bIsPausing = bIsPausing;
	}
}

public class AudioManager : Singleton<AudioManager>{
	//=======================Events========================
	public EventHandler<PauseArgs> OnGamePaused; 		// when the game is paused (NOT application paused)
	//=====================================================		

	/** Types of Audio Clips
	*	background	:	only one playing at a time, if any (ie. music)
	*	effect		:	overlapping allowed to all (ie. jump, coin collect)
	**/
	
	public AudioClip background1;
	public string strBgMusic;

	private AudioClip backgroundClip;
	private AudioClip effectClip;
	private AudioSource backgroundSource;

	public bool isMusicOn = true; // Thank me(Sean) later, devs
	

	void Awake(){
		// Spawns components itself
		backgroundSource = gameObject.AddComponent("AudioSource") as AudioSource;
		
		// load sound xml data
		DataSounds.SetupData();
	}
	
	void Start() {
		PlayBackground();	
	}

	public void PlayBackground(){
		if(isMusicOn){
			if ( background1 )
				backgroundClip = background1;
			else if ( strBgMusic != null )
				backgroundClip = Resources.Load( strBgMusic ) as AudioClip;

			D.Assert(backgroundClip != null, "Null audioclip");
			backgroundSource.volume = 0.3f;
			backgroundSource.loop = true;
			backgroundSource.clip = backgroundClip;
			backgroundSource.Play();
		}
	}

	public void LowerBackgroundVolume(float newVolume){
		backgroundSource.volume = newVolume;
	}

	// Pass in null if don't want new music
	public void FadeOutPlayNewBg(string newAudioClipName){
		StartCoroutine(FadeOutPlayNewBgHelper(newAudioClipName));
	}

	private IEnumerator FadeOutPlayNewBgHelper(string newAudioClipName){
		for (int i = 9; i > 0; i--){
	        backgroundSource.volume = i * .1f;
	        yield return new WaitForSeconds(.4f);
	    }
		if(newAudioClipName != null){
			PlayBackground();
		}
		else{
			backgroundSource.Stop();
		}
	}
	
	///////////////////////////////////////////
	// Pause()
	// If the game ever gets paused, all the
	// audio sources also need to pause.
	///////////////////////////////////////////		
	public void Pause( bool bPausing ) {
		if ( OnGamePaused != null )
			OnGamePaused( this, new PauseArgs(bPausing) );		

		if(bPausing)
			backgroundSource.Pause();
		else
			backgroundSource.Play();
	}
	
	///////////////////////////////////////////
	// PlayClip()
	// Plays a sound with the name strClip
	// from resources.
	// hashOverrides Params:
	//	Volume(float)
	//	Pitch(float)
	///////////////////////////////////////////	
	public LgAudioSource PlayClip( string strClip, Hashtable hashOverrides = null ) {
		if ( hashOverrides == null )
			hashOverrides = new Hashtable();
		
		if ( strClip == "" ) {
			Debug.LogError("Something trying to play a sound with an empty sound id...");
			return null;
		}
		
		DataSound sound = DataSounds.GetSoundData( strClip );
		
		if ( sound == null ) {
			Debug.LogError("No such sound with id " + strClip );
			return null;
		}
			
		return PlaySound( sound, hashOverrides );	
	}
	
	///////////////////////////////////////////
	// PlaySound()
	// The base level private method that plays
	// a sound.  It creates a custom audio
	// source that gives us more control over
	// the sound.
	///////////////////////////////////////////	
	private LgAudioSource PlaySound( DataSound sound, Hashtable hashOverrides ) {
		GameObject soundObject = new GameObject("Sound: " + sound.GetResourceName()); 
		LgAudioSource soundSource = soundObject.AddComponent<LgAudioSource>();
		soundSource.Init( sound, transform, hashOverrides );
		
		return soundSource;		
	}
	
//	public void PlayEffect(string audioClipName){
//		PlayEffect(audioClipName, 1.0f);
//	}
//	
//	public void PlayEffect(string audioClipName, float volume){
//		if(audioClipName == "button1"){
//			effectClip = button1;
//		}
//		else if(audioClipName == "button2"){
//			effectClip = button2;
//		}
//		else{
//			Debug.LogError("Could not find AudioClip Name");	
//		}
//		
//		if(effectClip != null){
//			if(volume > 1){
//				Debug.Log("Audio volume greater than 1");
//				volume = 1;
//			}
//			else if(volume < 0){
//				Debug.Log("Audio volume negative");
//				volume = 0;
//			}
//			effectSource.volume = volume;
//			backgroundSource.PlayOneShot(effectClip);
//		}
//		else{
//			Debug.LogError("Effect audio clip can not be found");
//		}
//	}
}
