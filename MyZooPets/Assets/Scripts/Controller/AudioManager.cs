using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : LgAudioManager<AudioManager>{
	public bool isMusicOn = true;
	public string backgroundMusic;

	private AudioSource backgroundSource;
	
	protected override void Awake(){
		base.Awake();
		backgroundSource = GetComponent<AudioSource>();
	}
	
	protected override void Start(){
		base.Start();
		StartCoroutine(PlayBackground());
	}

	private IEnumerator PlayBackground(){
		yield return new WaitForSeconds(0.5f);
		if(isMusicOn){
			AudioClip backgroundClip = null;
			if(backgroundMusic != null){
				backgroundClip = Resources.Load(backgroundMusic) as AudioClip;
			}

			if(backgroundClip != null){
				backgroundSource.clip = backgroundClip;
				backgroundSource.Play();
			}
		}
	}

	public void LowerBackgroundVolume(float newVolume){
		backgroundSource.volume = newVolume;
	}

	// Pass in null if don't want new music
	public void FadeOutPlayNewBackground(string newAudioClipName, bool isLoop = true){
		StartCoroutine(FadeOutPlayNewBackgroundHelper(newAudioClipName, isLoop));
	}

	private IEnumerator FadeOutPlayNewBackgroundHelper(string newAudioClipName, bool isLoop){
		for(int i = 9; i >= 0; i--){
			backgroundSource.volume = i * .1f;
			yield return new WaitForSeconds(.01f);
		}
		if(newAudioClipName != null){
			backgroundSource.Stop();
			backgroundSource.volume = 0.6f;
			backgroundMusic = newAudioClipName;

			if(!isLoop){
				backgroundSource.loop = false;
			}

			StartCoroutine(PlayBackground());
		}
		else{
			backgroundSource.Stop();
		}
	}

	/// <summary>
	/// Pause all audio sources.
	/// </summary>
	/// <param name="isPaused">If set to <c>true</c> is paused.</param>
	public void PauseBackground(bool isPaused){	
		if(isPaused){
			backgroundSource.Pause();
		}
		else{
			backgroundSource.Play();
		}
	}
}
