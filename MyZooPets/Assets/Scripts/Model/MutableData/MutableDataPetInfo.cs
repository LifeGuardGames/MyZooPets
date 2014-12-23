using UnityEngine;
using System;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;

public class MutableDataPetInfo : MutableData{	
	public string PetID { get; set; }

	/// <summary>
	/// Gets or sets the name of the pet. **NOTE For json serialization use only. Use 
	/// ChangeName() to edit pet name otherwise data will not be save to the server
	/// </summary>
	/// <value>The name of the pet.</value>
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

	/// <summary>
	/// Changes the name.
	/// </summary>
	/// <param name="petName">Pet name.</param>
	public void ChangeName(string petName){
		IsDirty = true;
		if(!string.IsNullOrEmpty(petName)){
			PetName = petName;
		}
	}

	public void ChangeColor(PetColor petColorEnum){
		// Sould we do sanity color checking here?
		if(true){
			IsDirty = true;
			PetColor = petColorEnum.ToString();
		}
	}
	
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
	}

	public override void VersionCheck(Version currentDataVersion){
		Version version131 = new Version("1.3.1");
		
		if(currentDataVersion < version131){
			FireBreaths = nFireBreaths;
		}
	}

	public override void SaveAsyncToParseServer(){
		//make the query that will get the kid account and eager load the pet accessory
		ParseQuery<ParseObjectKidAccount> query = new ParseQuery<ParseObjectKidAccount>()
			.WhereEqualTo("createdBy", ParseUser.CurrentUser)
			.Include("petInfo");

		query.FirstAsync().ContinueWith(t => {

			ParseObjectKidAccount fetchedAccount = t.Result;
			ParseObjectPetInfo petInfo = fetchedAccount.PetInfo;
			List<ParseObject> objectsToSave = new List<ParseObject>();

			if(petInfo == null){
				petInfo = new ParseObjectPetInfo();
				ParseACL acl = new ParseACL();
				acl.PublicReadAccess = true;
				acl.PublicWriteAccess = false;
				acl.SetWriteAccess(ParseUser.CurrentUser, true);
				petInfo.ACL = acl;
				
				fetchedAccount.PetInfo = petInfo;
				objectsToSave.Add(fetchedAccount);
			}

			petInfo.Name = PetName;
			petInfo.Color = PetColor;
			petInfo.Species = PetSpecies;

			objectsToSave.Add(petInfo);
			
			return ParseObject.SaveAllAsync(objectsToSave);
		}).Unwrap().ContinueWith(t => {
			if(t.IsFaulted || t.IsCanceled){
				foreach(ParseException e in t.Exception.InnerExceptions)
					Debug.Log("Message: " + e.Message + ", Code: " + e.Code);

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
