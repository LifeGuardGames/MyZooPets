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

	public event EventHandler<EventArgs> OnGameDataLoaded;
	public event EventHandler<EventArgs> OnGameDataSaved;

	public string CURRENT_VERSION{
		get{
			return Constants.GetConstant<string>("BuildVersion");
		}
	}

	public bool isDebug = false; //turn isDebug to true if working on independent scene

	private static bool isCreated;
	private PetGameData gameData; //Super class that stores all the game data related to a specific petID
//	private float syncToParseTimer = 0f;
//	private float syncToParseWaitTime = 30f; //30 seconds before data get sync to server

	#region Properties
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

//	/// <summary>
//	/// If membership check failed a code will be passed from the LoadingScene to
//	/// the MenuScene.
//	/// </summary>
//	/// <value>The membership check failed code.</value>
//	public string MembershipCheckFailedCode{get; set;}
	
	/// <summary>
	/// Gets a value indicating whether if it's user's first time launching app.
	/// first session ends when application is paused (i.e home out or quit)
	/// </summary>
	public bool IsFirstTime{
		get{
			//default to true
			return PlayerPrefs.GetInt("IsFirstTime", 1) > 0;
		}
	}

	/// <summary>
	/// Checks if the player 
	/// </summary>
	public bool IsQuestionaireCollected{
		get{
			//default to false
			return PlayerPrefs.GetInt("IsQuestionaireCollected", 0) > 0;
		}
		set{
			if(value){
				PlayerPrefs.SetInt("IsQuestionaireCollected", 1);
			}
		}
	}

	/// <summary>
	/// Gets or sets the membership check dates. Keep track of when membership check
	/// happens. Also used for calculating when the trial period expires. Data will
	/// be reset if trialStatus == expired, membershipStatus == active || expired.
	/// </summary>
	/// <value>The membership check dates.</value>
//	public string MembershipCheckDates{
//		get{
//			return PlayerPrefs.GetString("MembershipCheckDates", "");
//		}
//		set{
//			PlayerPrefs.SetString("MembershipCheckDates", value);
//		}
//	}

	/// <summary>
	/// Adds the membership check date. Save date in a comma deliminated string
	/// ex. 14908789,14908382,14i09090
	/// </summary>
	/// <param name="timestamp">Timestamp.</param>
//	public void AddMembershipCheckDate(string timestamp){
//		string currentDates = MembershipCheckDates;
//		if(!string.IsNullOrEmpty(timestamp)){
//			if(!string.IsNullOrEmpty(currentDates))
//				//Add new dates using a comma as the deliminating character
//				MembershipCheckDates = currentDates + "," + timestamp;
//			else
//				MembershipCheckDates = timestamp;
//		}
//	}
//
//	public void ResetMembershipCheckDates(){
//		MembershipCheckDates = "";
//	}

	/// <summary>
	/// Tracks the connection error allowed before game is locked and user will 
	/// be forced to go online before continue playing
	/// </summary>
	/// <value>The accumulated connection errors.</value>
//	public int AccumulatedConnectionErrors{
//		get{
//			return PlayerPrefs.GetInt("ConnectionErrors", 0);
//		}
//		set{
//			PlayerPrefs.SetInt("ConnectionErrors", value);
//		}
//	}

	/// <summary>
	/// Gets or sets the last play session date. Use to determine whether the game
	/// should be force start from LoadingScene.unity
	/// </summary>
	/// <value>The last play session date.</value>
