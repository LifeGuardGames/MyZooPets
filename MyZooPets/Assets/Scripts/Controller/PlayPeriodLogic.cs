using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayPeriodLogic : Singleton<PlayPeriodLogic>{
	public static EventHandler<EventArgs> OnNextPlayPeriod;
	private DateTime nextPlayPeriodAux;

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

	void Start(){
		nextPlayPeriodAux = NextPlayPeriod;
		InvokeRepeating("NextPlayPeriodPolling", 1, 1);
	}

	void NextPlayPeriodPolling(){
		// If the time now crosses over the play play period
		if(nextPlayPeriodAux < LgDateTime.GetTimeNow()){
			nextPlayPeriodAux = NextPlayPeriod;	// Update the aux to the next play period
			// Fire event
			if(OnNextPlayPeriod != null){
				OnNextPlayPeriod(this, EventArgs.Empty);
			}
		}
	}

	public void SetLastPlayPeriod(){
		if(DataManager.Instance != null){
			if(!DataManager.Instance.GameData.PlayPeriod.IsFirstPlayPeriodAux){
				DataManager.Instance.GameData.PlayPeriod.IsFirstPlayPeriodAux = true;
				DataManager.Instance.GameData.PlayPeriod.FirstPlayPeriod = GetCurrentPlayPeriod();
			}
			else{
				DataManager.Instance.GameData.PlayPeriod.LastPlayPeriod = GetCurrentPlayPeriod();
			}
		}
	}

	public DateTime GetLastPlayPeriod(){
		return DataManager.Instance.GameData.PlayPeriod.LastPlayPeriod;
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			// Save current information
			SetLastPlayPeriod();

			//calculate time diff since last play session ended and submit to game analytics
			TimeSpan timeSinceLastSession = LgDateTime.GetTimeSpanSinceLastPlayed();
			int timeDifference = (int)timeSinceLastSession.TotalHours;
			
			Analytics.Instance.TimeBetweenPlaySession(timeDifference);
		}
	}

	void OnDestroy(){
		// Save current information on scene change
		SetLastPlayPeriod();
	}

	#region Inhaler use functions
	/// <summary>
	/// Check if user can play inhaler game.
	/// </summary>
	public bool CanUseEverydayInhaler(){
		bool retVal = DataManager.Instance.GameData.Inhaler.LastPlayPeriodUsed < GetCurrentPlayPeriod();

		// If you didnt finish tutorial-1 and the tutorial is done
		bool isPart1TutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialPart1Done();
		bool isInhalerTutorialDone = DataManager.Instance.GameData.Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_INHALER);
		if(!isPart1TutorialDone && isInhalerTutorialDone){
			retVal = false;
		}

		return retVal;
	}

	public DateTime GetLastInhalerTime(){
		return DataManager.Instance.GameData.Inhaler.LastInhalerPlayTime;
	}

	public void SetLastInhalerTime(DateTime lastTime){
		DataManager.Instance.GameData.Inhaler.LastInhalerPlayTime = lastTime;
	}

	/// <summary>
	/// Additional steps to be done after inhaler game is finished
	/// </summary>
	public void InhalerGameDonePostLogic(){

	}
	#endregion

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
		return (LgDateTime.GetTimeNow().Hour < 12) ? LgDateTime.Today : LgDateTime.Today.AddHours(12);
	}

	/// <summary>
	/// Calculates the time left till next play period.
	/// </summary>
	public TimeSpan CalculateTimeLeftTillNextPlayPeriod(){
		TimeSpan timeTillNextPlayPeriod = NextPlayPeriod - LgDateTime.GetTimeNow();

		if(timeTillNextPlayPeriod < TimeSpan.Zero){
			Debug.LogError("Negative timespan detected");
		}
		return timeTillNextPlayPeriod;
	}

	public bool IsFirstPlayPeriod(){
		if(Constants.GetConstant<bool>("OverrideFirstPlayPeriod")){
			return false;
		}
		else{
			return DataManager.Instance.GameData.PlayPeriod.LastPlayPeriod == DateTime.MinValue ||
				DataManager.Instance.GameData.PlayPeriod.FirstPlayPeriod == GetCurrentPlayPeriod();
		}
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
//			Debug.Log(CanUseEverydayInhaler());
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "2")){
//			Debug.Log(GetTimeFrame(GetCurrentPlayPeriod()));
//		}
//		if(GUI.Button(new Rect(300, 100, 100, 100), "3")){
//		}
//		if(GUI.Button(new Rect(400, 100, 100, 100), "4")){
//			Debug.Log(GetTimeFrame(NextPlayPeriod));
//		}
//	}
}
