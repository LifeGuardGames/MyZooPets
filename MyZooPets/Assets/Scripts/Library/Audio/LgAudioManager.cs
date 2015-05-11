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
	/// <para>Plays the clip. </para>
	/// <para>option: pass in properties (Volume, Loop, etc) for LgAudioSource. </para>
	/// <para>option: isSoundClipManaged (T: LgAudioManager will keep a reference to this sound clip
	/// and will not allow the same sound clip to be played again until the current clip is stopped or
	/// finished, F: same sound can be played more than once and once the sound is played there's no
	/// way to stop it until it finishes)</para>  
	/// </summary>
	/// <param name="clipName">Sound clip name</param>
	/// <param name="variations">Number of sounds for the same clip</param>
	/// <param name="hashOverrides">Hash overrides.</param>
	public virtual void PlayClip(string clipName, int variations = 1, Hashtable option = null){
		if(option == null){
			option = new Hashtable();
		}
		
		if(clipName == ""){
			Debug.LogError("Something trying to play a sound with an empty sound id...");
			return;
		}
		
		DataSound sound = DataSounds.GetSoundData(clipName, variations);
		
		if(sound == null){
			Debug.LogError("No such sound with id " + clipName);
			return;
		}

		bool isSoundClipManaged = false;	// Set the default to false
		if(option.ContainsKey("IsSoundClipManaged")){
			isSoundClipManaged = (bool) option["IsSoundClipManaged"];
		}

		//if multip sound mode is on. just play the clip
		if(!isSoundClipManaged){
			LgAudioSource audioSource = PlaySound(sound, option);
			audioSource.OnDestroyed += LgAudioSourceDestroyed;
		}
		//if multi sound mode is off. check the dicitonary for the same clip
		else{
			if(!spawnedAudioSources.ContainsKey(clipName)){
				LgAudioSource audioSource = PlaySound(sound, option);
				spawnedAudioSources.Add(clipName, audioSource);
				audioSource.OnDestroyed += LgAudioSourceDestroyed;
			}
		}

	}

	/// <summary>
	/// Stops the clip. clip gets destoryed after stop
	/// </summary>
	/// <param name="clipName">Clip name.</param>
	public virtual void StopClip(string clipName){
		if(!string.IsNullOrEmpty(clipName) && spawnedAudioSources.ContainsKey(clipName)){
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
	private LgAudioSource PlaySound(DataSound sound, Hashtable option){
		GameObject soundObject = new GameObject("Sound: " + sound.GetResourceName()); 
		LgAudioSource soundSource = soundObject.AddComponent<LgAudioSource>();
		soundSource.Init(sound, transform, option);
		
		return soundSource;		
	}
}