//	public DateTime LastPlaySessionDate{
//		get{
//			string timeString = PlayerPrefs.GetString("LastPlaySessionDate", "");
//			DateTime lastSessionTime = LgDateTime.GetTimeNow();
//
//			if(!string.IsNullOrEmpty(timeString))
//				lastSessionTime = Convert.ToDateTime(timeString);
//		
//			return lastSessionTime;
//		}
//		set{
//			string timeString = value.ToString("o");
//			PlayerPrefs.SetString("LastPlaySessionDate", timeString);
//		}
//	}

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
		#if DEVELOPMENT_BUILD
		PlayerPrefs.DeleteAll();
		#endif

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
			InitGameDataForDebug();
		else{
			// if not first time need to do version check, retrieve old version
			if(!IsFirstTime){
				string currentDataVersionString = PlayerPrefs.GetString("CurrentDataVersion", CURRENT_VERSION);
				VersionCheck(new Version(currentDataVersionString));
			}
//			else{
//				bool isSyncToServerOn = Constants.GetConstant<bool>("IsSyncToServerOn");
//				if(isSyncToServerOn)
//					ExtraParseLogic.Instance.UserCheck();
//			}

			LoadGameData();
		}
	}

	
	//Serialize the data whenever the game is paused
	void OnApplicationPause(bool paused){
		if(paused){
//			#if DEVELOPMENT_BUILD
//				return;
//			#endif

			// check immediately if a tutorial is playing...if one is, we don't want to save the game on pause
			if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()){
				Debug.Log("Auto save canceled because we are in a tutorial");
				return;
			}
			
			// also early out if we happen to be in the inhaler game.  Ultimately we may want to create a more elaborate hash/list
			// of scenes it is okay to save in, if we ever create more scenes that shouldn't serialize data
			if(Application.loadedLevelName == SceneUtils.INHALERGAME){
				Debug.Log("Not saving the game because its inhaler scene");
				return;
			}

			//game can be paused at anytime and sometimes MenuScene doesn't have
			//any thing that needs saving, so check before saving
			if(gameData != null){
				// special case: when we are about to serialize the game, we have to cache the moment it happens so we know when the user stopped
				DataManager.Instance.GameData.PlayPeriod.LastTimeUserPlayedGame = LgDateTime.GetTimeNow();
                
				// Save last play period here again..
				if(PlayPeriodLogic.Instance != null){
					PlayPeriodLogic.Instance.SetLastPlayPeriod();
				}
				Analytics.Instance.QuitGame(Application.loadedLevelName);
				SaveGameData();

				//No longer first time
				PlayerPrefs.SetInt("IsFirstTime", 0);
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
//		Debug.Log("Game is saving");
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
	/// Loads the game data from PlayerPrefs. If no game data is found from PlayerPrefs
	/// a new game data will be initiated. This function should only be called once when the
	/// game loads in LoadingScene.
	/// </summary>
	public void LoadGameData(){
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

				if(Constants.GetConstant<bool>("ForceSecondPlayPeriod")){
					Debug.Log("Setting dummy data for second play period");
					SetDummyDataForSecondPlayPeriod();
				}

				LoadDataVersion();
				
				Deserialized();
			}
			else{
				//initiate game data here because none is found in the PlayerPrefs
				gameData = new PetGameData();

				if(Constants.GetConstant<bool>("ForceSecondPlayPeriod")){
					Debug.Log("Setting dummy data for second play period");
					SetDummyDataForSecondPlayPeriod();
				}

				Deserialized();
			}
		}
	}
	#endregion

	public void SetDummyDataForSecondPlayPeriod(){
		IsQuestionaireCollected = true;
		gameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_INHALER);
		gameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_SUPERWELLA_INHALER);
		gameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_WELLAPAD);
		gameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_SMOKE_INTRO);
		gameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_FLAME_CRYSTAL);
		gameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_FLAME);
	}

	/// <summary>
	/// Modify Pet Info. Used in the MenuScene. Will trigger game data to be serialized
	/// immediately. 
	/// </summary>
	/// <param name="petID">Pet ID.</param>
	/// <param name="petName">Pet name.</param>
	/// <param name="petSpecies">Pet species.</param>
	/// <param name="petColor">Pet color.</param>
	public void ModifyBasicPetInfo(string petID = "Pet0", string petName = "", 
	                                        string petSpecies = "Basic", string petColor = "OrangeYellow"){

		if(!String.IsNullOrEmpty(petName))
			gameData.PetInfo.ChangeName(petName);
		else
			gameData.PetInfo.ChangeName("Player1");

		gameData.PetInfo.PetColor = petColor;
		gameData.PetInfo.IsHatched = true;
		gameData.PetInfo.PetID = petID;

		//serialize data right away
    	SaveGameData();
	}

	public void InitGameDataForDebug(){
		gameData = new PetGameData();
	}

	#region Data Version
	/// <summary>
	/// Checks the version. Handles any major data schema changes to the DataManager
	/// </summary>
	/// <param name="currentDataVersion">Current data version.</param>
	private void VersionCheck(Version currentDataVersion){
		//Deleting all data that is less than 2.0.0
		Version version200 = new Version("2.0.0");	//DONT TOUCH THIS
		if(currentDataVersion < version200){
			PlayerPrefs.DeleteKey("GameData");
		}
	}

	/// <summary>
	/// Loads the data version.
	/// use the data version for version check
	/// </summary>
	private void LoadDataVersion(){
		string currentDataVersionString = PlayerPrefs.GetString("CurrentDataVersion", CURRENT_VERSION);

		if(!IsFirstTime){
			gameData.VersionCheck(new Version(currentDataVersionString));
		}
	}

	/// <summary>
	/// Saves the data version.
	/// This steps is important because it updates the data version number to 
	/// the lastest build number
	/// </summary>
	private void SaveDataVersion(){
		PlayerPrefs.SetString("CurrentDataVersion", CURRENT_VERSION);
	}
	#endregion

	/// <summary>
	/// Called when game data has been deserialized. Could be successful or failure
	/// </summary>
	private void Deserialized(){
		if(OnGameDataLoaded != null){
			OnGameDataLoaded(this, EventArgs.Empty);
		}
	}
	
	/// <summary>
	/// Called when game data has been serialized
	/// </summary>
	private void Serialized(){
		if(OnGameDataSaved != null){
			OnGameDataSaved(this, EventArgs.Empty);
		}
	}
}
