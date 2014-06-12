﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetManager : Singleton<MiniPetManager> {
	private Level maxLevel = Level.Level6;
	// Use this for initialization
	void Start(){

		GatingManager.OnDestroyedGate += OnDestroyedGateHandler;
		//iterate through the MiniPetProgress
		//if minipet not in MiniPetProgress then it's not unlock yet
		//if in MiniPetProgress spawn the appropriate mini pet
		Dictionary<string, MutableDataMiniPets.Status> miniPetProgress = 
			DataManager.Instance.GameData.MiniPets.MiniPetProgress;

		foreach(KeyValuePair<string, MutableDataMiniPets.Status> progress in miniPetProgress){
			string miniPetID = progress.Key;
			MutableDataMiniPets.Status miniPetStatus = progress.Value;

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

	public bool CanModifyFoodXP(string miniPetID){
		//make sure current level is not at max level
		bool canModify = true;
		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);

		if(currentLevel == maxLevel)
			canModify = false;

		return canModify;
	}


	public void IncreaseFoodXP(string miniPetID){
		//check if level meter meets next lv condition
		//if it does play level up animations and spew out rewards

		//Increase food xp                                                                         
		DataManager.Instance.GameData.MiniPets.IncreaseFoodXP(miniPetID, 1);

		//get immutabledata
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		ImmutableDataMiniPetLevelUpConditions levelUpConditionData = 
			DataLoaderLevelUpConditions.GetData(data.LevelUpConditionID);

		//get current level
		Level currentLevel = DataManager.Instance.GameData.MiniPets.GetCurrentLevel(miniPetID);

		//get next level
		Level nextLevel = GetNextLevel(currentLevel);

		//get next level up condition
		int levelUpCondition = levelUpConditionData.GetLevelUpCondition(nextLevel);

		//get current food xp
		int currentFoodXP = DataManager.Instance.GameData.MiniPets.GetCurrentFoodXP(miniPetID);

		//check current food xp with that condition
		if(currentFoodXP >= levelUpCondition){
			Debug.Log("pet level up!!!!"); 
			DataManager.Instance.GameData.MiniPets.IncreaseCurrentLevel(miniPetID);
		}
			

		/*
		 * Few options here for level up. Can just return boolean to signify if user level up
		 * after IncreaseFoodXP is called. or send out an event from the manager. Sending out 
		 * an level up event will be usefull if other classes besides MiniPet.cs needs to know
		 * about mini pet level up as well otherwise returning boolean is the more straight forward
		 * way.
		 */

		/*
		 * As for reward just spawn 1 gem and randomize coin numbers for now. dont' think we need
		 * a xml for that yet
		 */
	}

	private void OnDestroyedGateHandler(object sender, DestroyedGateEventArgs args){
		string gateID = args.DestroyedGateID;
		string miniPetID = args.MiniPetID;

		//when a gate is destroyed load the proper minipet and spawned it
		ImmutableDataMiniPet data = DataLoaderMiniPet.GetData(miniPetID);
		GameObject prefab = Resources.Load(data.PrefabName) as GameObject;
		GameObject goMiniPet = Instantiate(prefab, data.SpawnLocation, Quaternion.identity) as GameObject;
		goMiniPet.name = prefab.name;
		goMiniPet.GetComponent<MiniPet>().Init(data);

		//unlock in data manager
		DataManager.Instance.GameData.MiniPets.UnlockMiniPet(miniPetID);
	}

	private Level GetNextLevel(Level currentLevel){
		int currentLevelNum = (int) currentLevel;
		currentLevelNum++;
		
		return (Level) currentLevelNum;
	}
}
