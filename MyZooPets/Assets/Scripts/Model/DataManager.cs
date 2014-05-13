using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using fastJSON;

//This class handles all game data. No game logic
//Saves and loads data into player preference
public class DataManager : Singleton<DataManager>{
	public class SerializerEventArgs : EventArgs{
		public bool IsSuccessful { get; set; }
	}

	public event EventHandler<SerializerEventArgs> OnGameDataLoaded;
	public event EventHandler<EventArgs> OnGameDataSaved;

	public bool isDebug = false; //turn isDebug to true if working on independent scene
//	public string currentPetID; //The id that will be used for pet serialization

//	private const int NUM_OF_PETS = 3;
	private static bool isCreated;
	private PetGameData gameData; //Super class that stores all the game data related to a specific petID
	private MutableDataPetMenuInfo menuSceneData; //basic info data of all the pet that are only used in this scene

	//Return menu scene data used in MenuScene
	public MutableDataPetMenuInfo MenuSceneData{
		get{ return menuSceneData; }
	}

	//Return the current game datda
	public PetGameData GameData{
		get{ return gameData; }
	}

//	public string CurrentPetID{
//		get{ return currentPetID;}
//		set{ currentPetID = value; }
//	}

	public int NumOfPets{
        //do a count form menuSceneData
		get{ return NUM_OF_PETS; }
	}
    
	//Save temporary data when transitioning to new scene
	public LoadSceneData SceneData{ get; set; } 

	//Use to tell if it's users' first time launching the app
	public bool IsFirstTime{
		get{
			bool firstTime = PlayerPrefs.GetInt("IsFirstTime", 1) > 0;

			//IsFirstTime is implemented in later versions so this chunk of code
			//is used to single out all the existing users prior to the implementation
			//of IsFirstTime 
			if(firstTime){
//				for(int petIndex=0; petIndex < NumOfPets; petIndex++){
//					string jsonSavedData = PlayerPrefs.GetString("Pet" + petIndex + "_GameData", "");
//
//					if(!String.IsNullOrEmpty(jsonSavedData)){
//						firstTime = false;
//						break;
//					}
//				}

				//not first time anymore after this property is called
				PlayerPrefs.SetInt("IsFirstTime", 0);
			}


			return firstTime;
		}
	}

	void Awake(){
		//JSON serializer setting
		JSON.Instance.Parameters.UseExtensions = false;
		JSON.Instance.Parameters.UseUTCDateTime = false; //turning utc off for now
		JSON.Instance.Parameters.UseOptimizedDatasetSchema = true;
		
		//--------------------Make Object persistent---------------------------
		if(isCreated){
			//If There is a duplicate in the scene. delete the object and jump Awake
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		isCreated = true;
		//---------------------------------------------------------------------



		//Use this when developing on an independent scene. Will initialize all the data
		//before other classes call DataManager
		if(isDebug){
			InitializeGameDataForNewPet();
		}else{
			ModifyDataIntoSinglePetMode();
		}

		LoadMenuSceneData();
	}

	//----------------------------------------------------
	//This function gets called before the script variables are ready so don't try
	//to use class variables here
	//----------------------------------------------------
	void OnLevelWasLoaded(){

		/*
            Game data should be serialized whenever user return to MenuScene because
            from the MenuScene user can switch to a different pet, so the previous game
            data need to be saved
        */
//		if(Application.loadedLevelName == "MenuScene"){
//			PlayerPrefs.SetString("CanSaveData", "Yes");
//		}
	}

	//----------------------------------------------------
	//Since DateManager is persistent, update is the only method that we can use
	//to check if user data need to be saved right away when user comes back
	//to menu scene
	//----------------------------------------------------
	void Update(){
//		string loadedLevelName = Application.loadedLevelName;

//		if(loadedLevelName == "MenuScene"){
//			string canSaveData = PlayerPrefs.GetString("CanSaveData", "");
//
//			//Serialize current pet data when loading into MenuScene
//			if(canSaveData == "Yes"){
//				//check there is actually data to be saved before saving
//				if(!String.IsNullOrEmpty(currentPetID) && gameData != null){
//					SaveGameData();
//					PlayerPrefs.SetString("CanSaveData", "No");
//				}
//			}
//		}
	}

	//Serialize the data whenever the game is paused
	void OnApplicationPause(bool paused){
		if(paused){
			//Save menu scene data. doesn't depend on if tutorial is finished or not
			SaveMenuSceneData();

			// check immediately if a tutorial is playing...if one is, we don't want to save the game on pause
			if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()){
				Debug.Log("Auto save canceled because we are in a tutorial");
				return;
			}
			
			// also early out if we happen to be in the inhaler game.  Ultimately we may want to create a more elaborate hash/list
			// of scenes it is okay to save in, if we ever create more scenes that shouldn't serialize data
			string loadedLevelName = Application.loadedLevelName;
			if(loadedLevelName == "InhalerGamePet"){
				Debug.Log("Not saving the game because its inhaler scene");
				return;
			}

			//game can be paused at anytime and sometimes MenuScene doesn't have
			//any thing that needs saving, so check before saving
			if(gameData != null){
				// special case: when we are about to serialize the game, we have to cache the moment it happens so we know when the user stopped
				DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame = LgDateTime.GetTimeNow();
                
				SaveGameData();
			}

		}
	}

//	//----------------------------------------------------
//	// RemovePetData()
//	// delete json data
//	//----------------------------------------------------
//	public void RemovePetData(string petID){
//		if(!String.IsNullOrEmpty(petID)){
//			menuSceneData.Remove(petID);
//			gameData = null;
//			PlayerPrefs.DeleteKey(petID + "_GameData");
//		}
//	}

