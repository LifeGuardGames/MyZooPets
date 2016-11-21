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
	
	public void Init(DataSound sound, Transform tf, Hashtable option){		
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
		if(option.Contains("Volume"))
			volume = (float)option["Volume"];
		
		float pitch = sound.GetPitch();
		if(option.Contains("Pitch")) {
			pitch = (float)option["Pitch"];
		}
		bool loop = false;
		if(option.Contains("Loop"))
			loop = (bool)option["Loop"];
		
		// create the audio source	
		audioSource = gameObject.AddComponent<AudioSource>(); 
		audioSource.clip = clip; 
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		audioSource.loop = loop;
		gameObject.transform.parent = tf;
		gameObject.transform.position = tf.position;
		audioSource.Play();

		// add destroy script
		if(!loop){
			DestroyThis scriptDestroy = gameObject.AddComponent<DestroyThis>();
			scriptDestroy.SetLife(clip.length);		
		}
	}
	
	void OnDestroy(){
		if(OnDestroyed != null){
			DestroyArgs args = new DestroyArgs();
			args.ClipName = audioClipName;
			OnDestroyed(this, args);
		}
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

	public void Stop(){
		audioSource.Stop();
		Destroy(gameObject);
	}

}
