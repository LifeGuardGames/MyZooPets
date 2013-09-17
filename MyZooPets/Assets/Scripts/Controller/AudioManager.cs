using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager>{

	/** Types of Audio Clips
	*	background	:	only one playing at a time, if any (ie. music)
	*	effect		:	overlapping allowed to all (ie. jump, coin collect)
	**/
	
	public AudioClip background1;

	private AudioClip backgroundClip;
	private AudioClip effectClip;

	private AudioSource backgroundSource;
	private AudioSource effectSource;

	public bool isMusicOn = true; // Thank me(Sean) later, devs

	void Awake(){
		// Spawns components itself
		backgroundSource = gameObject.AddComponent("AudioSource") as AudioSource;
		effectSource = gameObject.AddComponent("AudioSource") as AudioSource;
	}

	public void PlayBackground(string audioClipName){
		if(isMusicOn){
			if(D.Assert(audioClipName == "background1", "Could not find AudioClip Name")){
				backgroundClip = background1;
	
				D.Assert(backgroundClip != null, "Null audioclip");
				backgroundSource.volume = 1;
				backgroundSource.loop = true;
				backgroundSource.clip = backgroundClip;
				backgroundSource.Play();
			}
		}
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
			PlayBackground(newAudioClipName);
		}
		else{
			backgroundSource.Stop();
		}
	}
	
	
	///////////////////////////////////////////
	// PlayClip()
	// Plays a sound with the name strClip
	// from resources.
	///////////////////////////////////////////	
	public LgAudioSource PlayClip( string strClip, Preferences eType, float fVolume ) {
		if ( strClip == "" ) {
			Debug.Log("Something trying to play a sound with an empty sound id...");
			return null;
		}
			
		AudioClip clip = Resources.Load( strClip ) as AudioClip;
			
		return PlayClip( clip, eType, fVolume );	
	}
	public LgAudioSource PlayClip( string strClip, Preferences eType ) {
		return PlayClip( strClip, eType, 1.0f );	
	}	
	
	
	///////////////////////////////////////////
	// PlayClip()
	// Plays the incoming audio clip.
	///////////////////////////////////////////	
	public LgAudioSource PlayClip( AudioClip clip, Preferences eType, float fVolume )  {	
		// TO DO check some kind of save or preference manager to see if the sound should be played at all (i.e. sound turned off)
		
		return PlaySound( clip, fVolume );
	}	
	public LgAudioSource PlayClip( AudioClip clip, Preferences eType )  {	
		return PlaySound( clip, 1.0f );
	}		
	
	
	///////////////////////////////////////////
	// PlaySound()
	// The base level private method that plays
	// a sound.  It creates a custom audio
	// source that gives us more control over
	// the sound.
	///////////////////////////////////////////	
	private LgAudioSource PlaySound( AudioClip sound, float fVolume  ) {
		if ( sound == null ) {
			Debug.Log("Trying to play a null audio clip");
			return null;
		}
		
		GameObject soundObject = new GameObject("Sound: " + sound.name); 
		LgAudioSource soundSource = soundObject.AddComponent<LgAudioSource>();
		soundSource.Init( sound, transform, fVolume );
		
		return soundSource;
	}	
}