	//----------------------------------------------------
	// IsGameDataLoaded()
	// Use this to check if there is data loaded into gameData at anypoint
	// in the menuscene
	//----------------------------------------------------
	public bool IsGameDataLoaded(){
		return gameData != null;
	}

	//Initalize New PetGameData
	public void InitializeGameDataForNewPet(string petID = "Pet0", string petName = "", 
	                                        string petSpecies = "Basic", string petColor = "OrangeYellow"){

//		if(!String.IsNullOrEmpty(currentPetID)){

			gameData = new PetGameData();
			if(!String.IsNullOrEmpty(petName))
				gameData.PetInfo.PetName = petName;
			gameData.PetInfo.PetColor = petColor;
			gameData.PetInfo.IsHatched = true;
			gameData.PetInfo.PetID = petID;
           
			if(!isDebug){
				MutableDataPetMenuInfo petMenuInfo = 
                    new MutableDataPetMenuInfo(gameData.PetInfo.PetName, petColor, petSpecies);
				menuSceneData.Add(petID, petMenuInfo);
			}

//		}
//		else{
//			Debug.LogError("PetID is null or empty. Can't initialize pet with ID. Check  currentPetID");
//		}
	}

	/// <summary>
	/// Modifies the data into single pet mode.
	/// v1.2.7
	/// </summary>
	private void ModifyDataIntoSinglePetMode(){
		//check if game is already in single mode. default to false;
		bool isSinglePetMode = PlayerPrefs.GetInt("IsSinglePetMode", 0) > 0;
		//used to have 3 pets, but switching to one pet

		//return if it's already in single pet mode
		if(isSinglePetMode) return;

		string petIDToKeep = ModifyGameDataForSingleMode();

		ModifyMenuSceneDataForSingleMode(petIDToKeep);

		//Set single mode to true
		PlayerPrefs.SetInt("IsSinglePetMode", 1);
	}

	/// <summary>
	/// Modifies the game data for single mode.
	/// v1.2.7
	/// </summary>
	/// <returns>petID that will be kept.</returns>
	private string ModifyGameDataForSingleMode(){
		//deserialize all data. 
		List<PetGameData> existingData = new List<PetGameData>();
		
		for(int index=0; index < 3; index++){
			string jsonString = PlayerPrefs.GetString("Pet" + index + "_GameData", "");
			
			if(!String.IsNullOrEmpty(jsonString)){
				PetGameData newGameData = JSON.Instance.ToObject<PetGameData>(jsonString);
				existingData.Add(newGameData);
			}
		}
		
		//find the data with the highest level and use that one
		Level highestLevel = Level.Level1;
		PetGameData gameDataToKeep = null;
		string petIDToKeep = "";
		
		foreach(PetGameData gameData in existingData){
			if(gameData.Level.CurrentLevel >= highestLevel){
				highestLevel = gameData.Level.CurrentLevel;
				gameDataToKeep = gameData;
				petIDToKeep = gameData.PetInfo.PetID;
			}
		}
		
		//serialize the game data into a new player pref
		gameDataToKeep.PetInfo.PetID = "Pet0"; //reset the ID to 0
		string serializedString = JSON.Instance.ToJSON(gameDataToKeep);
		PlayerPrefs.SetString("GameData", serializedString);
		
		//delete everything from the old player pref. ex Pet0_GameData
		for(int index=0; index < 3; index++){
			PlayerPrefs.DeleteKey("Pet" + index + "_GameData");
		}

		return petIDToKeep;
	}

