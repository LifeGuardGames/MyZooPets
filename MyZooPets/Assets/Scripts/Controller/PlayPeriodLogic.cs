using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayPeriodEventArgs : EventArgs{
	public TimeSpan TimeLeft { get; set; }	
}

public class PlayPeriodLogic : Singleton<PlayPeriodLogic>{
	public static EventHandler<PlayPeriodEventArgs> OnUpdateTimeLeftTillNextPlayPeriod;
	public static EventHandler<EventArgs> OnNextPlayPeriod;
	public const float PLAYPERIOD_LENGTH = 12f;
	private bool isCountingDown = false;

	//Return the next time the user can collect bonuses
	public DateTime NextPlayPeriod{
		get{ 
			return DataManager.Instance.GameData.Calendar.NextPlayPeriod;
		}
	}

	/// <summary>
	/// Gets the total time remain.
	/// </summary>
	/// <value>The total time remain.</value>
	public TimeSpan TotalTimeRemain{
		get{
			return DataManager.Instance.GameData.Calendar.GetTotalTimeRemain();
		}
	}

	//Check if the user can play the inhaler game
	public bool CanUseEverydayInhaler(){

		DateTime now = LgDateTime.GetTimeNow();
		bool retVal = now >= NextPlayPeriod;


//		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FLAME);

		bool isPart1TutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialPart1Done();
		// special case: if we are done with the inhaler tutorial but not all tutorials, just return false
//		bool areTutorialsFinished = DataManager.Instance.GameData.Tutorial.AreTutorialsFinished();
		bool isInhalerTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_INHALER);
//		if(!areTutorialsFinished && isInhalerTutorialDone)
//		if(!isFlameTutorialDone)
		if(!isPart1TutorialDone && isInhalerTutorialDone)
			retVal = false;
			
		return retVal;

	}

	void Update(){

		if(CanUseEverydayInhaler()){
			// okay, so the player can use their inhaler...but were we previously counting down?
			if(isCountingDown){
				// if we were, stop
				isCountingDown = false;
				
				//fire event
				if(OnNextPlayPeriod != null)
					OnNextPlayPeriod(this, EventArgs.Empty);
			}
			return;
		}

		// if we make it here, we are counting down
		isCountingDown = true;

		TimeSpan timeTillNextPlayPeriod = NextPlayPeriod - LgDateTime.GetTimeNow();

		//fire counting down event
		if(OnUpdateTimeLeftTillNextPlayPeriod != null){
			PlayPeriodEventArgs args = new PlayPeriodEventArgs();
			args.TimeLeft = timeTillNextPlayPeriod;
			OnUpdateTimeLeftTillNextPlayPeriod(this, args);
		}
	}

	/// <summary>
	/// Gets the time frame.
	/// Given a time, will return whether or not that
	/// time is considered morning or evening.  All
	/// parts of the code should use this to determine
	/// what time of day it is.
	/// </summary>
	/// <returns>The time frame.</returns>
	/// <param name="time">Time.</param>
	public static TimeFrames GetTimeFrame(DateTime time){
		if(time.Hour >= 12)
			return TimeFrames.Evening;
		else
			return TimeFrames.Morning;
	}
	
	/// <summary>
	/// Gets the current play period.
	/// </summary>
	/// <returns>The current play period.</returns>
	public static DateTime GetCurrentPlayPeriod(){
		DateTime currentPlayPeriod;

		//if the time now is in the morning the current play period is 12 am
		if(LgDateTime.GetTimeNow().Hour < 12)
			currentPlayPeriod = LgDateTime.Today;
        //if the time now is in the afternoon the current play period is 12pm
        else
			currentPlayPeriod = LgDateTime.Today.AddHours(12);

		return currentPlayPeriod;
	}

	/// <summary>
	/// Calculates the time left till next play period.
	/// </summary>
	/// <returns>The time left till next play period.</returns>
	public TimeSpan CalculateTimeLeftTillNextPlayPeriod(){

		DateTime next = NextPlayPeriod;
		DateTime now = LgDateTime.GetTimeNow();
		TimeSpan timeTillNextPlayPeriod = next - now;

		//Remove negate TimeSpan
		if(timeTillNextPlayPeriod < TimeSpan.Zero)
			timeTillNextPlayPeriod = TimeSpan.Zero;

		return timeTillNextPlayPeriod;
	}
	
	/// <summary>
	/// Calculates the next play period.
	/// </summary>
	public void CalculateNextPlayPeriod(){
		DateTime nextPlayPeriod;
		DateTime localNotificationFireDate;

		if(LgDateTime.GetTimeNow().Hour < 12){ 
			//next reward time at noon
			nextPlayPeriod = LgDateTime.Today.AddHours(12);
		}
		else{ 
			//next reward time at midnight
			nextPlayPeriod = LgDateTime.Today.AddDays(1);
		}

		bool isInhalerTutDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_INHALER);
		if(isInhalerTutDone){
			//register local notification.
			localNotificationFireDate = nextPlayPeriod.AddHours(7); //set notif to 7am and 7pm
			string petName = DataManager.Instance.GameData.PetInfo.PetName;
			string notifText;

			if(VersionManager.IsLite()){
				string petPossession = StringUtils.FormatStringPossession(petName);
				notifText = String.Format(Localization.Localize("NOTIFICATION_1_LITE"), petPossession, petName);
			}
			else{
				notifText = String.Format(Localization.Localize("NOTIFICATION_1_PRO"), petName);
			}

			LgNotificationServices.RemoveIconBadgeNumber();
			LgNotificationServices.ScheduleLocalNotification(notifText, localNotificationFireDate);
		}

		//update next playperiod in DM so it gets serialized
		DataManager.Instance.GameData.Calendar.NextPlayPeriod = nextPlayPeriod;

		TimeSpan totalTimeRemainTillNextPlayPeriod = nextPlayPeriod - LgDateTime.GetTimeNow();
		DataManager.Instance.GameData.Calendar.SetTotalTimeRemain(totalTimeRemainTillNextPlayPeriod);
	}
	
	/// <summary>
	/// Calculates the current play period.
	/// Reset NextPlayPeriod to the current play period
	/// this is usually only use if user misses play period
	/// so at the start of the game (after degradation logic)
	/// we reset that play period to now
	/// </summary>
	public void CalculateCurrentPlayPeriod(){
		DataManager.Instance.GameData.Calendar.NextPlayPeriod = GetCurrentPlayPeriod(); 
	}

}
