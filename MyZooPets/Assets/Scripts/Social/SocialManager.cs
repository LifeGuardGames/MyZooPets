using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

//public enum ErrorCodes{
//	None,
//	ObjectNotFound,
//	ConnectionError,
//	DuplicateValue,
//	BadInput
//}

public class ServerEventArgs : EventArgs{
	public bool IsSuccessful {get; set;}
	public ParseException.ErrorCode ErrorCode {get; set;}
	public string ErrorMessage {get; set;}
}

public class SocialManager : Singleton<SocialManager> {
	public static EventHandler<ServerEventArgs> OnDataRefreshed;
	public static EventHandler<ServerEventArgs> OnFriendCodeAdded;

	/// <summary>
	/// Gets or sets the friend list.
	/// Need to be cast to List<KidAccount> using a foreach loop to gain access
	/// to the property in KidAccount
	/// </summary>
	/// <value>The friend list.</value>
	public List<ParseObjectKidAccount> FriendList {get; set;}

	/// <summary>
	/// Refreshs the data.
	/// </summary>
	public void RefreshData(){
		try{
			ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
				ParseQuery<ParseObjectKidAccount> friendListQuery = new ParseQuery<ParseObjectKidAccount>()
					.WhereEqualTo("createdBy", ParseUser.CurrentUser)
					.Include("friendList.petInfo")
					.Include("friendList.petAccessory");

				return friendListQuery.FirstAsync();
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					// Errors from Parse Cloud and network interactions
					ParseException exception = (ParseException)t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + exception.Message + ", Code: " + exception.Code);

					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = false;
					args.ErrorCode = ParseException.ErrorCode.ConnectionFailed;

					if(OnDataRefreshed != null)
						OnDataRefreshed(this, args);
				}
				else{
					ParseObjectKidAccount account = t.Result;

					if(account.FriendList != null){
						Debug.Log(account.FriendList.Count);
						FriendList = account.FriendList.ToList();
				
					}

					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = true;
					
					if(OnDataRefreshed != null)
						OnDataRefreshed(this, args);
				}
			});
		}
		catch(InvalidOperationException e){
			Debug.LogException(e);

			ServerEventArgs args = new ServerEventArgs();
			args.IsSuccessful = false;
			args.ErrorCode = ParseException.ErrorCode.OperationForbidden;
			
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
			};

			ParseCloud.CallFunctionAsync<IDictionary<string, object>>("addFriendCode", paramDict)
				.ContinueWith(t => {
				if(t.IsFaulted){
					ParseException e = (ParseException) t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + e.Message + ", Code: " + e.Code);

					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = false;
					args.ErrorCode = ParseException.ErrorCode.ConnectionFailed;
					
					
					if(OnFriendCodeAdded != null)
						OnFriendCodeAdded(this, args);
				} 
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;
					if(result.TryGetValue("code", out code)){
						Debug.Log("Error Code: " + code);
	
						int parseCode = Convert.ToInt32(code);

						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = false;
						args.ErrorCode = (ParseException.ErrorCode) parseCode;
						args.ErrorMessage = (string) result["message"];
						
						if(OnFriendCodeAdded != null)
							OnFriendCodeAdded(this, args);
					} 
					else{
						Debug.Log("Result: " + result["success"]);
					}
				}
			});
		}
	}
}
