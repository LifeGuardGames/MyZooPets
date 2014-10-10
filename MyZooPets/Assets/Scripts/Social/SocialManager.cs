using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ServerEventArgs : EventArgs{
	public bool IsSuccessful {get; set;}
	public ParseException.ErrorCode ErrorCode {get; set;}
	public string ErrorMessage {get; set;}
}

public class SocialManager : Singleton<SocialManager> {
	/// Friend request data type use in FriendsUIManager
	public class FriendRequest{
		public string RequestId {get; set;}
		public string FriendName {get; set;}
	}

	public static EventHandler<ServerEventArgs> OnDataRefreshed;
	public static EventHandler<ServerEventArgs> OnFriendRemoved;
	public static EventHandler<ServerEventArgs> OnFriendRequestRefreshed;
	public static EventHandler<ServerEventArgs> OnFriendCodeAdded;

	public bool useDummyData = false; // Turn this to true if testing with dummy data

	#region Properties
	/// <summary>
	/// Gets or sets the friend list.
	/// </summary>
	/// <value>The friend list.</value>
	public List<ParseObjectKidAccount> FriendList {get; set;}

	/// <summary>
	/// Gets or sets the friend requests.
	/// </summary>
	/// <value>The friend requests.</value>
	public List<FriendRequest> FriendRequests {get; set;}

	/// <summary>
	/// Gets or sets the account code. aka friend code
	/// </summary>
	/// <value>The account code.</value>
	public string AccountCode {get; set;}

	/// <summary>
	/// Gets or sets the user's social properties
	/// </summary>
	/// <value>The user social.</value>
	public ParseObjectSocial UserSocial {get; set;}
	#endregion

