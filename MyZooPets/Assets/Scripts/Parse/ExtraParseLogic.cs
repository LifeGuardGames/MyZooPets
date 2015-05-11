using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;

public class ExtraParseLogic : ServerConnector<ExtraParseLogic>{

	protected override void Awake(){
		ParseObject.RegisterSubclass<ParseObjectKidAccount>();
		ParseObject.RegisterSubclass<ParseObjectPetInfo>();
		ParseObject.RegisterSubclass<ParseObjectSocial>();
	}

	// Use this for initialization
	protected override void Start(){
		//start a loom single thread here so we can use DispatchToMainThread
		//in one nof Parse's worker thread
		Loom.StartSingleThread(() => {});
	}

	/// <summary>
	/// Check if Parse User is login and check if kid account exist.
	/// Call this method everytime before you try to any data to the backend.
	/// 
	/// Both ParseUser and KidAccount will be created if they don't already exist
	/// </summary>
	/// <returns>Task with current KidAccount object</returns>
	public Task UserCheck(){
		var user = ParseUser.CurrentUser;
		var source = new TaskCompletionSource<string>();

		//user not login yet
		if(user == null){
			//sign up and create kid account
			CreateParseUser().ContinueWith(t => {
				return CreateKidAccount();
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled)
					source.SetException(t.Exception);
				else
					source.SetResult("User account valid");
			});
			// or login .... will implement later
		}
		else{
			//need to do a extra check here to see if KidAccount has been created
			//there seems to be a problem in v1.4.1 where the user was created
			//but the KidAccount didn't. This creates one more request to the backend
			//whenever UserCheck() is called, but it's a more fail safe method.

			StartTimeOutTimer();
			
			ParseCloud.CallFunctionAsync<IDictionary<string, object>>("checkKidAccount",
			                                                          null,
			                                                          timeOutRequestCancellation.Token)
			.ContinueWith(t => {
				
				ServerEventArgs args = new ServerEventArgs();
				
				if(t.IsFaulted || t.IsCanceled){
					ParseException.ErrorCode code = ParseException.ErrorCode.OtherCause;
					string message = "time out by client";
					
					if(t.Exception != null){
						ParseException e = (ParseException) t.Exception.InnerExceptions[0];
						code = e.Code;
						message = e.Message;
						//Debug.LogError("Message: " + e.Message + ", Code: " + e.Code);
					}
					
					Loom.DispatchToMainThread(() => {
						source.SetCanceled();
					});
				} 
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;
	
					if(result.TryGetValue("code", out code)){
						//Debug.Log("Error Code: " + code);
						//int parseCode = Convert.ToInt32(code);
						Loom.DispatchToMainThread(() => {
							source.SetCanceled();
						});
					} 
					else{
						Loom.DispatchToMainThread(() => {
							source.SetResult("User account valid");
						});
					}
				}
				StopTimeOutTimer();
			});
		}
		return source.Task;
	}

	/// <summary>
	/// Creates the parse new user.
	/// </summary>
	/// <returns>Task.</returns>
	private Task CreateParseUser(){
		string guid = Guid.NewGuid().ToString();
		
		var user = new ParseUser(){
			Username = guid,
			Password = guid
		};
		
		user["userType"] = "kid";
		return user.SignUpAsync();
	}

	/// <summary>
	/// Creates the new kid account.
	/// </summary>
	/// <returns>Task with KidAccountID</returns>
	private Task CreateKidAccount(){
		var user = ParseUser.CurrentUser;
		ParseACL acl = new ParseACL(user);
		acl.PublicReadAccess = true;
		acl.PublicWriteAccess = false;

		var account = new ParseObjectKidAccount{
			CreatedBy = user,
			ACL = acl
		};

		return account.SaveAsync();
	}

#if UNITY_EDITOR
	public void CreateTestUser(){
//		var source = new TaskCompletionSource<string>();

		CreateParseUser().ContinueWith(t => {
			return CreateKidAccount();
		}).Unwrap().ContinueWith(t => {
//			if(t.IsFaulted || t.IsCanceled)
//				source.SetException(t.Exception);
//			else
//				source.SetResult("User account valid");
		});
	}
	#endif
}

