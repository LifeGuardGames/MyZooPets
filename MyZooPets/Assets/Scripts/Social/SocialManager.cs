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
}

public class SocialManager : Singleton<SocialManager> {
//	public static EventHandler<EventArgs> OnDataLoadSucessful;
//	public static EventHandler<EventArgs> OnDataLoadFailed;

	public static EventHandler<ServerEventArgs> OnDataRefreshed;

#if UNITY_EDITOR
//	void OnGUI(){
//		GUILayout.BeginHorizontal();
//		if(GUILayout.Button("add friend code")){
//			AddFriendCode("bEydV3HOUU");
//		}
//
//		if(GUILayout.Button("refresh friend list")){
////			StartCoroutine(RefreshData());
//			RefreshData();
//		}
//		
//		GUILayout.EndHorizontal();
//	}
#endif
	
	public void RefreshData(){
		try{

			ExtraParseLogic.Instance.UserAndKidAccountCheck().ContinueWith(t => {
				KidAccount kidAccount = t.Result;

				ParseQuery<KidAccount> friendListQuery = new ParseQuery<KidAccount>();

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

					if(account.FriendList != null)
						Debug.Log(account.FriendList.Count);

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
			Debug.Log(e.Message);


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
					
					// OtherCause code = connection error
				} 
				else{
					IDictionary<string, object> result = t.Result;
					// Hack, check for errors
					object code;
					if(result.TryGetValue("code", out code)){
						Debug.Log("Error Code: " + code);
						//101 -> object not found
						//137 -> dupliate value
					} 
					else{
						Debug.Log("Result: " + result["success"]);
					}
				}
			});
		}
	}
}
