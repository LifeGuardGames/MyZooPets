using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using Parse;

public class ExtraParseLogic : Singleton<ExtraParseLogic>{

	// Use this for initialization
	void Start(){
	
	}
	
	// Update is called once per frame
	void Update(){
	
	}

	void OnGUI(){
		if(GUI.Button(new Rect(0, 0, 100, 50), "create user + kid account")){
			//			DataManager.Instance.AddNewMenuSceneData();
			//			RefreshUI();
			LoginAndCreateKidAccount();
		}
		
		if(GUI.Button(new Rect(100, 0, 100, 50), "new account")){
			//			DataManager.Instance.CreateNewAccount();
		}
		
	}

	public bool UserAndAccountCheck(){
		bool isUserAndAccountValid = false;
		var user = ParseUser.CurrentUser;

		//user not login yet
		if(user == null){
			//sign up

			// or login

		}
		//user login and valid
		else{
			//check if kid account exist

		}
	}

	public void LoginAndCreateKidAccount(){
		CreateParseNewUser().ContinueWith(t => {
			return CreateNewAccount();
		}).Unwrap().ContinueWith(t => {
			if(t.IsFaulted || t.IsCanceled){
				foreach(ParseException e in t.Exception.InnerExceptions){
					Debug.Log(e.Message);
					Debug.Log(e.Code);
				}
			}
			else{
				Debug.Log("sign up and kid account creation successful");
			}
		});
	}

	public void TestUser(){
		CreateParseNewUser().ContinueWith(t => {
			if(t.IsFaulted || t.IsCanceled){
				foreach(ParseException e in t.Exception.InnerExceptions){
					Debug.Log(e.Message);
					Debug.Log(e.Code);
				}
			}
			else{
				Debug.Log("sign up successful");
			}
		});
	}
	
	public Task CreateParseNez
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	wUser(){
		string guid = Guid.NewGuid().ToString();
		
		var user = new ParseUser(){
			Username = guid,
			Password = guid
		};
		
		user["accountType"] = "kid";
		
		return user.SignUpAsync();
		//		user.SignUpAsync().ContinueWith(t => {
		//			if(t.IsFaulted || t.IsCanceled){
		//				foreach(ParseException e in t.Exception.InnerExceptions){
		//					Debug.Log(e.Message);
		//					Debug.Log(e.Code);
		//				}
		//			}
		//			else{
		//				Debug.Log("sign up successful");
		//			}
		//		});
	}
	
	public Task CreateNewAccount(){
		var user = ParseUser.CurrentUser;
//		if(user != null){
			var account = new ParseObject("Account");
			account["isLinkedWithParent"] = false;
			account.ACL = new ParseACL(user);
			user.AddUniqueToList("localAccountList", account);

			return account.SaveAsync();
//			account.SaveAsync().ContinueWith(t => {
//				if(t.IsFaulted || t.IsCanceled){
//					foreach(ParseException e in t.Exception.InnerExceptions){
//						Debug.Log(e.Message);
//						Debug.Log(e.Code);
//					}
//				}
//				else{
//					//save account.objectId so can be reference later
//					Debug.Log("account created");
//				}
//			});
//		}
	}
}

