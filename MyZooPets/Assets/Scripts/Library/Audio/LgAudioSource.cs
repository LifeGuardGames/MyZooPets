using UnityEngine;
using System;
using System.Collections;

///////////////////////////////////////////
// LgAudioSource
// Obfuscating playing audio a little bit,
// so that audio can listen to and respond
// to messages.
///////////////////////////////////////////

public class LgAudioSource : MonoBehaviour{
	public class DestroyArgs : EventArgs{
		public string ClipName {get; set;}
	}

	public EventHandler<DestroyArgs> OnDestroyed;

	private AudioSource audioSource;
	private string audioClipName;
	
	public void Init(DataSound sound, Transform tf, Hashtable hashOverrides){		
		string strResource = sound.GetResourceName();
		AudioClip clip = Resources.Load(strResource) as AudioClip;
		
		if(clip == null){
			Debug.LogError("No such sound clip for resource " + strResource);
			Destroy(gameObject);
			return;
		}

		audioClipName = strResource;

		// this is a little messy, but get variables that could be overriden
		float volume = sound.GetVolume();
		if(hashOverrides.Contains("Volume"))
			volume = (float)hashOverrides["Volume"];
		
		float pitch = sound.GetPitch();
		if(hashOverrides.Contains("Pitch"))
			pitch = (float)hashOverrides["Pitch"];
		
		// create the audio source	
		audioSource = gameObject.AddComponent<AudioSource>(); 
		audioSource.clip = clip; 
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		gameObject.transform.parent = tf;
		gameObject.transform.position = tf.position;
		audioSource.Play();
		
		// listen for pausing
//		AudioManager.Instance.OnGamePaused += SetPauseState;

		// listen for stopping
//		AudioManager.Instance.OnStopClipCalled += CheckForStop;

		// add destroy script
		DestroyThis scriptDestroy = gameObject.AddComponent<DestroyThis>();
		scriptDestroy.SetLife(clip.length);		
	}
	
	void OnDestroy(){
		if(OnDestroyed != null){
			DestroyArgs args = new DestroyArgs();
			args.ClipName = audioClipName;
			OnDestroyed(this, args);
		}
			
		// it's possible that the audio manager has been destroyed because the scene is changing, in which case, don't worry
		// about removing listeners, because the audio manager has been destroyed
//		if(!AudioManager.Instance)
//			return;
		
		// stop listening for pausing	
//		AudioManager.Instance.OnGamePaused -= SetPauseState;
//		AudioManager.Instance.OnStopClipCalled -= CheckForStop;
	}
	
	///////////////////////////////////////////
	// FadeOut()
	// Fades out this sound over fTime.
	///////////////////////////////////////////	
	public IEnumerator FadeOut(float fTime){
		for(int i = 9; i >= 0; i--){
			audioSource.volume = i * .1f;
			yield return new WaitForSeconds(fTime / 10);
		}			
		
		//Debug.Log(audioSource.name + " should be completely faded out now");
	}
	
	///////////////////////////////////////////
	// SetPauseState()
	// If the game is pausing or unpausing.
	///////////////////////////////////////////
//	private void SetPauseState(object sender, PauseArgs args){	
//		bool isPaused = args.IsPausing();	
//
//		if(isPaused)
//			audioSource.Pause();
//		else
//			audioSource.Play();
//	}

	public void Stop(){
		audioSource.Stop();
		Destroy(gameObject);
	}
	///////////////////////////////////////////
	// CheckForStop()
	// Listener call, will match parameters and check if this needs to be stopped
	///////////////////////////////////////////
//	public void CheckForStop(object sender, StopArgs args){
//		if(args.ClipName == audioClipName){
//			audioSource.Stop();
//		}
//	}
}
