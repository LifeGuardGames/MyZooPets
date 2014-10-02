using UnityEngine;
using System;
using Parse;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ParentPortalManager : Singleton<ParentPortalManager> {
	public static EventHandler<ServerEventArgs> OnDataRefreshed;
	public static EventHandler<ServerEventArgs> OnAccountCreated;
//	public static EventHandler<ServerEventArgs> OnAccountDeleted;
	
	/// <summary>
	/// Gets or sets the kid account list.
	/// This list contains KidAccounts that belong to the current Parse User
	/// </summary>
	/// <value>The kid account list.</value>
	public List<ParseObjectKidAccount> KidAccountList {get; set;}

	public void RemoveAccount(string petID){
		MutableDataPetMenuInfo petMenuInfo = DataManager.Instance.GetMenuSceneData(petID);
		if(petMenuInfo != null){
			string kidAccountID = petMenuInfo.ParseKidAccountID;

			if(string.IsNullOrEmpty(kidAccountID)){
				//remove locally
				DataManager.Instance.RemovePet(petID);
			}
			//need to remove from backend before removing locally
			else{
				IDictionary<string, object> paramDict = new Dictionary<string, object>{
					{"kidAccountId", kidAccountID}
				};

				//call the cloud function
				ParseCloud.CallFunctionAsync<IDictionary<string, object>>("deleteKidAccount", paramDict)
					.ContinueWith(t => {
						//connection faulted
						if(t.IsFaulted){
							ParseException e = (ParseException) t.Exception.InnerExceptions[0];
							Debug.Log("Message: " + e.Message + ", Code: " + e.Code);
							
							ServerEventArgs args = new ServerEventArgs();
							args.IsSuccessful = false;
							args.ErrorCode = ParseException.ErrorCode.ConnectionFailed;
							
							Loom.DispatchToMainThread(() => {
								if(OnDataRefreshed != null)
									OnDataRefreshed(this, args);
							});
						} 
						else{
							IDictionary<string, object> result = t.Result;
							// Hack, check for errors
							object code;
							ServerEventArgs args = new ServerEventArgs();

							//error happened
							if(result.TryGetValue("code", out code)){
								int parseCode = Convert.ToInt32(code);
								
								args.IsSuccessful = false;
								args.ErrorCode = (ParseException.ErrorCode) parseCode;
								args.ErrorMessage = (string) result["message"];
							} 
							//success
							else{
								Debug.Log("Result: " + result["success"]);
								args.IsSuccessful = true;
								
								Loom.DispatchToMainThread(() => {
									DataManager.Instance.RemovePet(petID);
									RefreshData();
								});
							}
						}
					});
			}
		}
	}
			
	public void AddNewAccount(){
		bool isMaxPet = DataManager.Instance.IsMaxNumOfPet;
		if(!isMaxPet){
			DataManager.Instance.AddNewPet();

			SyncKidAccountToParse();
		}
	}

	private void SyncKidAccountToParse(){
		ExtraParseLogic.Instance.UserAndKidAccountCheck().ContinueWith(t => {
			if(t.IsFaulted || t.IsCanceled){
				foreach(ParseException e in t.Exception.InnerExceptions)
					Debug.Log("Message: " + e.Message + ", Code: " + e.Code);
				
				Loom.DispatchToMainThread(() => {
					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = false;
					args.ErrorCode = ParseException.ErrorCode.ConnectionFailed;
					args.ErrorMessage = "Failed to creat user. Try again later";
					
					if(OnAccountCreated != null)
						OnAccountCreated(this, args);
				});
			}
			else{
				Loom.DispatchToMainThread(() => {
					DataManager.Instance.GameData.SaveAsyncToParse();

					ServerEventArgs args = new ServerEventArgs();
					args.IsSuccessful = true;
					
					if(OnAccountCreated != null)
						OnAccountCreated(this, args);
				});
			}
		});
	}
		
	public void RefreshData(){
		var parentPortalQuery = new ParseQuery<ParseObjectKidAccount>()
			.WhereEqualTo("createdBy", ParseUser.CurrentUser)
			.WhereEqualTo("isDeleted", false)
			.Include("petInfo");

		try{
			ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
				return parentPortalQuery.FindAsync();
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					foreach(ParseException e in t.Exception.InnerExceptions)
						Debug.Log("Message: " + e.Message + ", Code: " + e.Code);

					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = false;
						args.ErrorCode = ParseException.ErrorCode.ConnectionFailed;
						
						if(OnDataRefreshed != null)
							OnDataRefreshed(this, args);
					});
				}
				else{
					IEnumerable<ParseObjectKidAccount> kidAccounts = t.Result;
					KidAccountList = kidAccounts.ToList();

					Debug.Log(KidAccountList.Count());

					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = true;
						
						if(OnDataRefreshed != null)
							OnDataRefreshed(this, args);
					});

				}
			});
		}
		catch(InvalidOperationException e){
			//Errors from parse SDK logic check
			Debug.LogException(e);

			ServerEventArgs args = new ServerEventArgs();
			args.IsSuccessful = false;
			args.ErrorCode = ParseException.ErrorCode.OtherCause;
			
			if(OnDataRefreshed != null)
				OnDataRefreshed(this, args);
		}
	}
}