	#region Refresh Data
	/// <summary>
	/// Refreshs the data.
	/// </summary>
	public void RefreshData(){
		if(IsUsingDummyData()) return;
		ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
			return ParseCloud.CallFunctionAsync<IDictionary<string, object>>("getFriends", null);
		}).Unwrap().ContinueWith(t => {
			ServerEventArgs args = new ServerEventArgs();
			
			if(t.IsFaulted || t.IsCanceled){
				ParseException e = (ParseException) t.Exception.InnerExceptions[0];
				Debug.LogError("Message: " + e.Message + ", Code: " + e.Code);
				
				args.IsSuccessful = false;
				args.ErrorCode = e.Code;
				args.ErrorMessage = e.Message;
			}
			else{
				IDictionary<string, object> result = t.Result;
				// Hack, check for errors
				object code;
				
				if(result.TryGetValue("code", out code)){
//					Debug.LogError("Error Code: " + code);
					int parseCode = Convert.ToInt32(code);
					
					args.IsSuccessful = false;
					args.ErrorCode = (ParseException.ErrorCode) parseCode;
					args.ErrorMessage = (string) result["message"];
				} 
				else{
//					ParseObjectKidAccount kidAccount = (ParseObjectKidAccount) result["kidAccount"];
//					Loom.DispatchToMainThread(() => {
					var friendList = (IEnumerable) result["friendList"];
					AccountCode = (string) result["accountCode"];
					UserSocial = new ParseObjectSocial();
					UserSocial.NumOfStars = Convert.ToInt32(result["numOfStars"]);
					UserSocial.RewardCount = Convert.ToInt32(result["rewardCount"]);
					
					//assign FriedList if list is not null from server result
					FriendList = new List<ParseObjectKidAccount>();
					if(friendList != null){
						foreach(ParseObjectKidAccount friendAccount in friendList){
							FriendList.Add(friendAccount);
						}	
					}

					args.IsSuccessful = true;
//					});
				}
			}
			
			Loom.DispatchToMainThread(() => {
				if(OnDataRefreshed != null)
					OnDataRefreshed(this, args);
			});
		});
	}
	#endregion

	#region Send Friend Request
	/// <summary>
	/// Use this function to add/check the friend code.
	/// After you call this function you need to subscribe to it's call back event
	/// Appropriate successful or error events will be passed to the call back
	/// </summary>
	/// <param name="friendCode">Friend code.</param>
	public void SendFriendRequest(string friendCode){
		if(useDummyData){
			StartCoroutine(WaitForBadFriendRequest());
			return;
		}

		if(!string.IsNullOrEmpty(friendCode)){
			IDictionary<string, object> paramDict = new Dictionary<string, object>{
				{"friendCode", friendCode},
			};

			ParseCloud.CallFunctionAsync<IDictionary<string, object>>("sendFriendRequest", paramDict)
				.ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					ParseException e = (ParseException) t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + e.Message + ", Code: " + e.Code);
				
					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = false;
					args.ErrorCode = e.Code;
					args.ErrorMessage = e.Message;

					Loom.DispatchToMainThread(() => {	
						if(OnFriendCodeAdded != null)
							OnFriendCodeAdded(this, args);
					});
				} 
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;
					ServerEventArgs args = new ServerEventArgs();

					if(result.TryGetValue("code", out code)){
						Debug.Log("Error Code: " + code);
						int parseCode = Convert.ToInt32(code);

						args.IsSuccessful = false;
						args.ErrorCode = (ParseException.ErrorCode) parseCode;
						args.ErrorMessage = (string) result["message"];
					} 
					else{
						Debug.Log("Result: " + result["success"]);
						args.IsSuccessful = true;
					}
					
					Loom.DispatchToMainThread(() => {
						if(OnFriendCodeAdded != null)
							OnFriendCodeAdded(this, args);
					});
				}
			});
		}
		else{
			ServerEventArgs args = new ServerEventArgs();
			args.IsSuccessful = false;
			args.ErrorCode = ParseException.ErrorCode.ObjectNotFound;
			args.ErrorMessage = "friend code is required";

			if(OnFriendCodeAdded != null)
				OnFriendCodeAdded(this, args);
		}
	}
	#endregion

	#region Get Friend Request
	/// <summary>
	/// Gets the friend requests from backend. subscribe to OnFriendRequestRefreshed
	/// </summary>
	public void GetFriendRequests(){
		#region Dummy Data
		if(useDummyData){
			FriendRequests = new List<FriendRequest>();
			for(int i=0; i<3; i++){
				FriendRequest newRequest = new FriendRequest();
				newRequest.RequestId = "dummy Id";
				newRequest.FriendName = "no name";

				FriendRequests.Add(newRequest);
			}

			StartCoroutine(WaitForFriendRequest());
			return;
		}
		#endregion

		ParseCloud.CallFunctionAsync<IDictionary<string, object>>("getFriendRequests", null)
			.ContinueWith(t => {
			ServerEventArgs args = new ServerEventArgs();

			if(t.IsFaulted || t.IsCanceled){
				ParseException e = (ParseException) t.Exception.InnerExceptions[0];
				Debug.Log("Message: " + e.Message + ", Code: " + e.Code);

				args.IsSuccessful = false;
				args.ErrorCode = e.Code;
				args.ErrorMessage = e.Message;
			}
			else{
				IDictionary<string, object> result = t.Result;
				// Hack, check for errors
				object code;
				
				if(result.TryGetValue("code", out code)){
//					Debug.LogError("Error Code: " + code);
					int parseCode = Convert.ToInt32(code);
					
					args.IsSuccessful = false;
					args.ErrorCode = (ParseException.ErrorCode) parseCode;
					args.ErrorMessage = (string) result["message"];
				} 
				else{
					var friendRequests = (IEnumerable) result["success"];
					FriendRequests = new List<FriendRequest>();

					foreach(ParseObject friendRequest in friendRequests){
						string requestId = friendRequest.ObjectId;
						ParseObject fromKidAccount = (ParseObject) friendRequest["from"];
						ParseObjectPetInfo requestPetInfo = (ParseObjectPetInfo) fromKidAccount["petInfo"];

						FriendRequest newRequest = new FriendRequest();
						newRequest.RequestId = requestId;
					
						string friendName = "";
						if(requestPetInfo != null){
							friendName = requestPetInfo.Name;
						}
						newRequest.FriendName = friendName;

						FriendRequests.Add(newRequest);
					}

					args.IsSuccessful = true;
				}
			}

			Loom.DispatchToMainThread(() => {
				if(OnFriendRequestRefreshed != null)
					OnFriendRequestRefreshed(this, args);
			});
		});	
	}
	#endregion

	#region Friend Request Action
	/// <summary>
	/// Accepts the friend request. Subscribe to OnFriendRequestRefresh for callback 
	/// </summary>
	/// <param name="requestId">Request identifier.</param>
	public void AcceptFriendRequest(string requestId){
		FriendRequestAction("acceptFriendRequest", requestId);
	}

	/// <summary>
	/// Rejects the friend request. Subscribe to OnFriendRequestRefresh for callback
	/// </summary>
	/// <param name="requestId">Request identifier.</param>
	public void RejectFriendRequest(string requestId){
		FriendRequestAction("rejectFriendRequest", requestId);
	}

	private void FriendRequestAction(string cloudFunctionName, string requestId){
		if(useDummyData){
			StartCoroutine(WaitForFriendRequest());
			return;
		}

		if(!string.IsNullOrEmpty(cloudFunctionName)){
			IDictionary<string, object> paramDict = new Dictionary<string, object>{
				{"requestId", requestId},
			};
			
			ParseCloud.CallFunctionAsync<IDictionary<string, object>>(cloudFunctionName, paramDict)
				.ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					ParseException e = (ParseException) t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + e.Message + ", Code: " + e.Code);
					
					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = false;
					args.ErrorCode = e.Code;
					args.ErrorMessage = e.Message;
					
					Loom.DispatchToMainThread(() => {
						GetFriendRequests();
					});
				} 
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;
					ServerEventArgs args = new ServerEventArgs();
					
					if(result.TryGetValue("code", out code)){
//						Debug.Log("Error Code: " + code);
						int parseCode = Convert.ToInt32(code);
						
						args.IsSuccessful = false;
						args.ErrorCode = (ParseException.ErrorCode) parseCode;
						args.ErrorMessage = (string) result["message"];
						
						Loom.DispatchToMainThread(() => {
							if(OnFriendRequestRefreshed != null)
								OnFriendRequestRefreshed(this, args);
						});
					} 
					else{
//						Debug.Log("Result: " + result["success"]);
						Loom.DispatchToMainThread(() => {
							RefreshData();
							GetFriendRequests();
						});
					}
				}
			});
		}
	}
	#endregion

	#region Remove Friend
	/// <summary>
	/// Removes the friend. Subscribe to OnDataRefreshed to know when the remove is
	/// complete.
	/// </summary>
	/// <param name="friendObjectId">Friend object identifier.</param>
	public void RemoveFriend(string friendObjectId){
		if(useDummyData){
			StartCoroutine(RemoveFriendGood());
			return;
		}

		if(!string.IsNullOrEmpty(friendObjectId)){
			IDictionary<string, object> paramDict = new Dictionary<string, object>{
				{"friendObjectId", friendObjectId},
			};
			
			ParseCloud.CallFunctionAsync<IDictionary<string, object>>("removeFriend", paramDict)
				.ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					ParseException e = (ParseException) t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + e.Message + ", Code: " + e.Code);
					
					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = false;
					args.ErrorCode = e.Code;
					args.ErrorMessage = e.Message;
					
					Loom.DispatchToMainThread(() => {
						if(OnFriendRemoved != null)
							OnFriendRemoved(this, args);
					});
				} 
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;
					ServerEventArgs args = new ServerEventArgs();
					
					if(result.TryGetValue("code", out code)){
						Debug.Log("Error Code: " + code);
						int parseCode = Convert.ToInt32(code);
						
						args.IsSuccessful = false;
						args.ErrorCode = (ParseException.ErrorCode) parseCode;
						args.ErrorMessage = (string) result["message"];

						
					} 
					else{
						Debug.Log("Result: " + result["success"]);
						args.IsSuccessful = true;
						
						Loom.DispatchToMainThread(() => {
							RefreshData();
						});
					}

					Loom.DispatchToMainThread(() => {
						if(OnFriendRemoved != null)
							OnFriendRemoved(this, args);
					});
				}
			});
		}
	}
	#endregion

	#region Claim Reward
	/// <summary>
	/// Claims the friend referral reward.
	/// Call this method when the reward is given to the user
	/// </summary>
	public void ClaimFriendReferralReward(){
		if(useDummyData){
			return;
		}

		ParseCloud.CallFunctionAsync<IDictionary<string, object>>("claimReferralReward", null)
		.ContinueWith(t => {
			if(t.IsFaulted || t.IsCanceled){
				ParseException e = (ParseException) t.Exception.InnerExceptions[0];
				Debug.Log("Message: " + e.Message + ", Code: " + e.Code);
				
				ServerEventArgs args = new ServerEventArgs();
				args.IsSuccessful = false;
				args.ErrorCode = e.Code;
				args.ErrorMessage = e.Message;
				
			
			} 
			else{
				IDictionary<string, object> result = t.Result;
				// Hack, check for errors
				object code;
				ServerEventArgs args = new ServerEventArgs();
				
				if(result.TryGetValue("code", out code)){
//					Debug.Log("Error Code: " + code);
					int parseCode = Convert.ToInt32(code);
					
				
				} 
				else{
					Debug.Log("Result: " + result["success"]);
				}
			}
		});
	}
	#endregion

	#region Dummy Data
	private bool IsUsingDummyData(){
		bool retVal = false; 
		if(useDummyData){
			retVal = useDummyData;
			StartCoroutine(GoodFriendData());
		}
		
		return retVal;
	}

	private IEnumerator GoodFriendData(){
		yield return new WaitForSeconds(2f);
		List<ParseObjectKidAccount> dummyData = new List<ParseObjectKidAccount>();
		
		//set up dummy data
		for(int i=0; i<3; i++){
			ParseObjectPetInfo petInfo = new ParseObjectPetInfo();
			petInfo.Name = "dummy" + i;
			
			ParseObjectKidAccount account = new ParseObjectKidAccount();
			account.AccountCode = "testing code";
			account.PetInfo = petInfo;
			account.ObjectId = "testing12" + i;
			
			dummyData.Add(account);
		}

		ParseObjectSocial social = new ParseObjectSocial();
		social.NumOfStars = 0;
		social.RewardCount = 1;

		UserSocial = social;
		FriendList = dummyData;
		AccountCode = "dummydata369";
		
		ServerEventArgs args = new ServerEventArgs();
		args.IsSuccessful = true;
		
		if(OnDataRefreshed != null)
			OnDataRefreshed(this, args);
	}

	private IEnumerator BadFriendData(){
		yield return new WaitForSeconds(5f);

		ServerEventArgs args = new ServerEventArgs();
		args.IsSuccessful = false;
		args.ErrorCode = ParseException.ErrorCode.OtherCause;
		args.ErrorMessage = "Cannot connect to internet";
		
		if(OnDataRefreshed != null)
			OnDataRefreshed(this, args);

	}

	private IEnumerator RemoveFriendGood(){
		yield return new WaitForSeconds(2f);
		
		ServerEventArgs args = new ServerEventArgs();
		args.IsSuccessful = true;

		Debug.Log("firing remove friend");
		if(OnFriendRemoved != null)
			OnFriendRemoved(this, args);

		RefreshData();
	}

	private IEnumerator WaitForSendFriendRequest(){
		yield return new WaitForSeconds(2f);
		
		ServerEventArgs args = new ServerEventArgs();
		args.IsSuccessful = true;

		if(OnFriendCodeAdded != null)
			OnFriendCodeAdded(this, args);
	}

	private IEnumerator WaitForBadFriendRequest(){
		yield return new WaitForSeconds(2f);
		
		ServerEventArgs args = new ServerEventArgs();
		args.IsSuccessful = false;
		args.ErrorCode = ParseException.ErrorCode.ObjectNotFound;
		args.ErrorMessage = "objecct now found";
		
		if(OnFriendCodeAdded != null)
			OnFriendCodeAdded(this, args);
	}

	private IEnumerator WaitForFriendRequest(){
		yield return new WaitForSeconds(2f);

		ServerEventArgs args = new ServerEventArgs();
		args.IsSuccessful = true;

		if(OnFriendRequestRefreshed != null)
			OnFriendRequestRefreshed(this, args);
	}
	#endregion
}
