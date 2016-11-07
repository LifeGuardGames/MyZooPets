using UnityEngine;
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
		get{ return Application.version; }
	}

	public bool isDebug = false; //turn isDebug to true if working on independent scene

	private static bool isCreated;

	#region Properties
	private PetGameData gameData; //Super class that stores all the game data related to a specific petID
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

	public bool IsAdsEnabled{
		get{
			//default set to true
			if(!PlayerPrefs.HasKey("AdsEnabled")){
				PlayerPrefs.SetInt("AdsEnabled", 1);
			}
			return PlayerPrefs.GetInt("AdsEnabled", 1) == 1;
		}
		set{
			PlayerPrefs.SetInt("AdsEnabled", value ? 1 : 0);
			PlayerPrefs.Save();
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
#if DEVELOPMENT_BUILD
		PlayerPrefs.DeleteAll();
#endif
		Amplitude amplitude = Amplitude.Instance;
		//Live Amplitude
		//amplitude.logging = true;
		//amplitude.init("");
		//Dev Amplitude

		amplitude.logging = true;
		amplitude.init("a06f151d06c754bdbbff7bdbaffe12e2");

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

			LoadGameData();
		}
	}

	
	//Serialize the data whenever the game is paused
	void OnApplicationPause(bool paused){
		if(paused){
//			#if DEVELOPMENT_BUILD
//				return;
//			#endif
			Debug.Log("APPLICATION PAUSE CALLED");

			// check immediately if a tutorial is playing...if one is, we don't want to save the game on pause
			if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()){
				Debug.Log("Auto save canceled because we are in a tutorial");
				return;
			}
			
			// also early out if we happen to be in the inhaler game.  Ultimately we may want to create a more elaborate hash/list
			// of scenes it is okay to save in, if we ever create more scenes that shouldn't serialize data
			if(SceneUtils.CurrentScene == SceneUtils.INHALERGAME){
				Debug.Log("Not saving the game because its inhaler scene");
				return;
			}

			//game can be paused at anytime and sometimes MenuScene doesn't have
			//any thing that needs saving, so check before saving
			if(gameData != null){
				// special case: when we are about to serialize the game, we have to cache the moment it happens so we know when the user stopped
				GameData.PlayPeriod.LastTimeUserPlayedGame = LgDateTime.GetTimeNow();
                
				// Save last play period here again..
				if(PlayPeriodLogic.Instance != null){
					PlayPeriodLogic.Instance.SetLastPlayPeriod();
				}

				Analytics.Instance.QuitGame(SceneUtils.CurrentScene);
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
			if(!string.IsNullOrEmpty(jsonString)){
				newGameData = JSON.Instance.ToObject<PetGameData>(jsonString);
				
				#if UNITY_EDITOR
				Debug.Log("Deserialized: " + jsonString);
				#endif
				
				gameData = newGameData;

				if(isDebug && Constants.GetConstant<bool>("ForceSecondPlayPeriod")){
					Debug.Log("Setting dummy data for second play period");
					SetDummyDataForSecondPlayPeriod();
				}

				LoadDataVersion();
				
				Deserialized();
			}
			else{
				//initiate game data here because none is found in the PlayerPrefs
				gameData = new PetGameData();

				if(isDebug && Constants.GetConstant<bool>("ForceSecondPlayPeriod")){
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
	/// NOTE: Dont touch anything in here
	/// </summary>
	/// <param name="currentDataVersion">Current data version.</param>
	private void VersionCheck(Version currentDataVersion){
		//Deleting all data that is less than v2.0.0	//DONT TOUCH THIS
		if(currentDataVersion < new Version("2.0.0")) {
			PlayerPrefs.DeleteKey("GameData");
		}

		// Disabling ads for all users below v2.2.0 (premium users)		//DONT TOUCH THIS
		//if(currentDataVersion < new Version ("2.2.0")) {
		//	IsAdsEnabled = false;
		//}
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
