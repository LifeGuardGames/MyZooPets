using UnityEngine;
using System.Collections;
using System;
using Parse;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class MembershipCheck : Singleton<MembershipCheck> {
	public enum Errors{
		None,
		OverConnectionErrorLimit,
		TrialExpired,
		MembershipExpired
	}
	public enum Status{
		Active,
		Expired
	}
	public static EventHandler<EventArgs> OnCheckDoneEvent; //Event fired when membership check is finished

	private static bool isCreated;

	/// <summary>
	/// If isTestMode:True then trial time will be set to 3 minutes, and scene
	/// transitions woun't actually happen. 
	/// </summary>
	public bool isTestMode = false;

	private bool isConnectedToInternet;
	private bool runTimeOutTimer = false; // to run the time out timer or not
	private float timeOutTimer = 0;
	private float timeOut = 20f; //time out set to 20 seconds
	private CancellationTokenSource timeOutRequestCancellation; //token used to cancel unfinish task/thread when timeout timer is up

	/// <summary>
	/// Tells the UI what error the membership check failed with so UI can spawn the
	/// right notification
	/// </summary>
	/// <value>The membership check error.</value>
	public Errors MembershipCheckError {get; set;}



	void Awake(){
		//--------------------Make Object persistent---------------------------
		if(isCreated){
			//If There is a duplicate in the scene. delete the object and jump Awake
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		isCreated = true;
		//---------------------------------------------------------------------
	}

	/// <summary>
	/// Keeps tracks of the timeout timer. cancel the server connection of it takes too long
	/// </summary>
	void Update(){
		if(runTimeOutTimer){
			timeOutTimer += Time.deltaTime;
			if(timeOutTimer >= timeOut){
				timeOutTimer = 0;
				runTimeOutTimer = false;
				timeOutRequestCancellation.Cancel();
			}
		}
	}

	/// <summary>
	/// Raises the application pause event.
	/// </summary>
	/// <param name="isPaused">If set to <c>true</c> is paused.</param>
	void OnApplicationPause(bool isPaused){

		//When player resumes the game we first check how long has it been since
		//the last play session. If play session time > 30min we force the game
		//to start from LoadingScene.unity again. This is to prevent any game logic
		//from running while the membership check is going on. 
		if(!isPaused){
			DateTime lastPlaySessionDate = DataManager.Instance.LastPlaySessionDate;
			TimeSpan timeSinceLastPlaySession = LgDateTime.GetTimeNow() - lastPlaySessionDate;

			if(timeSinceLastPlaySession.TotalMinutes > 30){
				Application.LoadLevel(SceneUtils.LOADING);
			}
		}
		else{
			DataManager.Instance.LastPlaySessionDate = LgDateTime.GetTimeNow();
		}
	}

	public void StartCheck(){
		Debug.Log("Start check");
		StartCoroutine(CheckMembershipStatus());
	}

	/// <summary>
	/// Checks the membership status. 
	/// 1) Ping the backend server to check connectivity
	/// 2) Then check trial or membership status
	/// </summary>
	/// <returns>The membership status.</returns>
	private IEnumerator CheckMembershipStatus(){
		//Try to ping the server
		WWW www = new WWW("https://wellapetstest.parseapp.com/ping");

		//Store the date this check happens
		DataManager.Instance.AddMembershipCheckDate(LgDateTime.GetUtcNowTimestamp());
		
		//Wait for response
		yield return www;

		isConnectedToInternet = false;

		CheckInternetConnectivity(www);

		//Do the 3 day free trial check and time update here
		if(isConnectedToInternet){
			Debug.Log("connection ok");
			IDictionary<string, object> paramDict = new Dictionary<string, object>();
			paramDict.Add("membershipCheckDates", DataManager.Instance.MembershipCheckDates);
			
			ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
				StartTimeOutTimer();
				//Connect to server and check trial status
				return ParseCloud.CallFunctionAsync<IDictionary<string, object>>("checkTrialAndMembershipStatus", 
				                                                                 paramDict,
				                                                                 timeOutRequestCancellation.Token);
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					ParseException.ErrorCode code = ParseException.ErrorCode.OtherCause;
					string message = "time out by client";
					
					if(t.Exception != null){
						ParseException e = (ParseException) t.Exception.InnerExceptions[0];
						code = e.Code;
						message = e.Message;
						Debug.LogError("Message: " + e.Message + ", Code: " + e.Code);
					}

					Loom.DispatchToMainThread(() => {
						ConnectionFailed();
					});
				}
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;

					Loom.DispatchToMainThread(() => {
						if(result.TryGetValue("code", out code)){
							//Debug.LogError("Error Code: " + code);
							int parseCode = Convert.ToInt32(code);
							Debug.LogError("Error Code: " + parseCode + " " + result["message"]);
						} 
						else{
							if(result.ContainsKey("trialStatus")){
								string trialStatus = (string) result["trialStatus"];
								
								if(trialStatus == "expired"){
									Debug.Log("trial expired");

									//set error message so UI know what notification to spawn
									MembershipCheckError = Errors.TrialExpired;

									//reset
									DataManager.Instance.ResetMembershipCheckDates();
									CheckSceneToLoadInto();
								}
								else{
									Debug.Log("trial active");
									CheckSceneToLoadInto(true);
								}
							}
							else{
								//reset 
								DataManager.Instance.ResetMembershipCheckDates();

								string membershipStatus = (string) result["membershipStatus"];
								
								//If subscription status inactive load menu and prompt user to subscribe
								if(membershipStatus == "expired"){
									Debug.Log("membership expired");
									MembershipCheckError = Errors.MembershipExpired;
									CheckSceneToLoadInto();
								}
								//if egg hatch. yes -> bedroom , no -> menu
								else{
									Debug.Log("membership active");
									CheckSceneToLoadInto(true);
								}
							}
						}

						//after successfull connection with backend reset accumulated errors
						DataManager.Instance.AccumulatedConnectionErrors = 0;
					});
				}

				StopTimeOutTimer();
			});
		}
		else{
			Debug.Log("connection failed");
			ConnectionFailed();
		}
	}

	private void ConnectionFailed(){
		//only record connection error in the loading scene. Otherwise connection
		//error will accumulate too fast
		if(string.Equals(Application.loadedLevelName, SceneUtils.LOADING))
			DataManager.Instance.AccumulatedConnectionErrors++;

		//if conection failed check for accumulated connection errors
		int accumulatedConnectionErrors = DataManager.Instance.AccumulatedConnectionErrors;
		
		//if <= 3 check if egg hatch. yes -> bedroom , no -> menu
		if(accumulatedConnectionErrors <= 3){
			Debug.Log("less than 3 connection errors");
			CheckSceneToLoadInto(true);
		}
		//else load menu and show error message
		else{
			Debug.Log("more than 3 connection error");
			MembershipCheckError = Errors.OverConnectionErrorLimit;
			CheckSceneToLoadInto();
		}
	}
	
	private void CheckSceneToLoadInto(bool performHatchCheck = false){
		if(!isTestMode){
			if(performHatchCheck){
				bool isHatched = DataManager.Instance.GameData.PetInfo.IsHatched;
				if(isHatched){
					if(!string.Equals(Application.loadedLevelName, SceneUtils.BEDROOM))
						Application.LoadLevel(SceneUtils.BEDROOM);
				}
				else{
					if(!string.Equals(Application.loadedLevelName, SceneUtils.MENU))
						Application.LoadLevel(SceneUtils.MENU);
				}
			}
			else{
				if(!string.Equals(Application.loadedLevelName, SceneUtils.MENU))
					Application.LoadLevel(SceneUtils.MENU);
			}
		}

		if(OnCheckDoneEvent != null)
			OnCheckDoneEvent(this, EventArgs.Empty);
	}
	
	/// <summary>
	/// Checks the internet connectivity.
	/// </summary>
	/// <returns>The internet connectivity.</returns>
	private void CheckInternetConnectivity(WWW www){
		if(!string.IsNullOrEmpty(www.error))
			isConnectedToInternet = false;
		else{
			if(www.text == "OK")
				isConnectedToInternet = true;
		}
	}

	/// <summary>
	/// Starts the time out timer.
	/// </summary>
	private void StartTimeOutTimer(){
		runTimeOutTimer = true;
		timeOutRequestCancellation = new CancellationTokenSource();
	}
	
	/// <summary>
	/// Stops the time out timer.
	/// </summary>
	private void StopTimeOutTimer(){
		runTimeOutTimer = false;
		timeOutTimer = 0;
	}
}
