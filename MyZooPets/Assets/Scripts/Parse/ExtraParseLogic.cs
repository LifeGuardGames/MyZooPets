using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;

public class ExtraParseLogic : Singleton<ExtraParseLogic>{
	
	/*
	 * Update logic:
	 *   query for the ParseObject created by KidAccount
	 * 
	 * var query = ParseObject.GetQuery("Badge")
     * .WhereEqualTo("createdBy", ParseObject.CreateWithoutData("KidAccount", "1zEcyElZ80"));
	 * 
	 *   continue with changing the appropriate values then save async
	 * 
	 * get data by id
	 * 
	 * ParseQuery<ParseObject> query = ParseObject.GetQuery("KidAccount");
     *  query.GetAsync("xWMyZ4YEGZ").ContinueWith
	 */

	void Awake(){
		ParseObject.RegisterSubclass<ParseObjectKidAccount>();
		ParseObject.RegisterSubclass<ParseObjectPetInfo>();
		ParseObject.RegisterSubclass<ParseObjectSocial>();
	}

	// Use this for initialization
	void Start(){
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
			source.SetResult("User account valid");
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
//		var source = new TaskCompletionSource<string>();
		var user = ParseUser.CurrentUser;
		ParseACL acl = new ParseACL(user);
		acl.PublicReadAccess = true;
		acl.PublicWriteAccess = false;

		var account = new ParseObjectKidAccount{
			IsLinkedToParentAccount = false,
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

