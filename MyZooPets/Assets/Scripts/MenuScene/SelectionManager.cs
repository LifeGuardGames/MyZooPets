using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : Singleton<SelectionManager> {

    public MutableDataPetInfo PetMenuInfo{
        get{ return DataManager.Instance.GameData.PetInfo;}
    }

	public bool IsFirstTime{
		get{ return DataManager.Instance.IsFirstTime;}
	}
}
