using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : Singleton<SelectionManager> {

//    public string CurrentPetID{
//        get{
//            return DataManager.Instance.CurrentPetID;
//        }
//        set{
//            DataManager.Instance.CurrentPetID = value;
//        }
//    }

//    public int NumOfPets{
//        get{
//            return DataManager.Instance.NumOfPets;
//        }
//    }

    //Return PetMenuInfo serialized data
    public Dictionary<string, MutableDataPetMenuInfo> PetMenuInfo{
        get{
            return DataManager.Instance.MenuSceneData;
        }
    }

    //Has a game data been loaded into DataManager. Game is only safe to 
    //start if the data is there
    public bool IsGameDataLoaded{
        get{
            return DataManager.Instance.IsGameDataLoaded;
        }
    }

	public bool IsFirstTime{
		get{
			return DataManager.Instance.IsFirstTime;
		}
	}

    public void LoadPetGameData(string petID){
        DataManager.Instance.LoadGameData(petID);
    }



//    public void RemovePetData(string petID){
//        LgNotificationServices.RemoveIconBadgeNumber(); //make sure no notification exists if pet data is removed
//        DataManager.Instance.RemovePetData(petID);
//    }

}
