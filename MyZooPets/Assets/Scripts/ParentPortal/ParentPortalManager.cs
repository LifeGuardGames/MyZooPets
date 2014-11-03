using UnityEngine;
using System;
using Parse;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ParentPortalManager : ServerConnector<ParentPortalManager> {
	public static EventHandler<ServerEventArgs> OnDataRefreshed;
	
	/// <summary>
	/// Gets or sets the kid account list.
	/// This list contains KidAccounts that belong to the current Parse User
	/// </summary>
	/// <value>The kid account list.</value>
	public ParseObjectKidAccount KidAccount {get; set;}

	public void RefreshData(){
		if(IsUsingDummyData()) return;

		ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
			StartTimeOutTimer();

			var parentPortalQuery = new ParseQuery<ParseObjectKidAccount>()
				.WhereEqualTo("createdBy", ParseUser.CurrentUser);

			return parentPortalQuery.FirstAsync(timeOutRequestCancellation.Token);
		}).Unwrap().ContinueWith(t => {
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
				
				args.IsSuccessful = false;
				args.ErrorCode = code;
				args.ErrorMessage = message;
			}
			else{
				ParseObjectKidAccount kidAccount = t.Result;
				KidAccount = kidAccount;
				args.IsSuccessful = true;
			}

			Loom.DispatchToMainThread(() => {
				if(OnDataRefreshed != null)
					OnDataRefreshed(this, args);
			});
			
			StopTimeOutTimer();
		});
	}
	
	private bool IsUsingDummyData(){
		bool retVal = false; 
		if(useDummyData){
			retVal = useDummyData;

			KidAccount = new ParseObjectKidAccount();
			KidAccount.AccountCode = "testingdummy";
			
			ServerEventArgs args = new ServerEventArgs();
			args.IsSuccessful = true;
			
			if(OnDataRefreshed != null)
				OnDataRefreshed(this, args);
		}

		return retVal;
	}
}
