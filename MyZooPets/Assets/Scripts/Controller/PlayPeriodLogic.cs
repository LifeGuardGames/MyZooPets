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
			DateTime nextPlayPeriod;
			if(LgDateTime.GetTimeNow().Hour < 12){
				nextPlayPeriod = LgDateTime.Today.AddHours(12);	//next pp at noon
			}
			else{
				nextPlayPeriod = LgDateTime.Today.AddDays(1);	//next pp at midnight
			}
			return nextPlayPeriod;
		}
	}

	/// <summary>
	/// Gets the total time remaining
	/// </summary>
	/// <value>The total time remaining</value>
//	public TimeSpan TotalTimeRemaining{
//		get{
//			return DataManager.Instance.GameData.Calendar.GetTotalTimeRemaining();
//		}
//	}

	/// <summary>
	/// Check if user can play inhaler game.
	/// </summary>
	public bool CanUseEverydayInhaler(){	
//		Debug.Log(DataManager.Instance.GameData.Inhaler.LastestPlayPeriodUsed + " " + GetCurrentPlayPeriod());
		bool retVal = DataManager.Instance.GameData.Inhaler.LastestPlayPeriodUsed < GetCurrentPlayPeriod();

		bool isPart1TutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialPart1Done();
		bool isInhalerTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_INHALER);
		if(!isPart1TutorialDone && isInhalerTutorialDone){
			retVal = false;
		}
		return retVal;
	}

	void Update(){
		// TODO this is called every frame????
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

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			//calculate time diff since last play session ended and submit to game analytics
			TimeSpan timeSinceLastSession = LgDateTime.GetTimeSinceLastPlayed();
			int timeDifference = (int)timeSinceLastSession.TotalHours;

			Analytics.Instance.TimeBetweenPlaySession(timeDifference);
		}
	}

	/// <summary>
	/// Gets the time frame.
	/// Given a time, will return whether or not that time is considered morning or evening.
	/// All parts of the code should use this to determine what time of day it is.
	/// </summary>
	/// <returns>TimeFrames.Evening or Morning</returns>
	/// <param name="time">DateTime</param>
	public static TimeFrames GetTimeFrame(DateTime time){
		if(time.Hour >= 12)
			return TimeFrames.Evening;
		else
			return TimeFrames.Morning;
	}
	
	/// <summary>
	/// Gets the current play period.
	/// </summary>
	public static DateTime GetCurrentPlayPeriod(){
		//if the time now is morning, current play period = 12am
		//if afternoon, current play period = 12pm
		return (LgDateTime.GetTimeNow().Hour < 12) ? LgDateTime.Today : LgDateTime.Today.AddHours(12);;
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
	/// Additional steps to be done after inhaler game is finished
	/// </summary>
	public void InhalerGameDonePostLogic(){
		DateTime localNotificationFireDate;
		bool isInhalerTutDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_INHALER);
		if(isInhalerTutDone){
			//register local notification.
			localNotificationFireDate = NextPlayPeriod.AddHours(7); //set notif to 7am and 7pm
			string petName = DataManager.Instance.GameData.PetInfo.PetName;
			string notifText;
		
			notifText = String.Format(Localization.Localize("NOTIFICATION_1_PRO"), petName);

			LgNotificationServices.RemoveIconBadgeNumber();
			LgNotificationServices.ScheduleLocalNotification(notifText, localNotificationFireDate);
		}

		//update next play period in DM so it gets serialized
//		DataManager.Instance.GameData.Calendar.NextPlayPeriod = nextPlayPeriod;

		TimeSpan totalTimeRemainTillNextPlayPeriod = NextPlayPeriod - LgDateTime.GetTimeNow();
//		DataManager.Instance.GameData.Calendar.SetTotalTimeRemaining(totalTimeRemainTillNextPlayPeriod);
	}
	
	/// <summary>
	/// Calculates the current play period.
	/// Reset NextPlayPeriod to the current play period this is usually only use if user misses play period
	/// so at the start of the game (after degradation logic) we reset that play period to now
	/// </summary>
//	public void CalculateCurrentPlayPeriod(){ // TODO wtf is this doing here
//		DataManager.Instance.GameData.Calendar.NextPlayPeriod = GetCurrentPlayPeriod(); 
//
//		//since there's a miss play period. reactivate sick notification
//		DataManager.Instance.GameData.SickNotification.IsRemindedThisPlayPeriod = false;
//	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "1")){
//			Debug.Log( CalculateTimeLeftTillNextPlayPeriod());
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "2")){
//			Debug.Log(GetCurrentPlayPeriod());
//		}
//		if(GUI.Button(new Rect(300, 100, 100, 100), "3")){
//		}
//		if(GUI.Button(new Rect(400, 100, 100, 100), "4")){
//			Debug.Log(NextPlayPeriod);
//		}
//	}
}
