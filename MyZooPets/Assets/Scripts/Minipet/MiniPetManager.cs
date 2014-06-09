using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetManager : MonoBehaviour {
	
	// Use this for initialization
	void Start(){

		GatingManager.OnDestroyedGate += OnDestroyedGateHandler;
		//iterate through the MiniPetProgress
		//if minipet not in MiniPetProgress then it's not unlock yet
		//if in MiniPetProgress spawn the appropriate mini pet
		Dictionary<string, MutableDataMiniPetStatus> miniPetProgress = 
			DataManager.Instance.GameData.MiniPets.MiniPetProgress;

		foreach(KeyValuePair<string, MutableDataMiniPetStatus> progress in miniPetProgress){
			string miniPetID = progress.Key;
			MutableDataMiniPetStatus miniPetStatus = progress.Value;

			//use the id to get the immutable data
			ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
			GameObject prefab = Resources.Load(data.PrefabName) as GameObject;
			GameObject goMiniPet = Instantiate(prefab, data.SpawnLocation, Quaternion.identity) as GameObject;
		}

		//listen to Gate unlock from GatingManager
	}

	void OnDestroy(){
		GatingManager.OnDestroyedGate -= OnDestroyedGateHandler;
	}

	private void OnDestroyedGateHandler(object sender, DestroyedGateEventArgs args){
		string gateID = args.DestroyedGateID;
		string miniPetID = args.MiniPetID;

		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		GameObject prefab = Resources.Load(data.PrefabName) as GameObject;
		GameObject goMiniPet = Instantiate(prefab, data.SpawnLocation, Quaternion.identity) as GameObject;
		goMiniPet.name = prefab.name;

		//unlock in data manager
		DataManager.Instance.GameData.MiniPets.UnlockMiniPet(miniPetID);
	}

}
