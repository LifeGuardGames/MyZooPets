using UnityEngine;
using System;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;

public class MutableDataPetInfo : MutableData{	
	public string PetID { get; set; }
	public string PetName { get; set; }
	public string PetSpecies { get; set; }
	public string PetColor { get; set; }
	public bool IsHatched { get; set; }
	public int FireBreaths { get; set; } // fire breathing status of the pet

	/// <summary>
	/// DEPRECATED in v1.3.1 don't use this.
	/// </summary>
	/// <value>The n fire breaths.</value>
	public int nFireBreaths { get; set; } // Deprecated in 1.3.1
	public bool IsQuestionaireCollected {get; set;}
	public string ParseKidAccountID {get; set;}
	
	public void SetFireBreaths(int amount){
		FireBreaths = amount;	
	
		// for now, we are capping the max breaths at 1
		bool isInfiniteMode = IsInfiniteFire();
		if(FireBreaths > 1)
			FireBreaths = 1;
		else if(FireBreaths < 0 && !isInfiniteMode){
			Debug.LogError("Fire breaths somehow going negative.");
			FireBreaths = 0;
		}
	}

	public bool CanBreathFire(){
		int breaths = FireBreaths;
		bool isInfiniteFire = IsInfiniteFire();
		bool canBreathFire = breaths > 0 || isInfiniteFire;
		return canBreathFire;
	}

	public bool IsInfiniteFire(){
		bool isInfinite = Constants.GetConstant<bool>("InfiniteFireMode");
		return isInfinite;
	}

	#region Initialization and override functions
	public MutableDataPetInfo(){
		Init();        
	}

	private void Init(){
		PetID = "";
		PetName = "";
		PetSpecies = "Basic";
		PetColor = "OrangeYellow";
		IsHatched = false;
		FireBreaths = 0;
		IsQuestionaireCollected = false;
		ParseKidAccountID = "";
	}

	public override void VersionCheck(Version currentDataVersion){
		Version version131 = new Version("1.3.1");
		
		if(currentDataVersion < version131){
			FireBreaths = nFireBreaths;
		}
	}

	public override void SaveAsyncToParseServer(string kidAccountID){
//		ExtraParseLogic.Instance.UserAndKidAccountCheck().ContinueWith(t => {
//			KidAccount kidAccount = t.Result;
//			
//			//make the query that will get the kid account and eager load the pet accessory
			ParseQuery<KidAccount> query = new ParseQuery<KidAccount>()
				.Include("petInfo");
//				.WhereEqualTo("objectId", kidAccount.ObjectId);
//			
//			return query.GetAsync(kidAccount.ObjectId);
//		}).Unwrap()

//		string kidAccountID = DataManager.Instance.GameData.PetInfo.ParseKidAccountID;

		query.GetAsync(kidAccountID).ContinueWith(t => {

			KidAccount fetchedAccount = t.Result;
			ParseObject petInfo = fetchedAccount.PetInfo;
			List<ParseObject> objectsToSave = new List<ParseObject>();

			if(petInfo == null){
				petInfo = new ParseObject("PetInfo");
				ParseACL acl = new ParseACL();
				acl.PublicReadAccess = true;
				acl.PublicWriteAccess = false;
				acl.SetWriteAccess(ParseUser.CurrentUser, true);
				petInfo.ACL = acl;
				
				fetchedAccount.PetInfo = petInfo;
				objectsToSave.Add(fetchedAccount);
			}

			petInfo["id"] = PetID;
			petInfo["name"] = PetName;
			petInfo["color"] = PetColor;
			petInfo["species"] = PetSpecies;

			objectsToSave.Add(petInfo);
			
			return ParseObject.SaveAllAsync(objectsToSave);
		}).Unwrap().ContinueWith(t => {
			if(t.IsFaulted || t.IsCanceled){
				Debug.Log("Fail to save async: " + this.ToString());
			}
			else{
				Loom.DispatchToMainThread(() =>{
					IsDirty = false;
					Debug.Log("save async successful: " + this.ToString());
					Debug.Log("is data dirty: " + IsDirty);
				});
			}
		});
	}
	#endregion
}