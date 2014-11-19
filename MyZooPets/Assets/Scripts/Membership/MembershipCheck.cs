using UnityEngine;
using System.Collections;
using System;
using Parse;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class MembershipCheck : MonoBehaviour {
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

		StartCoroutine(CheckMembershipStatus());
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

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			StartCoroutine(CheckMembershipStatus());
		}
	}

	public void MembershipTest(){
		StartCoroutine(CheckMembershipStatus());
	}

	private IEnumerator CheckMembershipStatus(){
		//Try to ping the server
		WWW www = new WWW("https://wellapetstest.parseapp.com/ping");
		
		//Wait for response
		yield return www;

		isConnectedToInternet = false;

		CheckInternetConnectivity(www);

		//Do the 3 day free trial check and time update here
		if(isConnectedToInternet){
			Debug.Log("connection ok");
			IDictionary<string, object> paramDict = new Dictionary<string, object>();
			paramDict.Add("isTestingMode", isTestMode);

			bool isFirstTime = DataManager.Instance.IsFirstTime;
			string startTime = LgDateTime.GetUtcNowTimestamp();

			//if first time playing send trial start time
			if(isFirstTime){
				paramDict.Add("trialStart", startTime);
			}
			//if not first time. check if a local trial start time has been cached
			//cached time means there's an unsuccessful connection previously
			else{
				string trialStartTimeStamp = DataManager.Instance.TrialStartTimeStamp;
				if(!string.IsNullOrEmpty(trialStartTimeStamp)){
					startTime = trialStartTimeStamp;
				}
			}
			
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
					
					if(result.TryGetValue("code", out code)){
						//Debug.LogError("Error Code: " + code);
						int parseCode = Convert.ToInt32(code);
						Debug.LogError("Error Code: " + parseCode + " " + result["message"]);
					} 
					else{
						string trialStatus = (string) result["trialStatus"];
						string membershipStatus = (string) result["membershipStatus"];

						Loom.DispatchToMainThread(() => {
							//If trial is over check membership subscription status
							if(trialStatus == "expired"){
								//If subscription status inactive load menu and prompt user to subscribe
								if(membershipStatus == "expired"){
									Debug.Log("membership expired");
									CheckSceneToLoadInto();
								}
								//if egg hatch. yes -> bedroom , no -> menu
								else{
									Debug.Log("membership active");
									CheckSceneToLoadInto(true);
								}
							}
							//if egg hatch. yes -> bedroom , no -> menu
							else{
								Debug.Log("trial active");
								CheckSceneToLoadInto(true);
							}

							//after successfull connection with backend remove cached trial start time
							DataManager.Instance.TrialStartTimeStamp = "";
						});
					}
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
		DataManager.Instance.AccumulatedConnectionErrors++;
		//if conection failed check for accumulated connection errors
		int accumulatedConnectionErrors = DataManager.Instance.AccumulatedConnectionErrors;
		
		//if < 3 check if egg hatch. yes -> bedroom , no -> menu
		if(accumulatedConnectionErrors < 3){
			Debug.Log("less than 3 connection errors");
			CheckSceneToLoadInto(true);
		}
		//else load menu and show error message
		else{
			Debug.Log("more than 3 connection error");
			CheckSceneToLoadInto();
		}
		
		//save start time locally if first time playing
		bool isFirstTime = DataManager.Instance.IsFirstTime;
		if(isFirstTime)
			DataManager.Instance.TrialStartTimeStamp = LgDateTime.GetUtcNowTimestamp();
	}
	
	private void CheckSceneToLoadInto(bool performHatchCheck = false){
//		if(!isTestMode){
			if(performHatchCheck){
				bool isHatched = DataManager.Instance.GameData.PetInfo.IsHatched;
				if(isHatched)
					if(!Application.loadedLevelName == SceneUtils.BEDROOM)
						Application.LoadLevel(SceneUtils.BEDROOM);
				else
					Application.LoadLevel(SceneUtils.MENU);
			}
			else{
				Application.LoadLevel(SceneUtils.MENU);
			}
//		}
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
