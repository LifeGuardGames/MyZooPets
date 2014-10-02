using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : Singleton<SelectionManager> {

    //Return PetMenuInfo serialized data
    public MutableDataPetMenuInfo PetMenuInfo{
        get{ return DataManager.Instance.MenuSceneData;}
    }

    //Has a game data been loaded into DataManager. Game is only safe to 
    //start if the data is there
    public bool IsGameDataLoaded{
        get{ return DataManager.Instance.IsGameDataLoaded;}
    }

	public bool IsFirstTime{
		get{ return DataManager.Instance.IsFirstTime;}
	}

    public void LoadPetGameData(string petID){
        DataManager.Instance.LoadGameData(petID);
    }
}
