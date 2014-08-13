using UnityEngine;
using System.Collections;

public class MiniPetAudioManager : LgAudioManager<MiniPetAudioManager> {

	private string lastPlayedClip; //keep track of the last clip being played
	private string loopingClipName; //the name of the clip that's looping
	private string recurringClipName; //the name of the clip that is being played at an interval
	private bool isPlayingRecurringClip = false;
	private float recurringTimer = 0;
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

	public override void PlayClip(string clipName, Hashtable option = null){
		bool isInterruptingRecurringClip = false;
		if(option != null && option.ContainsKey("IsInterruptingRecurringClip"))
			isInterruptingRecurringClip = (bool) option["IsInterruptingRecurringClip"];

		if(isInterruptingRecurringClip)
			StopRecurringClip();
		else
			StopClip(lastPlayedClip);

		lastPlayedClip = clipName;

		base.PlayClip(clipName, option);
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
	public void PlayRecurringClip(string clipName){
		if(!isPlayingRecurringClip){
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
	public void PlayLoopingClip(string clipName){

		//only play looping clip again if there is no looping clip right now
		if(string.IsNullOrEmpty(loopingClipName)){
			Hashtable option = new Hashtable();
			option.Add("Loop", true);
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
