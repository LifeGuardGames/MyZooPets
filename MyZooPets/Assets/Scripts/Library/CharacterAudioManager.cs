using UnityEngine;
using System.Collections;

public class CharacterAudioManager : Singleton<CharacterAudioManager> {

	private LgAudioSource lastPetSound;
	private LgAudioSource lastMiniPetSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Stop all audio sources
	/// </summary>
	/// <param name="clipName">Clip name.</param>
	public void Stop(string clipName){
//		if(OnStopClipCalled != null){
//			OnStopClipCalled(this, new StopArgs(clipName));
//		}
	}
	
	/// <summary>
	/// Plays the clip.
	/// </summary>
	/// <param name="soundClip">Sound clip.</param>
	/// <param name="hashOverrides">Hash overrides.</param>
	public void PlayClip(string soundClip, Hashtable hashOverrides = null){
		if(hashOverrides == null)
			hashOverrides = new Hashtable();
		
		if(soundClip == ""){
			Debug.LogError("Something trying to play a sound with an empty sound id...");
			return;
		}
		
		DataSound sound = DataSounds.GetSoundData(soundClip);
		
		if(sound == null){
			Debug.LogError("No such sound with id " + soundClip);
			return;
		}
		
		PlaySound(sound, hashOverrides);	
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
