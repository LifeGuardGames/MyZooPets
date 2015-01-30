using UnityEngine;
using System.Collections;

public class FireCrystalManager : Singleton<FireCrystalManager> {

	public void RewardShards(int numberOfShards){
		// Data saving and syncing right here
		FireCrystalUIManager.Instance.PopupAndRewardShards(numberOfShards);
	}
}
