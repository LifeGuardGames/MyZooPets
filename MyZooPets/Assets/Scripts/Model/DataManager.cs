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
	private MutableDataPetMenuInfo menuSceneData; // key: petID, value: instance of MutableDataPetInfo
	private float syncToParseTimer = 0f;
	private float syncToParseWaitTime = 30f; //30 seconds before data get sync to server

	#region Properties
	/// <summary>
	/// Gets the menu scene data. Note: GetMenuSceneData or SetMenuSceneData is the preferred method
	/// </summary>
	/// <value>The menu scene data.</value>
	public MutableDataPetMenuInfo MenuSceneData{
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
			//default to true
			bool firstTime = PlayerPrefs.GetInt("IsFirstTime", 1) > 0;
		
			return firstTime;
		}
	}

	/// <summary>
	/// Gets a value indicating whether terms of service and privacy are accepted
	/// by the user
	/// </summary>
	public bool IsTermsAndPrivacyAccepeted{
		get{
			//default to false
			bool isAccepted = PlayerPrefs.GetInt("IsTermsAndPrivacyAccepted", 0) > 0;

			return isAccepted;
		}
		set{
			bool isAccepted = value;
			if(isAccepted)
				PlayerPrefs.SetInt("IsTermsAndPrivacyAccepted", 1);
		}
	}

	/// <summary>
	/// Use this to check if there is data loaded into gameData at anypoint
	/// in the menuscene
	/// </summary>
	public bool IsGameDataLoaded{
		get{ return gameData != null;}
	}
	#endregion

	#region Unity MonoBehaviours
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
		else{
			// if not first time need to do version check
			if(!IsFirstTime){
				string currentDataVersionString = PlayerPrefs.GetString("CurrentDataVersion", "1.3.0");
				VersionCheck(new Version(currentDataVersionString));
			}
			else{
				bool isSyncToServerOn = Constants.GetConstant<bool>("IsSyncToServerOn");
				if(isSyncToServerOn)
					ExtraParseLogic.Instance.UserCheck();
			}
			
			LoadMenuSceneData();
		}
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
	#endregion

	#region Game Data
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
			
			PlayerPrefs.SetString("GameData", jsonString);
			SaveDataVersion();
			Serialized();
		}
		else{
			Debug.LogError("gameData is null, so data cannot be serialized");
		}
	}
	
	/// <summary>
	/// Loads the game data from PlayerPrefs.
	/// </summary>
	public void LoadGameData(string petID = "Pet0"){
		//if gameData is not null then the gameData needs to be saved first
		//otherwise loading the new data will erase the current gameData
		if(gameData == null){
			PetGameData newGameData = null;
			string jsonString = PlayerPrefs.GetString("GameData", "");
			
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
	#endregion

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
			gameData.PetInfo.PetName = "Player1";

		gameData.PetInfo.PetColor = petColor;
		gameData.PetInfo.IsHatched = true;
		gameData.PetInfo.PetID = petID;
           
		if(!isDebug)
			menuSceneData = new MutableDataPetMenuInfo(gameData.PetInfo.PetName, petColor, petSpecies);
	}

	#region Data Version
	/// <summary>
	/// Versions the check. Handles any major data schema changes to the DataManager
	/// </summary>
	/// <param name="currentDataVersion">Current data version.</param>
	private void VersionCheck(Version currentDataVersion){
		Version version140 = new Version("1.4.0");
		
		if(currentDataVersion < version140){
			PlayerPrefs.DeleteKey("IsSinglePetMode");
			
			ExtraParseLogic.Instance.UserCheck();
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
	#endregion

	#region MenuScene Data
	/// <summary>
	/// Loads the menu scene data.
	/// </summary>
	private void LoadMenuSceneData(){
		string jsonString = PlayerPrefs.GetString("MenuSceneData", "");

		//Check if json string is actually loaded and not empty
		if(!String.IsNullOrEmpty(jsonString))
			menuSceneData = JSON.Instance.ToObject<MutableDataPetMenuInfo>(jsonString);
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
	#endregion
	
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
