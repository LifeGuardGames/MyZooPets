using UnityEngine;
using System.Collections;

public class FireCrystalManager : Singleton<FireCrystalManager> {

	public void RewardShards(int numberOfShards){
		// Data saving
		DataManager.Instance.GameData.Stats.AddShard(numberOfShards);

		if(numberOfShards > 0){
			// Queue up the reward in RewardManager
			RewardQueueData.GenericDelegate function2 = delegate{
				FireCrystalUIManager.Instance.PopupAndRewardShards(numberOfShards);
			};
			RewardManager.Instance.AddToRewardQueue(function2);
		}
	}
}
