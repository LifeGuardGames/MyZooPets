using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Parse;
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
	private float syncToParseTimer = 0f;
	private float syncToParseWaitTime = 60f; //60 seconds before data get sync to server

	/// <summary>
	/// Gets the menu scene data. Note: GetMenuSceneData or SetMenuSceneData is the preferred method
	/// </summary>
	/// <value>The menu scene data.</value>
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

	/// <summary>
	/// Gets or sets the number of pets.
	/// </summary>
	/// <value>The number of pets.</value>
	public int NumOfPets{
		get{ return PlayerPrefs.GetInt("NumOfPets", 0);}
		set{ PlayerPrefs.SetInt("NumOfPets", value);}
	}

	public bool IsMaxNumOfPet{
		get{ return NumOfPets >= 3;}
	}

	/// <summary>
	/// Use this to check if there is data loaded into gameData at anypoint
	/// in the menuscene
	/// </summary>
	public bool IsGameDataLoaded{
		get{ return gameData != null;}
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

		//Initialize Parse so it can be used right away
		ParseClient.Initialize("J0M9QYtJJ7mUCnplysgza2zX2fo5MUCCbuoniZnN", "jTbrSqfrOKlJbIn5b1STpFpVItqwF4HyrWgejJ5S");

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
			AddNewPet();

			//first time playing game so try to create Parse User and create Account
			UserCheckAndSyncToParse();
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
                
				Debug.Log("before game save");
				SaveGameData();

				//No longer first time
				PlayerPrefs.SetInt("IsFirstTime", 0);

				UserCheckAndSyncToParse();
			}
		}
	}

	void Update(){
		//this is the timer that will be running to keep track of when to sync data
		//to parse server
		bool isSyncToServerOn = Constants.GetConstant<bool>("IsSyncToServerOn");
		if(!isDebug && isSyncToServerOn){

			//waiting till auto sync time
			syncToParseTimer += Time.deltaTime;
			if(syncToParseTimer >= syncToParseWaitTime){
				syncToParseTimer = 0;
				if(gameData != null){
					Debug.Log("auto sync starting");
					gameData.SaveAsyncToParse();
				}
			}
		}
	}

	/// <summary>
	/// Add a new pet to the game. This function will initialize/save the MenuSceneData
	/// and the GameData for the new pet. GameData will be loaded with the new pets data after this
	/// functions is completed.
	/// </summary>
	public void AddNewPet(){
		//create menu scene data here
		string petID = "Pet" + NumOfPets;
		AddNewMenuSceneData();
		InitializeGameDataForNewPet(petID: petID);
		
		SaveMenuSceneData();
		//save the new game data right after it's created so we don't risk
		//losing it if the user decide to create another one right away.
		SaveGameData();
	}

	/// <summary>
	/// Removes the menu scene data. This will also remove the GameData of that pet
	/// and decrease the NumOfPets
	/// </summary>
	/// <param name="petID">Pet ID.</param>
	public void RemovePet(string petID){
		if(!string.IsNullOrEmpty(petID) && menuSceneData.ContainsKey(petID)){
			menuSceneData.Remove(petID);
			
			PlayerPrefs.DeleteKey(petID + "_GameData");
			NumOfPets--;
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
		
		SaveGameData(this.gameData);
	}
	
	/// <summary>
	/// Gets the menu scene data for petID
	/// </summary>
	/// <returns>The menu scene data.</returns>
	/// <param name="petID">Pet ID.</param>
	public MutableDataPetMenuInfo GetMenuSceneData(string petID){
		MutableDataPetMenuInfo retVal = null;
		
		if(!string.IsNullOrEmpty(petID) && menuSceneData.ContainsKey(petID)){
			retVal = menuSceneData[petID];
		}
		
		return retVal;
	}

	/// <summary>
	/// Sets the menu scene data.
	/// </summary>
	/// <param name="petID">Pet ID.</param>
	/// <param name="petMenuInfo">Pet menu info.</param>
	public void SetMenuSceneData(string petID, MutableDataPetMenuInfo petMenuInfo){
		if(!string.IsNullOrEmpty(petID) && menuSceneData.ContainsKey(petID)){
			menuSceneData[petID] = petMenuInfo;
		}
	}
	
	/// <summary>
	/// Loads the game data from PlayerPrefs.
	/// </summary>
	public void LoadGameData(string petID = "Pet0"){
		//if gameData is not null then the gameData needs to be saved first
		//otherwise loading the new data will erase the current gameData
		if(gameData == null){
			LoadGameDataHelper(petID);
		}
		else{
			//gameData not null && tyring to load gameData of a different pet
			//the current gameData need to be serialized first otherwise data will
			//be lost
			if(gameData.PetInfo.PetID != petID){
				PetGameData oldGameData = this.gameData;
				SaveGameData(oldGameData);
				
				LoadGameDataHelper(petID);
			}
			else{
				Deserialized(true);
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
	private void InitializeGameDataForNewPet(string petID = "Pet0", string petName = "", 
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

	/// <summary>
	/// Loads the game data from PlayerPrefs.
	/// </summary>
	private void LoadGameDataHelper(string petID){
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

	/// <summary>
	/// Adds the new menu scene data.
	/// </summary>
	/// <param name="petMenuInfo">Pet menu info.</param>
	private void AddNewMenuSceneData(MutableDataPetMenuInfo petMenuInfo = null){
		if(!IsMaxNumOfPet){
			if(menuSceneData == null)
				menuSceneData = new Dictionary<string, MutableDataPetMenuInfo>();
			
			string petID = "Pet" + NumOfPets;
			if(petMenuInfo == null)
				menuSceneData.Add(petID, new MutableDataPetMenuInfo());
			else
				menuSceneData.Add(petID, petMenuInfo);

			NumOfPets++;
		}
	}
	
	/// <summary>
	/// Checks if Parse user exists. if not creates a user then try to sync all
	/// dirty data to parse.
	/// </summary>
	private void UserCheckAndSyncToParse(){
		bool isSyncToBackendOn = Constants.GetConstant<bool>("IsSyncToServerOn");
		if(!isDebug && isSyncToBackendOn)
			gameData.SaveAsyncToParse();
	}

	/// <summary>
	/// Versions the check. Handles any major data schema changes to the DataManager
	/// </summary>
	/// <param name="currentDataVersion">Current data version.</param>
	private void VersionCheck(Version currentDataVersion){
		Version version134 = new Version("1.3.4"); //change version number before push to store

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

			//need to create ParseUser here as well
			bool isSyncToBackendOn = Constants.GetConstant<bool>("IsSyncToServerOn");
			if(!isDebug && isSyncToBackendOn)
				ExtraParseLogic.Instance.UserCheck();
		}
	}

	/// <summary>
	/// Serialize data into json string and store locally in PlayerPrefs
	/// </summary>
	private void SaveGameData(PetGameData dataToSave){
		//Data will not be saved if gameData is empty
		if(dataToSave != null){
			string jsonString = JSON.Instance.ToJSON(dataToSave);
			
			#if UNITY_EDITOR
			Debug.Log("SERIALIZED: " + jsonString);
			#endif
			
			string currentPetID = dataToSave.PetInfo.PetID;
			PlayerPrefs.SetString(currentPetID + "_GameData", jsonString);
			
			SaveDataVersion();
			Serialized();
		}
		else{
			Debug.LogError("PetID is null or empty, so data cannot be serialized");
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
