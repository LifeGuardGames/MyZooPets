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
	
	/// <summary>
	/// Gets or sets the kid account list.
	/// This list contains KidAccounts that belong to the current Parse User
	/// </summary>
	/// <value>The kid account list.</value>
	public List<ParseObjectKidAccount> KidAccountList {get; set;}


	public void AddNewAccount(){
		bool isMaxPet = DataManager.Instance.IsMaxNumOfPet;
		if(!isMaxPet){
			//create menu scene data here
			string petID = "Pet" + DataManager.Instance.NumOfPets;
			DataManager.Instance.AddNewMenuSceneData();
			DataManager.Instance.InitializeGameDataForNewPet(petID: petID);

			//save the new game data right after it's created so we don't risk
			//losing it if the user decide to create another one right away.
			DataManager.Instance.SaveGameData();

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
