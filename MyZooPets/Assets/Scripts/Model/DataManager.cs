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

	private static bool isCreated;
	private PetGameData gameData; //Super class that stores all the game data related to a specific petID

	//basic info data of all the pet that are only used in the menu scene
	private Dictionary<string, MutableDataPetMenuInfo> menuSceneData; // key: petID, value: instance of MutableDataPetInfo

	//Return menu scene data used in MenuScene
	public Dictionary<string, MutableDataPetMenuInfo> MenuSceneData{
		get{ return menuSceneData; }
	}

	/// <summary>
	/// Gets the game data.
	/// </summary>
	/// <value>The game data.</value>
	public PetGameData GameData{
		get{ return gameData; }
	}
    
	/// <summary>
	/// Gets or sets the scene data. temporary data when transition to new scene
	/// </summary>
	/// <value>The scene data.</value>
	public LoadSceneData SceneData{ get; set; } 
	
	/// <summary>
	/// Gets a value indicating whether if it's user's first time launching app.
	/// first session ends when application is paused (i.e home out or quit)
	/// </summary>
	public bool IsFirstTime{
		get{
			bool firstTime = PlayerPrefs.GetInt("IsFirstTime", 1) > 0;
		
			return firstTime;
		}
	}

	public int NumOfPets{
		get{
			return PlayerPrefs.GetInt("NumOfPets", 0);
		}
		set{
			PlayerPrefs.SetInt("NumOfPets", value);
		}
	}

	/// <summary>
	/// Use this to check if there is data loaded into gameData at anypoint
	/// in the menuscene
	/// </summary>
	public bool IsGameDataLoaded{
		get{
			return gameData != null;
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
		if(isDebug)
			InitializeGameDataForNewPet();

		// if not first time need to do version check
		if(!IsFirstTime){
			string currentDataVersionString = PlayerPrefs.GetString("CurrentDataVersion", "1.3.0");
			VersionCheck(new Version(currentDataVersionString));
		}
		// else create data for pet0 by default
		else{
			AddNewMenuSceneData();
			SaveMenuSceneData();
		}
			
		LoadMenuSceneData();
	}

	//Serialize the data whenever the game is paused
	void OnApplicationPause(bool paused){
		if(paused){
			#if DEVELOPMENT_BUILD
				return;
			#endif

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

				//No longer first time
				PlayerPrefs.SetInt("IsFirstTime", 0);
			}
		}
	}

	/// <summary>
	/// Initializes the game data for new pet.
	/// </summary>
	/// <param name="petID">Pet ID.</param>
	/// <param name="petName">Pet name.</param>
	/// <param name="petSpecies">Pet species.</param>
	/// <param name="petColor">Pet color.</param>
	public void InitializeGameDataForNewPet(string petID = "Pet0", string petName = "", 
	                                        string petSpecies = "Basic", string petColor = "OrangeYellow"){

		gameData = new PetGameData();

		if(!String.IsNullOrEmpty(petName))
			gameData.PetInfo.PetName = petName;
		else
			gameData.PetInfo.PetName = "Player" + NumOfPets;

		gameData.PetInfo.PetColor = petColor;
		gameData.PetInfo.IsHatched = true;
		gameData.PetInfo.PetID = petID;
           
		if(!isDebug){
			if(menuSceneData.ContainsKey(petID)){
				menuSceneData[petID].PetName = gameData.PetInfo.PetName;
				menuSceneData[petID].PetColor = petColor;
				menuSceneData[petID].PetSpecies = petSpecies;
			}
		}
	}

