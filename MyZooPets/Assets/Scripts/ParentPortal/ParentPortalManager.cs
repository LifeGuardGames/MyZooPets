using UnityEngine;
using System;
using Parse;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ParentPortalManager : Singleton<ParentPortalManager> {
	public static EventHandler<ServerEventArgs> OnDataRefreshed;
	public bool useDummyData = false;
	
	/// <summary>
	/// Gets or sets the kid account list.
	/// This list contains KidAccounts that belong to the current Parse User
	/// </summary>
	/// <value>The kid account list.</value>
	public ParseObjectKidAccount KidAccount {get; set;}

	public void RefreshData(){
		if(IsUsingDummyData()) return;

		try{
			ExtraParseLogic.Instance.UserCheck().ContinueWith(t => {
				var parentPortalQuery = new ParseQuery<ParseObjectKidAccount>()
					.WhereEqualTo("createdBy", ParseUser.CurrentUser);

				return parentPortalQuery.FirstAsync();
			}).Unwrap().ContinueWith(t => {
				if(t.IsFaulted || t.IsCanceled){
					ParseException exception = (ParseException)t.Exception.InnerExceptions[0];
					Debug.Log("Message: " + exception.Message + ", Code: " + exception.Code);

					Loom.DispatchToMainThread(() => {
						ServerEventArgs args = new ServerEventArgs();
						args.IsSuccessful = false;
						args.ErrorCode = exception.Code;
						args.ErrorMessage = exception.Message;
						
						if(OnDataRefreshed != null)
							OnDataRefreshed(this, args);
					});
				}
				else{
					ParseObjectKidAccount kidAccount = t.Result;
					KidAccount = kidAccount;

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
