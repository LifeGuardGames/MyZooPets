using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum ErrorCodes{
	None,
	ObjectNotFound,
	ConnectionError,
	DuplicateValue
}

public class ServerEventArgs : EventArgs{
	public bool IsSuccessful {get; set;}
	public ErrorCodes ErrorCode {get; set;}
	public string ErrorMessage {get; set;}
}

public class SocialManager : Singleton<SocialManager> {
	public static EventHandler<ServerEventArgs> OnDataRefreshed;
	public static EventHandler<ServerEventArgs> OnFriendCodeAdded;

	/// <summary>
	/// Refreshs the data.
	/// </summary>
	public void RefreshData(){
		try{
			ExtraParseLogic.Instance.UserAndKidAccountCheck().ContinueWith(t => {
				KidAccount kidAccount = t.Result;

				ParseQuery<KidAccount> friendListQuery = new ParseQuery<KidAccount>()
					.Include("friendList.petInfo")
					.WhereEqualTo("objectId", kidAccount.ObjectId);


				return friendListQuery.GetAsync(kidAccount.ObjectId);
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					// Errors from Parse Cloud and network interactions
					ParseException exception = (ParseException)t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + exception.Message + ", Code: " + exception.Code);

					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = false;
						args.ErrorCode = ErrorCodes.ConnectionError;

						if(OnDataRefreshed != null)
							OnDataRefreshed(this, args);
					});
				}
				else{
					KidAccount account = t.Result;

					ParseObject petInfo = new ParseObject("PetInfo");
					petInfo = account.Get<ParseObject>("petInfo");
					Debug.Log("pet info data available: " + petInfo.IsDataAvailable);
//					List<ParseObject> friendList = account.FriendList;

//					Debug.Log(petInfo.Get<string>("name"));
					if(account.FriendList != null){
						Debug.Log(account.FriendList.Count);
						foreach(KidAccount friendAccount in account.FriendList){
							Debug.Log("friend object id: " + friendAccount.ObjectId);
							Debug.Log("Friend Account is linked?: " + friendAccount.IsLinkedToParentAccount);

							ParseObject friendPetInfo = new ParseObject("PetInfo");
							friendPetInfo = friendAccount.PetInfoPointer;
							Debug.Log("Friend Account pet info data available?: " + friendAccount.IsDataAvailable);
						}
					}

					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = true;
						args.ErrorCode = ErrorCodes.None;
						
						if(OnDataRefreshed != null)
							OnDataRefreshed(this, args);
					});
				}
			});
		}
		catch(InvalidOperationException e){
			Debug.LogException(e);

			ServerEventArgs args = new ServerEventArgs();
			args.IsSuccessful = false;
			args.ErrorCode = ErrorCodes.ConnectionError;
			
			if(OnDataRefreshed != null)
				OnDataRefreshed(this, args);
		}
	}
	
	/// <summary>
	/// Use this function to add/check the friend code.
	/// After you call this function you need to subscribe to it's call back event
	/// Appropriate successful or error events will be passed to the call back
	/// </summary>
	/// <param name="friendCode">Friend code.</param>
	public void AddFriendCode(string friendCode){
		if(!string.IsNullOrEmpty(friendCode)){
			IDictionary<string, object> paramDict = new Dictionary<string, object>{
				{"friendCode", friendCode},
				{"kidAccountId", DataManager.Instance.GameData.PetInfo.ParseKidAccountID}
			};

			ParseCloud.CallFunctionAsync<IDictionary<string, object>>("addFriendCode", paramDict)
				.ContinueWith(t => {
				if(t.IsFaulted){
					foreach(ParseException e in t.Exception.InnerExceptions)
						Debug.Log("Message: " + e.Message + ", Code: " + e.Code);

					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = false;
						args.ErrorCode = ErrorCodes.ConnectionError;
						
						if(OnFriendCodeAdded != null)
							OnFriendCodeAdded(this, args);
					});
				} 
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;
					if(result.TryGetValue("code", out code)){
//						Debug.Log("Error Code: " + code);
	
						int parseCode = Convert.ToInt32(code);
						ErrorCodes errorCode = ErrorCodes.None;

						switch(parseCode){
							case 101:
								errorCode = ErrorCodes.ObjectNotFound;
								break;
						}

						Loom.DispatchToMainThread(() => {
							ServerEventArgs args = new ServerEventArgs();
							args.IsSuccessful = false;
							args.ErrorCode = errorCode;
							
							if(OnFriendCodeAdded != null)
								OnFriendCodeAdded(this, args);
						});
					} 
					else{
						Debug.Log("Result: " + result["success"]);
					}
				}
			});
		}
	}
}
