using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LgAudioManager<T> : Singleton<T> where T : MonoBehaviour{

	private Dictionary<string, LgAudioSource> spawnedAudioSources; //key: clip name, value: LgAudioSource

	protected virtual void Awake(){
		spawnedAudioSources = new Dictionary<string, LgAudioSource>();
	}

	// implement from child if desire
	protected virtual void Start(){}
	protected virtual void Update(){}
	protected virtual void OnDestroy(){}

	/// <summary>
	/// Plays the clip.
	/// </summary>
	/// <param name="soundClip">Sound clip.</param>
	/// <param name="hashOverrides">Hash overrides.</param>
	public void PlayClip(string clipName, Hashtable hashOverrides = null){
		if(hashOverrides == null)
			hashOverrides = new Hashtable();
		
		if(clipName == ""){
			Debug.LogError("Something trying to play a sound with an empty sound id...");
			return;
		}
		
		DataSound sound = DataSounds.GetSoundData(clipName);
		
		if(sound == null){
			Debug.LogError("No such sound with id " + clipName);
			return;
		}
		
		if(!spawnedAudioSources.ContainsKey(clipName)){
			LgAudioSource audioSource = PlaySound(sound, hashOverrides);
			spawnedAudioSources.Add(clipName, audioSource);
			audioSource.OnDestroyed += LgAudioSourceDestroyed;
		}
	}

	/// <summary>
	/// Stops the clip. clip gets destoryed after stop
	/// </summary>
	/// <param name="clipName">Clip name.</param>
	public void StopClip(string clipName){
		if(spawnedAudioSources.ContainsKey(clipName)){
			LgAudioSource audioSource = spawnedAudioSources[clipName];
			audioSource.Stop();
		}
	}

	private void LgAudioSourceDestroyed(object sender, LgAudioSource.DestroyArgs args){
		if(spawnedAudioSources.ContainsKey(args.ClipName)){
			spawnedAudioSources.Remove(args.ClipName);
		}
	}
	
	/// <summary>
	/// The base level private method that plays
	/// a sound.  It creates a custom audio
	/// source that gives us more control over
	/// the sound.
	/// </summary>
	/// <returns>The sound.</returns>
	/// <param name="sound">Sound.</param>
	/// <param name="hashOverrides">Hash overrides.</param>
	private LgAudioSource PlaySound(DataSound sound, Hashtable hashOverrides){
		GameObject soundObject = new GameObject("Sound: " + sound.GetResourceName()); 
		LgAudioSource soundSource = soundObject.AddComponent<LgAudioSource>();
		soundSource.Init(sound, transform, hashOverrides);
		
		return soundSource;		
	}
}