//	/// <summary>
//	/// Modifies the data into single pet mode.
//	/// v1.2.7
//	/// </summary>
//	private void ModifyDataIntoSinglePetMode(){
//		//check if game is already in single mode. default to false;
//		bool isSinglePetMode = PlayerPrefs.GetInt("IsSinglePetMode", 0) > 0;
//		//used to have 3 pets, but switching to one pet
//
//		//return if it's already in single pet mode
//		if(isSinglePetMode) return;
//
//		string petIDToKeep = ModifyGameDataForSingleMode();
//
//		ModifyMenuSceneDataForSingleMode(petIDToKeep);
//
//		//Set single mode to true
//		PlayerPrefs.SetInt("IsSinglePetMode", 1);
//	}
//
//	/// <summary>
//	/// Modifies the game data for single mode.
//	/// v1.2.7
//	/// </summary>
//	/// <returns>petID that will be kept.</returns>
//	private string ModifyGameDataForSingleMode(){
//		//deserialize all data. 
//		List<PetGameData> existingData = new List<PetGameData>();
//		
//		for(int index=0; index < 3; index++){
//			string jsonString = PlayerPrefs.GetString("Pet" + index + "_GameData", "");
//			
//			if(!String.IsNullOrEmpty(jsonString)){
//				PetGameData newGameData = JSON.Instance.ToObject<PetGameData>(jsonString);
//				existingData.Add(newGameData);
//			}
//		}
//		
//		//find the data with the highest level and use that one
//		Level highestLevel = Level.Level1;
//		PetGameData gameDataToKeep = null;
//		string petIDToKeep = "";
//		
//		foreach(PetGameData data in existingData){
//			if(data.Level.CurrentLevel >= highestLevel){
//				highestLevel = data.Level.CurrentLevel;
//				gameDataToKeep = data;
//				petIDToKeep = data.PetInfo.PetID;
//			}
//		}
//
//		
//		//serialize the game data into a new player pref
//		if(gameDataToKeep != null)
//			gameDataToKeep.PetInfo.PetID = "Pet0"; //reset the ID to 0
//		gameData = gameDataToKeep;
//
//		string serializedString = JSON.Instance.ToJSON(gameDataToKeep);
//		if(!String.IsNullOrEmpty(serializedString))
//			PlayerPrefs.SetString("GameData", serializedString);
//
//		//delete everything from the old player pref. ex Pet0_GameData
//		for(int index=0; index < 3; index++){
//			PlayerPrefs.DeleteKey("Pet" + index + "_GameData");
//		}
//
//		//Usually don't want to do this but we are modifying data so want to update it
//		//right away
//		PlayerPrefs.Save();
//
//		return petIDToKeep;
//	}
//
//	/// <summary>
//	/// Modifies the menu scene data for single mode.
//	/// v1.2.7
//	/// </summary>
//	/// <param name="petIDToKeep">Pet identifier to keep.</param>
//	private void ModifyMenuSceneDataForSingleMode(string petIDToKeep){
//		Dictionary<string, MutableDataPetMenuInfo> oldMenuSceneData = 
//			new Dictionary<string, MutableDataPetMenuInfo>();
//		string jsonString = PlayerPrefs.GetString("MenuSceneData", "");
//		
//		if(!String.IsNullOrEmpty(jsonString)){
//			oldMenuSceneData = JSON.Instance.ToObject<Dictionary<string, MutableDataPetMenuInfo>>(jsonString);
//			List<string> keysToBeRemoved = new List<string>();
//
//			foreach(KeyValuePair<string, MutableDataPetMenuInfo> petMenuInfo in oldMenuSceneData){
//				if(petMenuInfo.Key != petIDToKeep){
//					keysToBeRemoved.Add(petMenuInfo.Key);
//				}else{
//					menuSceneData = petMenuInfo.Value;
//				}
//			}
//
//			//remove unwanted data
//			foreach(string key in keysToBeRemoved){
//				if(oldMenuSceneData.ContainsKey(key))
//					oldMenuSceneData.Remove(key);
//			}
//			
//			SaveMenuSceneData();
//		}
//	}

	
	/// <summary>
	/// Loads the game data from PlayerPrefs.
	/// </summary>
	public void LoadGameData(string petID = "Pet0"){
		if(gameData == null || gameData.PetInfo.PetID != petID){
			PetGameData newGameData = null;
			string jsonString = PlayerPrefs.GetString(petID + "_GameData", "");
			
			//Check if json string is actually loaded and not empty
			if(!String.IsNullOrEmpty(jsonString)){
				newGameData = JSON.Instance.ToObject<PetGameData>(jsonString);
				
				#if UNITY_EDITOR
				Debug.Log("Deserialized: " + jsonString);
				#endif
				
				gameData = newGameData;
				LoadDataVersion();
				
				Deserialized(true);
			}
			else{
				Deserialized(false);
			}
		}
		else{
			Deserialized(true);
		}
	}

	/// <summary>
	/// Serialize data into json string and store locally in PlayerPrefs
	/// </summary>
	public void SaveGameData(){
		// hopefully we are safe here, but do an absolute check if a tutorial is playing
		if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()){
			return;
		}
        
		#if UNITY_EDITOR
            Debug.Log("Game is saving");
		#endif

		//Data will not be saved if gameData is empty
		if(gameData != null){
			string jsonString = JSON.Instance.ToJSON(gameData);

			#if UNITY_EDITOR
            Debug.Log("SERIALIZED: " + jsonString);
			#endif

			string currentPetID = gameData.PetInfo.PetID;
			PlayerPrefs.SetString(currentPetID + "_GameData", jsonString);

			SaveDataVersion();
			Serialized();
		}
		else{
			Debug.LogError("PetID is null or empty, so data cannot be serialized");
		}
	}

	public void AddNewMenuSceneData(MutableDataPetMenuInfo petMenuInfo = null){
		if(menuSceneData == null)
			menuSceneData = new Dictionary<string, MutableDataPetMenuInfo>();

		string petID = "Pet" + NumOfPets;
		if(petMenuInfo == null)
			menuSceneData.Add(petID, new MutableDataPetMenuInfo());
		else
			menuSceneData.Add(petID, petMenuInfo);

		NumOfPets++;
//		SaveMenuSceneData();
	}

	private void VersionCheck(Version currentDataVersion){
		Version version134 = new Version("1.3.4");

		// Bring back the code that supports multiple pets
		if(currentDataVersion < version134){
			string menuSceneJSONString = PlayerPrefs.GetString("MenuSceneData", "");
			MutableDataPetMenuInfo oldMenuSceneData = null;
			if(!String.IsNullOrEmpty(menuSceneJSONString)){
				oldMenuSceneData = JSON.Instance.ToObject<MutableDataPetMenuInfo>(menuSceneJSONString);

				AddNewMenuSceneData(oldMenuSceneData);
			}

			//load the old game data json
			string oldGameDataJSONString = PlayerPrefs.GetString("GameData", "");
			if(!String.IsNullOrEmpty(oldGameDataJSONString)){
				//assign the json string to a new PlayerPrefs key that has the petID
				PlayerPrefs.SetString("Pet0_GameData", oldGameDataJSONString);

				//remove the old player pref key
				PlayerPrefs.DeleteKey("GameData");
				PlayerPrefs.DeleteKey("IsSinglePetMode");
				PlayerPrefs.Save();
			}
		}
	}

	/// <summary>
	/// Loads the data version.
	/// use the data version for version check
	/// </summary>
	private void LoadDataVersion(){
		//don't change the default value
		string currentDataVersionString = PlayerPrefs.GetString("CurrentDataVersion", "1.3.0");
		
		if(!IsFirstTime)
			gameData.VersionCheck(new Version(currentDataVersionString));
	}

	/// <summary>
	/// Saves the data version.
	/// This steps is important because it updates the data version number to 
	/// the lastest build number
	/// </summary>
	private void SaveDataVersion(){
		string buildVersionString = Constants.GetConstant<string>("BuildVersion");
		string currentDataVersionString = PlayerPrefs.GetString("CurrentDataVersion", "1.3.0");
		Version buildVersion = new Version(buildVersionString);
		Version currentDataVersion = new Version(currentDataVersionString);
		
		if(buildVersion > currentDataVersion)
			PlayerPrefs.SetString("CurrentDataVersion", buildVersionString);
	}
	
	/// <summary>
	/// Loads the menu scene data.
	/// </summary>
	private void LoadMenuSceneData(){
		string jsonString = PlayerPrefs.GetString("MenuSceneData", "");

		//Check if json string is actually loaded and not empty
		if(!String.IsNullOrEmpty(jsonString))
			menuSceneData = JSON.Instance.ToObject<Dictionary<string, MutableDataPetMenuInfo>>(jsonString);
	}
	
	/// <summary>
	/// Saves the menu scene data.
	/// </summary>
	private void SaveMenuSceneData(){
		if(menuSceneData != null){
			string jsonString = JSON.Instance.ToJSON(menuSceneData);
			PlayerPrefs.SetString("MenuSceneData", jsonString);
		}
	}
	
	/// <summary>
	/// Called when game data has been deserialized. Could be successful or failure
	/// </summary>
	private void Deserialized(bool isSuccessful){
		SerializerEventArgs args = new SerializerEventArgs();
		args.IsSuccessful = isSuccessful;

		if(OnGameDataLoaded != null)
			OnGameDataLoaded(this, args);
	}
	
	/// <summary>
	/// Called when game data has been serialized
	/// </summary>
	private void Serialized(){

		if(OnGameDataSaved != null)
			OnGameDataSaved(this, EventArgs.Empty);
	}
}