	/// <summary>
	/// Modifies the menu scene data for single mode.
	/// v1.2.7
	/// </summary>
	/// <param name="petIDToKeep">Pet identifier to keep.</param>
	private void ModifyMenuSceneDataForSingleMode(string petIDToKeep){
		Dictionary<string, MutableDataPetMenuInfo> oldMenuSceneData = 
			new Dictionary<string, MutableDataPetMenuInfo>();
		string jsonString = PlayerPrefs.GetString("MenuSceneData", "");
		
		if(!String.IsNullOrEmpty(jsonString))
			oldMenuSceneData = JSON.Instance.ToObject<Dictionary<string, MutableDataPetMenuInfo>>(jsonString);
		
		foreach(KeyValuePair<string, MutableDataPetMenuInfo> petMenuInfo in oldMenuSceneData){
			if(petMenuInfo.Key != petIDToKeep){
				oldMenuSceneData.Remove(petMenuInfo.Key);
			}else{
				menuSceneData = petMenuInfo.Value;
			}
		}
		
		SaveMenuSceneData();
	}


	//Load the data of the pet that has been chosen
	public void LoadGameData(){
//		if(!String.IsNullOrEmpty(currentPetID)){
			PetGameData newGameData = null;
			string jsonString = PlayerPrefs.GetString("GameData", "");

			//Check if json string is actually loaded and not empty
			if(!String.IsNullOrEmpty(jsonString)){
				newGameData = JSON.Instance.ToObject<PetGameData>(jsonString);

				#if UNITY_EDITOR
                Debug.Log("Deserialized: " + jsonString);
				#endif

				gameData = newGameData;
				gameData.VersionCheck();

				Deserialized(true);
			}
			else{
				Deserialized(false);
//				Debug.LogError("Cannot retrieve json string for " + currentPetID +
//					" .Check to make sure it exists");
			}
//		}
//		else{
//			Deserialized(false);
//			Debug.LogError("PetID is null or empty. Can't load pet without ID. Check  currentPetID");
//		}
	}

	//serialize data into byte array and store locally in PlayerPrefs
	public void SaveGameData(){
		// hopefully we are safe here, but do an absolute check if a tutorial is playing
		if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()){
			Debug.LogError("Something trying to save the game while a tutorial is playing.  Investigate.");
			return;
		}
        
		#if UNITY_EDITOR
            Debug.Log("Game is saving");
		#endif

		//Data will not be saved if petID and gameData is empty
		if(gameData != null){
			string jsonString = JSON.Instance.ToJSON(gameData);

			#if UNITY_EDITOR
            Debug.Log("SERIALIZED: " + jsonString);
			#endif

			PlayerPrefs.SetString("GameData", jsonString); 
			Serialized();

		}
		else{
			Debug.LogError("PetID is null or empty, so data cannot be serialized");
		}
	}

	//----------------------------------------------------
	// LoadMenuSceneData()
	// Deserialize menu scene data
	//----------------------------------------------------
	private void LoadMenuSceneData(){
//		menuSceneData = new MutableDataPetMenuInfo();
		string jsonString = PlayerPrefs.GetString("MenuSceneData", "");

		//Check if json string is actually loaded and not empty
		if(!String.IsNullOrEmpty(jsonString))
			menuSceneData = JSON.Instance.ToObject<MutableDataPetMenuInfo>(jsonString);
	}

	//----------------------------------------------------
	// SaveMenuSceneData()
	// Serialize menu scene data.
	//----------------------------------------------------
	private void SaveMenuSceneData(){
		if(menuSceneData != null){
			string jsonString = JSON.Instance.ToJSON(menuSceneData);
			PlayerPrefs.SetString("MenuSceneData", jsonString);
		}
	}

	//called when game data has be deserialized. could be successful or failure 
	private void Deserialized(bool isSuccessful){
		SerializerEventArgs args = new SerializerEventArgs();
		args.IsSuccessful = isSuccessful;

		if(OnGameDataLoaded != null)
			OnGameDataLoaded(this, args);
	}

	//called when game data has been serialized
	private void Serialized(){

		if(OnGameDataSaved != null)
			OnGameDataSaved(this, EventArgs.Empty);
	}
}
