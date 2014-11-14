using UnityEngine;
using System.Collections;

public class MembershipCheck : MonoBehaviour {
	private static bool isCreated;
	private bool isConnectedToInternet;

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

	void OnApplicationPause(bool isPaused){
		if(!isPaused){

		}
	}

	private IEnumerator CheckMembershipStatus(){
		//Try to ping the server
		WWW www = new WWW("http://www.google.com");
		
		//Wait for response
		yield return www;

		isConnectedToInternet = false;

		CheckInternetConnectivity(www);

		//Do the 3 day free trial check and time update here
		if(isConnectedToInternet){
			Debug.Log("connection ok");
			//Connect to server and check trial status
			//If trial is over check membership subscription status
				//If subscription status inactive load menu and prompt user to subscribe
				//Else check if egg hatch. yes -> bedroom, no -> menu
			//Else check if egg hatch. yes -> bedroom, no -> menu
		}
		else{
			Debug.Log("connection failed");
			DataManager.Instance.AccumulatedConnectionErrors++;
			//if conection failed check for accumulated connection errors
			int accumulatedConnectionErrors = DataManager.Instance.AccumulatedConnectionErrors;

			//if < 3 check if egg hatch. yes -> bedroom , no -> menu
			if(accumulatedConnectionErrors < 3){
				bool isHatched = DataManager.Instance.GameData.PetInfo.IsHatched;
				if(isHatched)
					Application.LoadLevel(SceneUtils.BEDROOM);
				else
					Application.LoadLevel(SceneUtils.MENU);
			}
			//else load menu and show error message
			else{
				Application.LoadLevel(SceneUtils.MENU);
			}

			//save start time locally if first time playing
			bool isFirstTime = DataManager.Instance.IsFirstTime;
			if(isFirstTime)
				DataManager.Instance.TrialStartTimeStamp = LgDateTime.GetUtcNowTimestamp();
		}
	}

	/// <summary>
	/// Checks the internet connectivity.
	/// Uses Game Analytics code to check for server response. 
	/// </summary>
	/// <returns>The internet connectivity.</returns>
	private void CheckInternetConnectivity(WWW www){
		try{
			if(GA_Submit.CheckServerReply(www)){
				isConnectedToInternet = true;
			}
			else if(!string.IsNullOrEmpty(www.error)){
				isConnectedToInternet = false;
			}
			else{
				//Get the JSON object from the response
				Hashtable returnParam = (Hashtable)GA_MiniJSON.JsonDecode(www.text);
				
				//If the response contains the key "status" with the value "ok" we know that we are connected
				if(returnParam != null && returnParam.ContainsKey("status") && 
				   returnParam["status"].ToString().Equals("ok")){
					isConnectedToInternet = true;
				}
				else{
					isConnectedToInternet = false;
				}
			}
		}
		catch{
			isConnectedToInternet = false;
		}
	}	
}
