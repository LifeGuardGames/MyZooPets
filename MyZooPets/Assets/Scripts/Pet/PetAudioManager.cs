using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetAudioManager : LgAudioManager<PetAudioManager> {

	private string lastPlayedClip;
	private string loopingClipName;

	private string recurringClipName; //the name of the clip that is being played at an interval
	private bool isPlayingRecurringClip = false;
	private float recurringTimer = 5f;
	private float maxTimeBetweenRecurring = 5f;


	protected override void Update(){
		base.Update();
		
		if(isPlayingRecurringClip){
			recurringTimer += Time.deltaTime;
			
			if(recurringTimer >= maxTimeBetweenRecurring){
				recurringTimer = 0;
				
				//play clip here
				PlayClip(recurringClipName);
			}
		}
	}

	/// <summary>
	/// Play the sound using animationID. animationID needs to be converted to clipName first
	/// </summary>
	/// <returns>The animation sound.</returns>
	/// <param name="animationID">Animation I.</param>
	public string PlayAnimationSound(string animationID){
		ImmutableDataPetAnimationSound animationSound = DataLoaderPetAnimationSounds.GetData(animationID);
		string clipName = animationSound.GetRandomClipName();

		PlayClip(clipName);

		return clipName;
	}

	public void StopAnimationSound(){
		if(!string.IsNullOrEmpty(lastPlayedClip))
			StopClip(lastPlayedClip);
	}

	public override void PlayClip(string clipName, Hashtable hashOverrides = null){
		StopClip(lastPlayedClip);
		lastPlayedClip = clipName;

		base.PlayClip(clipName, hashOverrides);
	}

	public override void StopClip(string clipName){
		lastPlayedClip = "";
		base.StopClip(clipName);
	}

	/// <summary>
	/// Plays recurring clip. This means the clip will be played every 
	/// maxTimeBetweenRecurring.
	/// </summary>
	/// <param name="clipName">Clip name.</param>
	/// <param name="timer">Timer.</param>
	public void PlayRecurringClip(string animationID){
		if(!isPlayingRecurringClip){
			ImmutableDataPetAnimationSound animationSound = DataLoaderPetAnimationSounds.GetData(animationID);
			string clipName = animationSound.GetRandomClipName();

			recurringClipName = clipName;
			isPlayingRecurringClip = true;
		}
	}
	
	/// <summary>
	/// Stops the recurring clip. recurring clip doesn't stop unless this function
	/// is called.
	/// </summary>
	public void StopRecurringClip(){
		StopClip(recurringClipName);
		recurringClipName = "";
		isPlayingRecurringClip = false;
		recurringTimer = 0;
	}

	/// <summary>
	/// This will make the audio clip keep looping until stop
	/// </summary>
	/// <param name="clipName">Clip name.</param>
	public void PlayLoopingClip(string animationID){
		
		//only play looping clip again if there is no looping clip right now
		if(string.IsNullOrEmpty(loopingClipName)){
			Hashtable option = new Hashtable();
			option.Add("Loop", true);

			ImmutableDataPetAnimationSound animationSound = DataLoaderPetAnimationSounds.GetData(animationID);
			string clipName = animationSound.GetRandomClipName();

			loopingClipName = clipName;
			PlayClip(clipName, option);
		}
	}
	
	/// <summary>
	/// Stops the current looping clip.
	/// </summary>
	public void StopLoopingClip(){
		StopClip(loopingClipName);
		loopingClipName = "";
	}
}
