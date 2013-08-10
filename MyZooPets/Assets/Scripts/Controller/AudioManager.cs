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

	/*
	public void PlayEffect(string audioClipName){
		PlayEffect(audioClipName, 1.0f);
	}
	
	public void PlayEffect(string audioClipName, float volume){
		if(audioClipName == "button1"){
			effectClip = button1;
		}
		else if(audioClipName == "button2"){
			effectClip = button2;
		}
		else{
			Debug.LogError("Could not find AudioClip Name");	
		}
		
		if(effectClip != null){
			if(volume > 1){
				Debug.Log("Audio volume greater than 1");
				volume = 1;
			}
			else if(volume < 0){
				Debug.Log("Audio volume negative");
				volume = 0;
			}
			effectSource.volume = volume;
			backgroundSource.PlayOneShot(effectClip);
		}
		else{
			Debug.LogError("Effect audio clip can not be found");
		}
	}
	*/
}
