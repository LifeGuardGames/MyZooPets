using UnityEngine;
using System;
using System.Linq;
using System.Collections;
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
		ParseObject.RegisterSubclass<KidAccount>();
	}

	// Use this for initialization
	void Start(){
		//start a loom single thread here so we can use DispatchToMainThread
		//in one nof Parse's worker thread
		Loom.StartSingleThread(() => {});
	}

	/// <summary>
	/// Check if ParseUser exists. If not create one
	/// </summary>
	/// <returns>Task</returns>
	public Task UserCheck(){
		var user = ParseUser.CurrentUser;
		var source = new TaskCompletionSource<string>();

		if(user == null){
			CreateParseNewUser().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					source.SetException(t.Exception);
				}
				else{
					source.SetResult("User Valid");
				}
			});
		}
		else{
			source.SetResult("User Valid");
		}

		return source.Task;
	}
	
	/// <summary>
	/// Check if Parse User is login and check if kid account exist.
	/// Call this method everytime before you try to any data to the backend.
	/// 
	/// Both ParseUser and KidAccount will be created if they don't already exist
	/// </summary>
	/// <returns>Task with current KidAccount object</returns>
	public Task<KidAccount> UserAndKidAccountCheck(){
		var user = ParseUser.CurrentUser;
		var source = new TaskCompletionSource<KidAccount>();

		//user not login yet
		if(user == null){
			//sign up and create kid account
			CreateParseNewUser().ContinueWith(t => {
				return CreateNewKidAccount();
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					source.SetException(t.Exception);
				}
				else{
					KidAccount account = ParseObject.CreateWithoutData<KidAccount>(t.Result);
					source.SetResult(account);
				}
			});
			// or login .... will implement later

		}
		//user login and valid
		else{
			//check if kid account exist
			bool isKidAccountValid = false;
			string kidAccountID = "";
			Loom.DispatchToMainThread(() => {
				kidAccountID = DataManager.Instance.GameData.PetInfo.ParseKidAccountID;

				isKidAccountValid = !string.IsNullOrEmpty(kidAccountID);
			});

			if(isKidAccountValid){
				KidAccount account = ParseObject.CreateWithoutData<KidAccount>(kidAccountID);
				source.SetResult(account);
			}
			// kid account doesn't exist so create one
			else{
				CreateNewKidAccount().ContinueWith(t => {
					if(t.IsFaulted || t.IsCanceled){
						source.SetException(t.Exception);
					}
					else{
						KidAccount account = ParseObject.CreateWithoutData<KidAccount>(t.Result);
						source.SetResult(account);
					}
				});
			}
		}

		return source.Task;
	}

	/// <summary>
	/// Creates the parse new user.
	/// </summary>
	/// <returns>Task.</returns>
	public Task CreateParseNewUser(){
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
	public Task<string> CreateNewKidAccount(){
		var source = new TaskCompletionSource<string>();
		var user = ParseUser.CurrentUser;
		ParseACL acl = new ParseACL(user);
		acl.PublicReadAccess = true;
		acl.PublicWriteAccess = false;

		var account = new KidAccount{
			IsLinkedToParentAccount = false,
			CreatedBy = user,
			ACL = acl
		};

		account.SaveAsync().ContinueWith(t => {
			if(t.IsFaulted || t.IsCanceled){
				source.SetException(t.Exception);
			}
			else{
				Loom.DispatchToMainThread(() =>{
					DataManager.Instance.GameData.PetInfo.ParseKidAccountID = account.ObjectId;
				});
				source.SetResult(account.ObjectId);
			}
		});

		return source.Task;
	}
}

